using System.Collections;
using System.Collections.Generic;
using Kamaria.DropItem;
using Kamaria.Item;
using Kamaria.Player.Data;
using Kamaria.Utilities;
using Kamaria.Utilities.PoolingPattern;
using UnityEngine;

namespace Kamaria.Enemy.AIEnemy.Fish
{
    public enum FishAnimation
    {
        movement, normalAttack, isDead, isStun
    }
    
    public sealed class AIFish : BaseAI, IDamageable
    {
        [SerializeField] private AudioSource sfxAttack;
        [SerializeField] private AudioSource sfxSummon;
        [SerializeField] private Transform spawnBomb;
        [SerializeField] private List<Transform> targetBombAllAround;
        [SerializeField] private Vector3 fallPointRange;
        
        
        public int HowAttack { get; set; }
        
        #region ANIMATION_PARAMETER

        public int AnimMove { get; } = Animator.StringToHash(FishAnimation.movement.ToString());
        public int AnimNormalAttack { get; } = Animator.StringToHash(FishAnimation.normalAttack.ToString());
        public int AnimIsDead { get; } = Animator.StringToHash(FishAnimation.isDead.ToString());
        public int AnimIsStun { get; } = Animator.StringToHash(FishAnimation.isStun.ToString());

        #endregion

        protected override void OnEnable()
        {
            GetStatus();
            base.OnEnable();
            Initialize();
            animator.SetBool(AnimIsDead, false);
            animator.SetBool(AnimIsStun, false);
            animator.ResetTrigger(AnimNormalAttack);

            currentState = patrolState;
            
            currentState.EnterState();
            vfxStun.SetActive(false);
            vfxShield.SetActive(useGuard);
            if (useGuard)
            {
                healthBar.SetIconWeakWeapon(weakForWeapon);
            }
            
            if (useVfxSummon)
            {
                PlaySfxSummon();
            }
        }

        public override void Initialize()
        {
            patrolState = new PatrolState(this, statesData);
            alertState = new AlertState(this, statesData);
            chaseState = new ChaseState(this, statesData);
            escapState = new EscapeState(this, statesData);
            deadState = new DeadState(this, statesData);
            stunState = new StunState(this, statesData);
            attackState = new FishAttackState(this, statesData);

            statesData.Add(patrolState);
            statesData.Add(alertState);
            statesData.Add(chaseState);
            statesData.Add(escapState);
            statesData.Add(deadState);
            statesData.Add(stunState);
            statesData.Add(attackState);
        }

        private void Update()
        {
            currentState.UpdateState();
        }
        
        #region USE_WITH_ANIMATION

        public void Attack()
        {
            if (HowAttack == 0)
            {
                Attack1(FallTarget(target.transform, false, out Vector3 targetOut));
                PoolIndicatorBomb(targetOut);
            }
            else if (HowAttack == 1)
            {
                Attack1(FallTarget(target.transform, false, out Vector3 targetOut1));
                Attack1(FallTarget(target.transform, true, out Vector3 targetOut2));
                PoolIndicatorBomb(targetOut1);
                PoolIndicatorBomb(targetOut2);
            }
            else if (HowAttack == 2)
            {
                for (int i = 0; i < targetBombAllAround.Count; i++)
                {
                    Attack1(FallTarget(targetBombAllAround[i], false, out Vector3 targetOut));
                    PoolIndicatorBomb(targetOut);
                }
            }
        }
        
        private void Attack1(Vector3 fallTarget)
        {
            PoolManager.PoolObjectType poolObj = PoolManager.PoolObjectType.Bomb;
            
            var bomb = PoolManager.Instance.GetPooledObject(poolObj);
            
            if (bomb)
            {
                bomb.GetComponent<Bomb>().InitDamage(EnemyStatusData.Atk,
                    EffectAttack.None, Vector3.zero, 0, 0, false, 0);
                SetBombPool(bomb, fallTarget);
            }
        }

        private void PoolIndicatorBomb(Vector3 target)
        {
            PoolManager.PoolObjectType poolObj = PoolManager.PoolObjectType.IndicatorBomb;
            
            var indicatorBomb = PoolManager.Instance.GetPooledObject(poolObj);
            
            if (indicatorBomb)
            {
                indicatorBomb.GetComponent<Indicator>().Init(target);
                indicatorBomb.SetActive(true);
            }
        }

        private void SetBombPool(GameObject bomb , Vector3 target)
        {
            bomb.transform.position = spawnBomb.position;
            bomb.transform.rotation = Quaternion.identity;
            bomb.SetActive(true);
            bomb.GetComponent<Bomb>().Init(target, spawnBomb.position);
        }
        
        private Vector3 FallTarget(Transform target, bool isUseRange, out Vector3 targetOut)
        {
            if (Physics.Raycast(target.position,
                    Vector3.down,out RaycastHit hit,playerData.CharacterControllerData.GroundLayers))
            {
                fallPointRange.y = hit.point.y;
            }

            if (isUseRange)
            {
                var temp = target.position + new Vector3(
                    Random.Range(-fallPointRange.x, fallPointRange.x), 0, 0);
                targetOut = new Vector3(temp.x, fallPointRange.y, temp.z);
                return targetOut;
            }

            targetOut = new Vector3(target.position.x, fallPointRange.y, target.position.z);
            return targetOut;
        }

        #endregion

        protected override void SetDamageCollider(int damageCollider, bool isOpen)
        {
        }

        protected override void CalculatorDamage(int damagePercent, bool isOpen)
        {
        }

        public override void DropItem()
        {
            for (int i = 0; i < dropItemHandler.CountDropItem; i++)
            {
                if (!dropItemHandler.CanDrop()) continue; // If need random can drop.
            
                dropItemHandler.DropItem(out ItemsName dropItem);

                createItemDropManager.CreateItemDrop(dropItem, transform);
            }
        }

        protected override void TakeDamageBehavior(EffectAttack effectAttack, Vector3 explosionPos, float powerKnockback, float radiusKnockback,
            bool isAOE, float stunTime)
        {
            if (IsFirstDefinitelyAttack)
            {
                IsFirstDefinitelyAttack = false;
                DefinitelyAttack();
            }
            
            if (!IsStun)
            {
                if (effectAttack == EffectAttack.Stun)
                {
                    StunTime = stunTime;
                    NextState(statesData.Find(x => x.State == AIState.Stun));
                }
            }
            
            if (IsDefinitelyAttack) return;
            
            if (!IsStun)
            {
                RotationToTarget(false);
            }

            //agent.isStopped = true;

            if (effectAttack == EffectAttack.KnockBack)
            {
                canGetDamage = false;
                Debug.Log("KnockBack");

                StartCoroutine(CanGetDamage());
                StartCoroutine(KnockBack(explosionPos, powerKnockback, radiusKnockback, isAOE));
            }
        }

        private IEnumerator CanGetDamage()
        {
            yield return new WaitForSeconds(knockBackTime + 0.2f);
            canGetDamage = true;
        }
        
        private IEnumerator KnockBack(Vector3 explosionPos, float powerKnockback,
            float radiusKnockback, bool isAOE)
        {
            controllerRb.isKinematic = false;
            agent.isStopped = true;
            float time = knockBackTime;
            Vector3 direction = this.transform.position - explosionPos;
            direction.y = 0;
            
            if (isAOE)
            {
                while (time > 0)
                {
                    controllerRb.AddExplosionForce(powerKnockback, explosionPos, radiusKnockback, 0,
                        ForceMode.VelocityChange);
                    
                    time -= Time.deltaTime;
                    yield return null;
                }
            }
            else
            {
                while (time > 0)
                {
                    controllerRb.AddForce(direction.normalized * powerKnockback, ForceMode.VelocityChange);

                    time -= Time.deltaTime;
                    yield return null;
                }
            }
            controllerRb.isKinematic = true;
            agent.isStopped = false;
        }
        
        public override void ResetGetDamage()
        {
            controllerRb.isKinematic = true;
            agent.isStopped = false;
        }

        public override void ResetColliderAttackAndVFX()
        {
        }

        public void TakeDamage(int damage, EffectAttack effectAttack, Vector3 explosionPos, float powerKnockback,
            float radiusKnockback, bool isAOE, float stunTime, WeaponTypes weaponTypes, KeyStones keyStones)
        {
            if (!canGetDamage) return;

            int damageHp = damage;
            
            if (useGuard)
            {
                if (weakForWeapon == weaponTypes)
                {
                    Debug.Log($"Have1 = {EnemyStatusData.CurrentGuard}");
                    int damageGuard = keyStones == KeyStones.PowerStone ? getDamagePowerGuard : getDamageGuard;

                    if (EnemyStatusData.CurrentGuard > 0)
                    {
                        EnemyStatusData.CurrentGuard -= damageGuard;
                        Debug.Log($"Have2 = {EnemyStatusData.CurrentGuard}");
                    }

                    if (EnemyStatusData.CurrentGuard <= 0)
                    {
                        Debug.Log("Have3");
                        vfxShield.SetActive(false);
                    }
                }

                if (EnemyStatusData.CurrentGuard > 0)
                {
                    damageHp = (int)(damage - ((damage * percentDecreaseDamageHaveGuard) / 100));
                    Debug.Log($"Have {damageHp}");
                }
            }

            Debug.Log($"GET Damage by {weaponTypes}");
            
            if (currentState.State == AIState.Patrol)
            {
                NextState(statesData.Find(x => x.State == AIState.Chase));
            }

            EnemyStatusData.CurrentHealth -= damageHp;
            blinkEffect.Blink();
            if (useGuard)
            {
                healthBar.ShowBar(EnemyStatusData.CurrentHealth, EnemyStatusData.CurrentGuard);
            }
            else
            {
                healthBar.ShowBar(EnemyStatusData.CurrentHealth);
            }
            ShowTextDamage(damageHp);
            Debug.Log($"AI GetDamage = {damageHp}");
            Debug.Log(EnemyStatusData.CurrentHealth);
            if (EnemyStatusData.CurrentHealth <= 0)
            {
                UpdateQuest1(weaponTypes);
                
                if (questManagerSO.CurrentQuests != null)
                {
                    UpdateQuestDefeatTheInvader(questManagerSO.CurrentQuests.NameQuest);
                }
                
                Dead();
                return;
            }
           
            TakeDamageBehavior(effectAttack, explosionPos, powerKnockback, radiusKnockback, isAOE, stunTime);
        }
        
        protected override void Dead()
        {
            IsDead = true;
            agent.isStopped = true;
            capsuleCollider.enabled = false;
            canGetDamage = false;
            animator.SetBool(AnimIsDead, true);
        }

        public void PlaySfxAttack()
        {
            GetDataSound(sfxAttack, SoundClip.Sound.FishGrenadeStart);
            sfxAttack.Play();
        }
        
        private void PlaySfxSummon()
        {
            GetDataSound(sfxSummon, SoundClip.Sound.SharkSummon);
            sfxSummon.Play();
        }
        
        public override void StopSfx()
        {
            sfxAttack.Stop();
        }
    }
}