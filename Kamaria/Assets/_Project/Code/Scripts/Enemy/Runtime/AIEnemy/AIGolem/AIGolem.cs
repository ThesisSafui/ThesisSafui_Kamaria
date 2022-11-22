using System;
using System.Collections;
using System.Collections.Generic;
using Kamaria.Item;
using Kamaria.Player.Data;
using Kamaria.Utilities;
using Kamaria.Utilities.PoolingPattern;
using Kamaria.VFX_ALL;
using UnityEngine;

namespace Kamaria.Enemy.AIEnemy.Golem
{
    public enum GolemAnimation
    {
        movement, isDead, isStun, isChargeAttack, attack1, attack2, spinAttack, isSpinEnd
    }
    
    public sealed class AIGolem : BaseAI, IDamageable
    {
        [SerializeField] private AudioSource normalAttack;
        [SerializeField] private AudioSource spinAttack;
        [SerializeField] private AudioSource sfxSummon;
        [SerializeField] private float spinTime;
        [SerializeField] private List<GolemDamageCollider> damageColliders;
        [SerializeField] private GameObject indicatorAttack;
        [SerializeField] private List<GameObject> vfxSlash;

        public float SpinTime => spinTime;
        public List<GolemDamageCollider> DamageColliders => damageColliders;
        public GameObject IndicatorAttack => indicatorAttack;
        
        private GolemDamageCollider attackCollider;

        #region ANIMATION_PARAMETER

        public int AnimMove { get; } = Animator.StringToHash(GolemAnimation.movement.ToString());
        public int AnimAttack1 { get; } = Animator.StringToHash(GolemAnimation.attack1.ToString());
        public int AnimAttack2 { get; } = Animator.StringToHash(GolemAnimation.attack2.ToString());
        public int AnimSpinAttack { get; } = Animator.StringToHash(GolemAnimation.spinAttack.ToString());
        public int AnimIsChargeAttack { get; } = Animator.StringToHash(GolemAnimation.isChargeAttack.ToString());
        public int AnimIsSpinEnd { get; } = Animator.StringToHash(GolemAnimation.isSpinEnd.ToString());
        public int AnimIsDead { get; } = Animator.StringToHash(GolemAnimation.isDead.ToString());
        public int AnimIsStun { get; } = Animator.StringToHash(GolemAnimation.isStun.ToString());

        #endregion
        
        protected override void OnEnable()
        {
            GetStatus();
            base.OnEnable();
            Initialize();
            animator.SetBool(AnimIsDead, false);
            animator.SetBool(AnimIsStun, false);
            animator.SetBool(AnimIsSpinEnd, false);
            animator.SetBool(AnimIsChargeAttack, false);
            animator.ResetTrigger(AnimAttack1);
            animator.ResetTrigger(AnimAttack2);
            animator.ResetTrigger(AnimSpinAttack);

            currentState = patrolState;
            
            currentState.EnterState();
            vfxStun.SetActive(false);
            Debug.Log($"USE = {useGuard}");
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
            deadState = new DeadState(this, statesData);
            stunState = new StunState(this, statesData);
            attackState = new GolemAttackState(this, statesData);

            statesData.Add(patrolState);
            statesData.Add(alertState);
            statesData.Add(chaseState);
            statesData.Add(deadState);
            statesData.Add(stunState);
            statesData.Add(attackState);
        }

        private void Update()
        {
            currentState.UpdateState();
        }

        protected override void SetDamageCollider(int damageCollider, bool isOpen)
        {
            switch (damageCollider)
            {
                case 0:
                    attackCollider = damageColliders.Find(x => x.CollidersDamage == GolemCollidersDamage.NormalAttack);
                    break;
                case 1:
                    attackCollider = damageColliders.Find(x => x.CollidersDamage == GolemCollidersDamage.SpineAttack);
                    break;
                default:
                    Debug.Log("Golem have 0-1");
                    break;
            }
            
            SetDataAttack(isOpen, attackCollider);
        }
        
        private void SetDataAttack(bool isOpen, GolemDamageCollider attackCollider)
        {
            attackCollider.gameObject.SetActive(isOpen);
            EffectAttack = attackCollider.EffectAttack;
            CalculatorDamage(attackCollider.DamagePercent, isOpen);
        }

        protected override void CalculatorDamage(int damagePercent, bool isOpen)
        {
            Damage = 0;
            if (!isOpen) return;

            Damage = (EnemyStatusData.Atk * damagePercent) / 100;
        }

        protected override void Dead()
        {
            IsDead = true;
            agent.isStopped = true;
            capsuleCollider.enabled = false;
            canGetDamage = false;
            animator.SetBool(AnimIsDead, true);
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
        }

        public override void ResetGetDamage()
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
        
        public override void ResetColliderAttackAndVFX()
        {
            for (int i = 0; i < damageColliders.Count; i++)
            {
                damageColliders[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < vfxSlash.Count; i++)
            {
                vfxSlash[i].SetActive(false);
            }
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
        
        #region USE_WITH_ANIMATION

        public void OpenDamageCollider(int damageCollider)
        {
            SetDamageCollider(damageCollider, true);
        }
        
        public void CloseDamageCollider(int damageCollider)
        {
            SetDamageCollider(damageCollider, false);
        }

        public void CloseAllDamageCollider()
        {
            for (int i = 0; i < damageColliders.Count; i++)
            {
                damageColliders[i].gameObject.SetActive(false);
            }
        }
        
        public void OpenVfxNormalAttack()
        {
            var spawnPoint = damageColliders.Find
                (x => x.CollidersDamage == GolemCollidersDamage.NormalAttack);
            
            PoolManager.PoolObjectType poolObj = PoolManager.PoolObjectType.ShockWaveEnemyGolem;
            
            var shockWave = PoolManager.Instance.GetPooledObject(poolObj);
            
            if (shockWave)
            {
                var position = spawnPoint.gameObject.transform.position;
                shockWave.transform.position = position;
                shockWave.transform.rotation = Quaternion.identity;
                shockWave.GetComponent<VfxShockWave>().Init(position);
                shockWave.SetActive(true);
            }
        }
        
        public void OpenVfx(int index)
        {
            StartCoroutine(PlayVfx(index));
        }

        private IEnumerator PlayVfx(int index)
        {
            vfxSlash[index].SetActive(true);
            yield return new WaitForSeconds(0.5f);
            vfxSlash[index].SetActive(false);
        }

        #endregion

        public void PlaySfxGoloemSpinAttack()
        {
            GetDataSound(spinAttack, SoundClip.Sound.GoloemSpinAttack);
            spinAttack.Play();
        }

        public void StopSfxGoloemSpinAttack()
        {
            spinAttack.Stop();
        }
        
        public void PlaySfxGolemNormalAttack()
        {
            GetDataSound(normalAttack, SoundClip.Sound.GolemNormalAttack);
            normalAttack.Play();
        }
        
        private void PlaySfxSummon()
        {
            GetDataSound(sfxSummon, SoundClip.Sound.SharkSummon);
            sfxSummon.Play();
        }
        
        public override void StopSfx()
        {
            normalAttack.Stop();
            spinAttack.Stop();
        }
    }
}