using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Kamaria.Item;
using Kamaria.Player.Data;
using Kamaria.Player.Data.Quest;
using Kamaria.Utilities;
using UnityEngine;
using static System.Decimal;
using RandomUnity = UnityEngine.Random;
using RandomSystem = System.Random;

namespace Kamaria.Enemy.AIEnemy.SharkBoss
{
    public enum SharkBossAnimation
    {
        movement, isDead, isChargeAttack, isRushAttack, isAttackSlam, isJump,
        normalAttack, slamAttack, rushAttack, summon
    }
    
    public sealed class AISharkBoss : BaseAI, IDamageable
    {
        [SerializeField] private AudioSource sharkNormalAttackStart;
        [SerializeField] private AudioSource sharkNormalAttackEnd;
        [SerializeField] private AudioSource sharkDash;
        [SerializeField] private RectTransform uiFinish;
        [SerializeField] private float minCooldownSkillSlam;
        [SerializeField] private float maxCooldownSkillSlam;
        [SerializeField] private float minCooldownSkillRush;
        [SerializeField] private float maxCooldownSkillRush;
        [SerializeField] private float slamTime;
        [SerializeField] private float rushTime;
        [SerializeField] private float moveRushAttackAcceleration;
        [SerializeField] private List<SharkBossDamageCollider> damageColliders;
        [SerializeField] private GameObject indicatorSlamAttack;
        [SerializeField] private GameObject indicatorRushAttack;
        [SerializeField] private GameObject vfxSlamAttack;
        [SerializeField] private List<GameObject> vfxSlash;
        [SerializeField] private List<BaseAI> enemySummon = new List<BaseAI>();
        [SerializeField] private List<Transform> positionSummon = new List<Transform>();
        [SerializeField] private int percentSummon;
        [SerializeField] private int canSummonCount;
        [SerializeField] private float moveY;
        [SerializeField] private float durationMoveUp;
        [SerializeField] private float moveSpeedSlam;
        [SerializeField] private GameObject vfxRushAttack;
        [SerializeField] private HideShadow[] shadows;

        private int countSummon;
        private RandomSystem random = new RandomSystem();

        public RectTransform UIFinish => uiFinish;
        public GameObject VfxRushAttack => vfxRushAttack;
        public float SlamTime => slamTime;
        public float RushTime => rushTime;
        public float MoveSpeedSlam => moveSpeedSlam;
        public float MinCooldownSkillSlam => minCooldownSkillSlam;
        public float MaxCooldownSkillSlam => maxCooldownSkillSlam;
        public float MinCooldownSkillRush => minCooldownSkillRush;
        public float MaxCooldownSkillRush => maxCooldownSkillRush;
        public float MoveRushAttackAcceleration => moveRushAttackAcceleration;
        public List<BaseAI> EnemySummon => enemySummon;
        public List<SharkBossDamageCollider> DamageColliders => damageColliders;
        public GameObject IndicatorSlamAttack => indicatorSlamAttack;
        public GameObject IndicatorRushAttack => indicatorRushAttack;
        public GameObject VfxSlamAttack => vfxSlamAttack;

        public SharkBossDamageCollider SlamCollider =>
            damageColliders.Find(x => x.CollidersDamage == SharkBossCollidersDamage.SlamAttack);
        
        public bool CanSlam { get; set; }
        public bool CanRush { get; set; }
        public bool EnemyClere { get; set; }

        private SharkBossDamageCollider attackCollider;

        #region ANIMATION_PARAMETER

        public int AnimMove { get; } = Animator.StringToHash(SharkBossAnimation.movement.ToString());
        public int AnimIsRushAttack { get; } = Animator.StringToHash(SharkBossAnimation.isRushAttack.ToString());
        public int AnimIsAttackSlam { get; } = Animator.StringToHash(SharkBossAnimation.isAttackSlam.ToString());
        public int AnimIsJump { get; } = Animator.StringToHash(SharkBossAnimation.isJump.ToString());
        public int AnimIsChargeAttack { get; } = Animator.StringToHash(SharkBossAnimation.isChargeAttack.ToString());
        public int AnimIsDead { get; } = Animator.StringToHash(SharkBossAnimation.isDead.ToString());
        public int AnimNormalAttack { get; } = Animator.StringToHash(SharkBossAnimation.normalAttack.ToString());
        public int AnimSlamAttack { get; } = Animator.StringToHash(SharkBossAnimation.slamAttack.ToString());
        public int AnimRushAttack { get; } = Animator.StringToHash(SharkBossAnimation.rushAttack.ToString());
        public int AnimSummon { get; } = Animator.StringToHash(SharkBossAnimation.summon.ToString());

        #endregion
        
        protected override void OnEnable()
        {
            uiFinish.gameObject.SetActive(false);
            GetStatus();
            base.OnEnable();
            Initialize();
            animator.SetBool(AnimIsRushAttack, false);
            animator.SetBool(AnimIsAttackSlam, false);
            animator.SetBool(AnimIsJump, false);
            animator.SetBool(AnimIsChargeAttack, false);
            animator.SetBool(AnimIsDead, false);
            animator.ResetTrigger(AnimNormalAttack);
            animator.ResetTrigger(AnimSlamAttack);
            animator.ResetTrigger(AnimRushAttack);
            animator.ResetTrigger(AnimSummon);

            currentState = patrolState;

            currentState.EnterState();

            CanRush = false;
            CanSlam = false;
            countSummon = canSummonCount;

            CoolDownSlam();
            CoolDownRush();
            vfxShield.SetActive(useGuard);
            if (useGuard)
            {
                SetGuard();
            }
        }
        
        public override void Initialize()
        {
            patrolState = new PatrolState(this, statesData);
            alertState = new AlertState(this, statesData);
            chaseState = new ChaseState(this, statesData);
            deadState = new DeadState(this, statesData);
            attackState = new SharkBossAttackState(this, statesData);
            summonState = new SummonState(this, statesData);

            statesData.Add(patrolState);
            statesData.Add(alertState);
            statesData.Add(chaseState);
            statesData.Add(deadState);
            statesData.Add(attackState);
            statesData.Add(summonState);
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
                    attackCollider = damageColliders.Find(x => x.CollidersDamage == SharkBossCollidersDamage.NormalAttack1);
                    break;
                case 1:
                    attackCollider = damageColliders.Find(x => x.CollidersDamage == SharkBossCollidersDamage.NormalAttack2);
                    break;
                case 2:
                    attackCollider = damageColliders.Find(x => x.CollidersDamage == SharkBossCollidersDamage.SlamAttack);
                    break;
                case 3:
                    attackCollider = damageColliders.Find(x => x.CollidersDamage == SharkBossCollidersDamage.SlamEndAttack);
                    break;
                default:
                    Debug.Log("Shark boss have 0-3");
                    break;
            }
            
            SetDataAttack(isOpen, attackCollider);
        }

        public void SetColliderRush(bool isOpen)
        {
            SetDataAttack(isOpen, damageColliders.Find
                (x => x.CollidersDamage == SharkBossCollidersDamage.RushAttack));
        }
        
        private void SetDataAttack(bool isOpen, SharkBossDamageCollider attackCollider)
        {
            attackCollider.gameObject.SetActive(isOpen);
            
            if (attackCollider.CollidersDamage == SharkBossCollidersDamage.SlamAttack)
            {
                EffectAttack = attackCollider.CurrentStun >= attackCollider.CountStun 
                    ? EffectAttack.None : attackCollider.EffectAttack;
            }
            else
            {
                EffectAttack = attackCollider.EffectAttack;
            }
            
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
            
            if (agent.enabled)
            {
                agent.isStopped = true;
            }
            
            capsuleCollider.enabled = false;
            canGetDamage = false;
            animator.SetBool(AnimIsDead, true);
            StopCoroutine(nameof(GuardCharging));
            vfxShield.SetActive(false);
        }

        protected override void TakeDamageBehavior(EffectAttack effectAttack, Vector3 explosionPos, float powerKnockback, float radiusKnockback,
            bool isAOE, float stunTime)
        {
            if (IsFirstDefinitelyAttack)
            {
                IsFirstDefinitelyAttack = false;
                DefinitelyAttack();
            }
        }

        public override void ResetGetDamage()
        {
        }

        public override void DropItem()
        {
            for (int i = 0; i < dropItemHandler.CountDropItem; i++)
            {
                //if (!dropItemHandler.CanDrop()) continue; // If need random can drop.
                if (i == dropItemHandler.CountDropItem - 1)
                {
                    createItemDropManager.CreateItemDrop(random.Next(2) == 0 
                            ? ItemsName.Gold : ItemsName.Diamond
                        , transform);
                    
                    break;
                }
            
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

            vfxSlamAttack.SetActive(false);
            vfxRushAttack.SetActive(false);
            
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
                if (HaveGuard)
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
                            IsBreakGuard = true;
                            HaveGuard = false;
                            vfxShield.SetActive(false);
                        }
                    }
                }

                if (EnemyStatusData.CurrentGuard > 0)
                {
                    damageHp = (int)(damage - ((damage * percentDecreaseDamageHaveGuard) / 100));
                    Debug.Log($"Have {damageHp}");
                }
            
                if (IsBreakGuard)
                {
                    IsBreakGuard = false;
                    StartCoroutine(nameof(GuardCharging));
                }
            }
            
            Debug.Log($"GET Damage by {weaponTypes}");
            
            if (currentState.State == AIState.Patrol)
            {
                NextState(statesData.Find(x => x.State == AIState.Chase));
            }
            
            EnemyStatusData.CurrentHealth -= damageHp;
            blinkEffect.Blink();
            healthBar.ShowBar(EnemyStatusData.CurrentHealth, EnemyStatusData.CurrentGuard);
            ShowTextDamage(damageHp);
            Debug.Log($"AI GetDamage = {damageHp}");
            Debug.Log(EnemyStatusData.CurrentHealth);
            if (EnemyStatusData.CurrentHealth <= 0)
            {
                UpdateQuest6And8();
                Dead();
                return;
            }
           
            TakeDamageBehavior(effectAttack, explosionPos, powerKnockback, radiusKnockback, isAOE, stunTime);
        }

        public void CoolDownSlam()
        {
            StartCoroutine(nameof(SlamAttackCooldown));
        }
        
        public void CoolDownRush()
        {
            StartCoroutine(nameof(RushAttackCooldown));
        }
        
        private IEnumerator SlamAttackCooldown()
        {
            CanSlam = false;
            yield return new WaitForSeconds(RandomCooldownSkill(minCooldownSkillSlam, maxCooldownSkillSlam));
            CanSlam = true;
        }
        
        private IEnumerator RushAttackCooldown()
        {
            CanRush = false;
            yield return new WaitForSeconds(RandomCooldownSkill(minCooldownSkillRush, maxCooldownSkillRush));
            CanRush = true;
        }

        private float RandomCooldownSkill(float min, float max)
        {
            decimal tempCooldownSkill = (decimal)RandomUnity.Range(min, max);
            return (float)Round(tempCooldownSkill, 2);
        }

        public void CheckEnemySummonClere()
        {
            int countEnemyDead = 0;
            var enemyDead = enemySummon.Where(x => x.IsDead);

            foreach (BaseAI enemy in enemyDead)
            {
                countEnemyDead++;
            }

            EnemyClere = countEnemyDead == enemySummon.Count;
        }

        public bool CheckSummon()
        {
            int currentHpPercent = (EnemyStatusData.MaxHealth * percentSummon) / 100;

            return EnemyStatusData.CurrentHealth <= currentHpPercent && countSummon > 0;
        }

        public void Summon()
        {
            for (int i = 0; i < enemySummon.Count; i++)
            {
                enemySummon[i].transform.position = positionSummon[i].position;
                enemySummon[i].gameObject.SetActive(true);
            }

            countSummon--;
        }

        public void MoveUp()
        {
            this.transform.DOMoveY(transform.position.y + moveY, durationMoveUp).SetEase(Ease.OutQuint);
            for (int i = 0; i < shadows.Length; i++)
            {
                shadows[i].Hide();
            }
        }

        public void MoveDown()
        {
            for (int i = 0; i < shadows.Length; i++)
            {
                shadows[i].Show();
            }
            this.transform.DOMoveY(transform.position.y - moveY, durationMoveUp).SetEase(Ease.OutQuint);
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
        
        public void OpenVfxSlash(int index)
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

        #region QUEST

        private void UpdateQuest6And8()
        {
            if (questManagerSO.CurrentQuests == null) return;

            if (questManagerSO.CurrentQuests.NameQuest == NameQuest.DefeatTheBossSharkPirate)
            {
                if (!questManagerSO.CanDoQuest(NameQuest.DefeatTheBossSharkPirate) 
                    || questManagerSO.CurrentQuests.IsSucceed) return;

                questManagerSO.UpdateProgressQuest(QuestRequirement.DefeatBossSharkPirate1, out bool finish);
            }
            else if (questManagerSO.CurrentQuests.NameQuest == NameQuest.NoPainNoGain)
            {
                if (!questManagerSO.CanDoQuest(NameQuest.NoPainNoGain) 
                    || questManagerSO.CurrentQuests.IsSucceed) return;
                
                questManagerSO.UpdateProgressQuest(QuestRequirement.DefeatBossSharkPirate2, out bool finish);
            }
            else
            {
                return;
            }

            questManagerSO.CurrentQuests.UpdateProgress();
        }

        #endregion

        public override void StopSfx()
        { 
            sharkNormalAttackStart.Stop(); 
            sharkNormalAttackEnd.Stop(); 
            sharkDash.Stop();
        }

        public void PlaySfxNormalAttack(int combo)
        {
            switch (combo)
            {
                case 1:
                    GetDataSound(sharkNormalAttackStart, SoundClip.Sound.SharkNormalAttackStart);
                    sharkNormalAttackStart.Play();
                    break;
                case 2:
                    GetDataSound(sharkNormalAttackEnd, SoundClip.Sound.SharkNormalAttackEnd);
                    sharkNormalAttackEnd.Play();
                    break;
            }
        }

        public void PlaySfxDash()
        {
            GetDataSound(sharkDash, SoundClip.Sound.SharkDash);
            sharkDash.Play();
        }

        public void PlaySfxSpin()
        {
            StartCoroutine(nameof(SfxSpin));
        }
        
        public void StopSfxSpin()
        {
            StopCoroutine(nameof(SfxSpin));
        }

        private IEnumerator SfxSpin()
        {
            while (true)
            {
                yield return new WaitForSeconds(0.23f);
                GetDataSound(sharkDash, SoundClip.Sound.SharkDash);
                GetDataSound(sharkNormalAttackEnd, SoundClip.Sound.SharkNormalAttackEnd);
                sharkDash.PlayOneShot(sharkDash.clip, sharkDash.volume + 0.5f);
                sharkDash.PlayOneShot(sharkNormalAttackEnd.clip, sharkDash.volume + 0.5f);
            }
        }
    }
}