using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Kamaria.Item;
using Kamaria.Player.Data;
using Kamaria.Utilities;
using Kamaria.Utilities.PoolingPattern;
using Kamaria.VFX_ALL;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Kamaria.Enemy.AIEnemy.Mouse
{
    public enum MouseAnimation
    {
        movement,
        alert,
        normalAttack, spinAttack, continuousAttack, jumpAttack1, jumpAttack2, isChargeAttack,
        isDead, getDamage, isStun
    }
    
    public sealed class AIMouse : BaseAI, IDamageable
    {
        [SerializeField] private AudioSource sfxMouseNormalAttack;
        [SerializeField] private AudioSource sfxMouseCombat1;
        [SerializeField] private AudioSource sfxMouseCombat2;
        [SerializeField] private AudioSource sfxSummon;
        [SerializeField] private MoveAttackData[] moveNormalAttack;
        [SerializeField] private MoveAttackData[] moveSpineAttack;
        [SerializeField] private MoveAttackData[] moveContinuousAttack;
        [SerializeField] private MoveAttackData[] moveJumpAttack1;
        [SerializeField] private MoveAttackData[] moveJumpAttack2;

        [SerializeField] private List<MouseDamageCollider> damageColliders;
        [SerializeField] private GameObject vfxSlashNormalAttack;
        [SerializeField] private GameObject vfxSlashSpinAttack;

        public MoveAttackData[] MoveNormalAttack => moveNormalAttack;
        public MoveAttackData[] MoveSpineAttack => moveSpineAttack;
        public MoveAttackData[] MoveContinuousAttack => moveContinuousAttack;
        public MoveAttackData[] MoveJumpAttack1 => moveJumpAttack1;
        public MoveAttackData[] MoveJumpAttack2 => moveJumpAttack2;
        public List<MouseDamageCollider> DamageColliders => damageColliders;

        private MouseDamageCollider attackCollider;

        #region ANIMATION_PARAMETER

        public int AnimMove { get; } = Animator.StringToHash(MouseAnimation.movement.ToString());
        public int AnimAlert { get; } = Animator.StringToHash(MouseAnimation.alert.ToString());
        public int AnimNormalAttack { get; } = Animator.StringToHash(MouseAnimation.normalAttack.ToString());
        public int AnimSpinAttack { get; } = Animator.StringToHash(MouseAnimation.spinAttack.ToString());
        public int AnimContinuousAttack { get; } = Animator.StringToHash(MouseAnimation.continuousAttack.ToString());
        public int AnimJumpAttack1 { get; } = Animator.StringToHash(MouseAnimation.jumpAttack1.ToString());
        public int AnimJumpAttack2 { get; } = Animator.StringToHash(MouseAnimation.jumpAttack2.ToString());
        public int AnimIsChargeAttack { get; } = Animator.StringToHash(MouseAnimation.isChargeAttack.ToString());
        public int AnimIsDead { get; } = Animator.StringToHash(MouseAnimation.isDead.ToString());
        public int AnimGetDamage { get; } = Animator.StringToHash(MouseAnimation.getDamage.ToString());
        public int AnimIsStun { get; } = Animator.StringToHash(MouseAnimation.isStun.ToString());

        #endregion

        protected override void OnEnable()
        {
            GetStatus();
            base.OnEnable();
            Initialize();
            animator.SetBool(AnimIsDead, false);
            animator.SetBool(AnimIsStun, false);
            animator.SetBool(AnimIsChargeAttack, false);
            animator.ResetTrigger(AnimAlert);
            animator.ResetTrigger(AnimNormalAttack);
            animator.ResetTrigger(AnimSpinAttack);
            animator.ResetTrigger(AnimContinuousAttack);
            animator.ResetTrigger(AnimJumpAttack1);
            animator.ResetTrigger(AnimJumpAttack2);
            animator.ResetTrigger(AnimGetDamage);
            
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
            attackState = new MouseAttackState(this, statesData);

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
        
        protected override void SetDamageCollider(int damageCollider, bool isOpen)
        {
            switch (damageCollider)
            {
                case 0:
                    attackCollider = damageColliders.Find(x => x.CollidersDamage == MouseCollidersDamage.NormalAttack);
                    break;
                case 1:
                    attackCollider = damageColliders.Find(x => x.CollidersDamage == MouseCollidersDamage.SpineAttack);
                    break;
                case 2:
                    attackCollider = damageColliders.Find(x => x.CollidersDamage == MouseCollidersDamage.JumpAttack);
                    break;
                default:
                    Debug.Log("Mouse have 0-2");
                    break;
            }
            
            SetDataAttack(isOpen, attackCollider);
        }

        private void SetDataAttack(bool isOpen, MouseDamageCollider attackCollider)
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

        public override void ResetGetDamage()
        {
            animator.ResetTrigger(AnimGetDamage);
            controllerRb.isKinematic = true;
            agent.isStopped = false;
        }

        public override void ResetColliderAttackAndVFX()
        {
            for (int i = 0; i < damageColliders.Count; i++)
            {
                damageColliders[i].gameObject.SetActive(false);
            }
            
            CloseVfxSlashNormalAttack();
            CloseVfxSlashSpinAttack();
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

        protected override void TakeDamageBehavior(EffectAttack effectAttack, Vector3 explosionPos, float powerKnockback,
            float radiusKnockback, bool isAOE, float stunTime)
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

            if (!IsStun)
            {
                animator.SetTrigger(AnimGetDamage);
            }
                
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
        
        public void PlaySoundMouseNormalAttack()
        {
            GetDataSound(sfxMouseNormalAttack, SoundClip.Sound.MouseNormalAttack);
            sfxMouseNormalAttack.Play();
        }

        public void PlaySoundMoseCombat1()
        {
            GetDataSound(sfxMouseCombat1, SoundClip.Sound.MouseCombat1);
            sfxMouseCombat1.Play();
        }
        
        public void PlaySoundMoseCombat2()
        {
            GetDataSound(sfxMouseCombat2, SoundClip.Sound.MouseCombat2);
            sfxMouseCombat2.Play();
        }

        public override void StopSfx()
        {
            sfxMouseCombat1.Stop();
            sfxMouseCombat2.Stop();
        }

        private void PlaySfxSummon()
        {
            GetDataSound(sfxSummon, SoundClip.Sound.SharkSummon);
            sfxSummon.Play();
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

        public void OpenVfxSlashSpin()
        {
            vfxSlashSpinAttack.SetActive(true);
            Invoke(nameof(CloseVfxSlashSpinAttack), 0.5f);
        }

        public void OpenVfxJumpAttack()
        {
            var spawnPoint = damageColliders.Find(x => x.CollidersDamage == MouseCollidersDamage.JumpAttack);
            
            PoolManager.PoolObjectType poolObj = PoolManager.PoolObjectType.ShockWaveEnemyMouse;
            
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
        
        public void OpenVfxSlashNormal()
        {
            vfxSlashNormalAttack.SetActive(true);
            Invoke(nameof(CloseVfxSlashNormalAttack), 0.5f);
        }

        private void CloseVfxSlashSpinAttack()
        {
            vfxSlashSpinAttack.SetActive(false);
        }
        
        private void CloseVfxSlashNormalAttack()
        {
            vfxSlashNormalAttack.SetActive(false);
        }

        #endregion
    }
}