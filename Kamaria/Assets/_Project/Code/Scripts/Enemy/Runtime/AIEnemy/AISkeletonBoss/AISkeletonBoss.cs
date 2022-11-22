using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kamaria.Item;
using Kamaria.Player.Data;
using Kamaria.Player.Data.Quest;
using Kamaria.Utilities;
using Kamaria.Utilities.PoolingPattern;
using UnityEngine;
using static System.Decimal;
using RandomUnity = UnityEngine.Random;
using RandomSystem = System.Random;

namespace Kamaria.Enemy.AIEnemy.SkeletonBoss
{
    public enum SkeletonBossAnimation
    {
        movement, isDead, isChargeAttack, normalAttack, pistolAttack, meteorAttack, summon, heal
    }
    
    public sealed class AISkeletonBoss : BaseAI, IDamageable
    {
        [SerializeField] private AudioSource sfxNormalAttack;
        [SerializeField] private AudioSource sfxPistolAttack;
        [SerializeField] private RectTransform uiFinish;
        [SerializeField] private Transform spawnBullet;
        [SerializeField] private float minCooldownSkillPistol;
        [SerializeField] private float maxCooldownSkillPistol;
        [SerializeField] private float minCooldownSkillMeteor;
        [SerializeField] private float maxCooldownSkillMeteor;
        [SerializeField] private List<SkeletonBossDamageCollider> damageColliders;
        [SerializeField] private GameObject indicatorPistolAttack;
        [SerializeField] private List<GameObject> vfxSlash;
        [SerializeField] private List<BaseAI> enemySummon = new List<BaseAI>();
        [SerializeField] private List<Transform> positionSummon = new List<Transform>();
        [SerializeField] private int summonTime;
        [SerializeField] private int percentHpToHeal;
        [SerializeField] private int percentHealFixed;
        [SerializeField] private int percentHealSummonLife;
        [SerializeField] private GameObject vfxHeal;

        private RandomSystem random = new RandomSystem();

        public RectTransform UIFinish => uiFinish;
        public List<BaseAI> EnemySummon => enemySummon;
        public int PercentHealFixed => percentHealFixed;
        public int PercentHealSummonLife => percentHealSummonLife;
        public float MinCooldownSkillPistol => minCooldownSkillPistol;
        public float MaxCooldownSkillPistol => maxCooldownSkillPistol;
        public float MinCooldownSkillMeteor => minCooldownSkillMeteor;
        public float MaxCooldownSkillMeteor => maxCooldownSkillMeteor;

        public List<SkeletonBossDamageCollider> DamageColliders => damageColliders;
        public GameObject IndicatorPistolAttack => indicatorPistolAttack;

        public bool CanPistol { get; set; }
        public bool CanMeteor { get; set; }
        public bool CanSummon { get; set; }
        public bool SummonAgain { get; set; }
        public bool CanHeal { get; set; }
        public bool SummonFinish { get; set; }
        public bool HealFinish { get; set; }

        private SkeletonBossDamageCollider attackCollider;

        #region ANIMATION_PARAMETER

        public int AnimMove { get; } = Animator.StringToHash(SkeletonBossAnimation.movement.ToString());
        public int AnimPistolAttack { get; } = Animator.StringToHash(SkeletonBossAnimation.pistolAttack.ToString());
        public int AnimMeteorAttack { get; } = Animator.StringToHash(SkeletonBossAnimation.meteorAttack.ToString());
        public int AnimHeal { get; } = Animator.StringToHash(SkeletonBossAnimation.heal.ToString());
        public int AnimIsChargeAttack { get; } = Animator.StringToHash(SkeletonBossAnimation.isChargeAttack.ToString());
        public int AnimIsDead { get; } = Animator.StringToHash(SkeletonBossAnimation.isDead.ToString());
        public int AnimNormalAttack { get; } = Animator.StringToHash(SkeletonBossAnimation.normalAttack.ToString());
        public int AnimSummon { get; } = Animator.StringToHash(SkeletonBossAnimation.summon.ToString());

        #endregion
        
        protected override void OnEnable()
        {
            uiFinish.gameObject.SetActive(false);
            GetStatus();
            base.OnEnable();
            Initialize();
            animator.SetBool(AnimIsChargeAttack, false);
            animator.SetBool(AnimIsDead, false);
            animator.ResetTrigger(AnimNormalAttack);
            animator.ResetTrigger(AnimPistolAttack);
            animator.ResetTrigger(AnimMeteorAttack);
            animator.ResetTrigger(AnimHeal);
            animator.ResetTrigger(AnimSummon);

            currentState = patrolState;

            currentState.EnterState();

            CanMeteor = false;
            CanPistol = false;
            CanSummon = false;
            CanHeal = false;
            SummonFinish = false;
            HealFinish = false;
            SummonAgain = true;

            CoolDownPistol();
            CoolDownMeteor();
            StartCoroutine(nameof(SummonTime));
            StartCoroutine(nameof(HealTime));
            vfxShield.SetActive(useGuard);
            if (useGuard)
            {
                SetGuard();
            }
        }
        
        private void OnDisable()
        {
            StopCoroutine(nameof(SummonTime));
            StopCoroutine(nameof(HealTime));

            vfxHeal.SetActive(false);
        }

        public override void Initialize()
        {
            patrolState = new PatrolState(this, statesData);
            alertState = new AlertState(this, statesData);
            chaseState = new ChaseState(this, statesData);
            deadState = new DeadState(this, statesData);
            attackState = new SkeletonBossAttackState(this, statesData);
            summonState = new SummonState(this, statesData);
            healState = new HealState(this, statesData);

            statesData.Add(patrolState);
            statesData.Add(alertState);
            statesData.Add(chaseState);
            statesData.Add(deadState);
            statesData.Add(attackState);
            statesData.Add(summonState);
            statesData.Add(healState);
        }

        private void Update()
        {
            CheckSummonAgain();
            
            currentState.UpdateState();
        }

        protected override void SetDamageCollider(int damageCollider, bool isOpen)
        {
            switch (damageCollider)
            {
                case 0:
                    attackCollider = damageColliders.Find(x => x.CollidersDamage == SkeletonBossCollidersDamage.NormalAttack);
                    break;
                default:
                    Debug.Log("Shark boss have 0");
                    break;
            }
            
            SetDataAttack(isOpen, attackCollider);
        }
        
        private void SetDataAttack(bool isOpen, SkeletonBossDamageCollider attackCollider)
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

        public void PistolAttack()
        {
            PoolManager.PoolObjectType poolObj = PoolManager.PoolObjectType.Pistol;
            
            var bullet = PoolManager.Instance.GetPooledObject(poolObj);
            
            if (bullet)
            {
                bullet.transform.position = spawnBullet.position;
                bullet.transform.rotation = Quaternion.identity;
                bullet.GetComponent<BulletEnemy>().Init(EnemyStatusData.Atk,
                    EffectAttack.None, Vector3.zero, 0, 0, false, 0,
                    spawnBullet.forward);
                bullet.SetActive(true);
            }
        }
        
        public void MeteorAttack()
        {
            PoolManager.PoolObjectType poolObj = PoolManager.PoolObjectType.Meteor;
            
            var meteor = PoolManager.Instance.GetPooledObject(poolObj);
            
            if (meteor)
            {
                meteor.SetActive(true);
                meteor.GetComponent<Meteor>().Init(target.transform.position, EnemyStatusData.Atk,
                    EffectAttack.None, Vector3.zero, 0, 0, false, 0);
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
                UpdateQuestLast();
                Dead();
                return;
            }
           
            TakeDamageBehavior(effectAttack, explosionPos, powerKnockback, radiusKnockback, isAOE, stunTime);
        }

        public void CoolDownPistol()
        {
            StartCoroutine(nameof(PistolAttackCooldown));
        }
        
        public void CoolDownMeteor()
        {
            StartCoroutine(nameof(MeteorAttackCooldown));
        }
        
        private IEnumerator PistolAttackCooldown()
        {
            CanPistol = false;
            yield return new WaitForSeconds(RandomCooldownSkill(minCooldownSkillPistol, maxCooldownSkillPistol));
            CanPistol = true;
        }
        
        private IEnumerator MeteorAttackCooldown()
        {
            CanMeteor = false;
            yield return new WaitForSeconds(RandomCooldownSkill(minCooldownSkillMeteor, maxCooldownSkillMeteor));
            CanMeteor = true;
        }

        private float RandomCooldownSkill(float min, float max)
        {
            decimal tempCooldownSkill = (decimal)RandomUnity.Range(min, max);
            return (float)Round(tempCooldownSkill, 2);
        }

        public void Summon()
        {
            for (int i = 0; i < enemySummon.Count; i++)
            {
                enemySummon[i].transform.position = positionSummon[i].position;
                enemySummon[i].gameObject.SetActive(true);
            }
        }
        
        private IEnumerator SummonTime()
        {
            while (true)
            {
                yield return new WaitUntil(() => SummonAgain);
                Debug.Log("Wait Summon");
                yield return new WaitForSeconds(summonTime);
                CanSummon = true;
                SummonAgain = false;
            }
        }

        private void CheckSummonAgain()
        {
            int countEnemyDead = 0;
            
            if (CanSummon)
            {
                SummonAgain = false;
                return;
            }
            
            var enemyDead = enemySummon.Where(x => x.IsDead);

            foreach (BaseAI enemy in enemyDead)
            {
                countEnemyDead++;
            }
            Debug.Log($"WWW {countEnemyDead}");
            SummonAgain = countEnemyDead == enemySummon.Count;
        }

        public int CountSummonLife()
        {
            int countEnemyLife = 0;
            var enemyLife = enemySummon.Where(x => !x.IsDead);

            foreach (BaseAI enemy in enemyLife)
            {
                countEnemyLife++;
            }

            return countEnemyLife;
        }

        private IEnumerator HealTime()
        {
            yield return new WaitUntil(() =>
                EnemyStatusData.CurrentHealth <= (EnemyStatusData.MaxHealth * percentHpToHeal) / 100);
            Debug.Log("CAN HEAL");
            CanHeal = true;
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
        
        public void OpenVfxHeal()
        {
            vfxHeal.SetActive(true);
        }

        private IEnumerator PlayVfx(int index)
        {
            vfxSlash[index].SetActive(true);
            yield return new WaitForSeconds(0.5f);
            vfxSlash[index].SetActive(false);
        }

        #endregion

        #region QUEST

        private void UpdateQuestLast()
        {
            if (questManagerSO.CurrentQuests is not { NameQuest: NameQuest.KingOfTheSea }) return;

            if (!questManagerSO.CanDoQuest(NameQuest.KingOfTheSea) || questManagerSO.CurrentQuests.IsSucceed) return;

            questManagerSO.UpdateProgressQuest(QuestRequirement.DefeatBossSkeletonPirate, out bool finish);

            questManagerSO.CurrentQuests.UpdateProgress();
        }

        #endregion

        public override void StopSfx()
        {
            sfxNormalAttack.Stop();
        }

        public void PlaySfxNormalAttack()
        {
            GetDataSound(sfxNormalAttack, SoundClip.Sound.SkeletonNormalAttack);
            sfxNormalAttack.Play();
        }
        
        public void PlaySfxPistolAttack()
        {
            GetDataSound(sfxPistolAttack, SoundClip.Sound.SkeletonPistolAttack);
            sfxPistolAttack.Play();
        }
    }
}