using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Kamaria.DropItem;
using Kamaria.Item;
using Kamaria.Manager;
using Kamaria.Player.Data;
using Kamaria.Player.Data.Quest;
using Kamaria.UIDamage;
using Kamaria.Utilities;
using Kamaria.Utilities.PoolingPattern;
using UnityEngine;
using UnityEngine.AI;
using Random = System.Random;

namespace Kamaria.Enemy.AIEnemy
{
    public enum Enemies
    {
        Mouse, Fish, Golem,
        SharkBoss, SkeletonBoss
    }

    public enum EnemyAttackTypes
    {
        Normal,
        CanCharge,
        SpinGolem
    }
    
    public enum EnemiesLevel
    {
        Lv1, Lv2, Lv3
    }
    
    [RequireComponent(typeof(NavMeshAgent))]
    public abstract class BaseAI : MonoBehaviour
    {
        [SerializeField] protected QuestManagerSO questManagerSO;
        [SerializeField] protected GameObject spAmIcon;
        [SerializeField] protected bool useVfxSummon;
        [SerializeField] protected GameObject vfxSummon;
        [SerializeField] protected bool useGuard;
        [SerializeField] protected WeaponTypes weakForWeapon;
        [SerializeField] protected float guardChargingTime;
        [SerializeField] protected bool useEvolution;
        [SerializeField] protected int getDamageGuard;
        [SerializeField] protected int getDamagePowerGuard;
        [SerializeField] protected int percentDecreaseDamageHaveGuard;
        [SerializeField] protected GameObject vfxShield;
        [SerializeField] protected GameObject vfxStun;
        [SerializeField] protected CreateItemDropManager createItemDropManager;
        [SerializeField] protected DropItemHandler dropItemHandler;
        [SerializeField] protected FarmingManagerSO farmingManager;
        [SerializeField] protected HealthBar healthBar;
        [SerializeField] protected BlinkEffect blinkEffect;
        [SerializeField] protected EnemyStatusDataSO enemyStatusDataSO;
        [SerializeField] protected PlayerDataSO playerData;
        [SerializeField] protected GameObject target;
        [SerializeField] protected Animator animator;
        [SerializeField] protected Enemies enemyType;
        [SerializeField] protected EnemiesLevel enemyLevel;
        [SerializeField] protected NavMeshAgent agent;
        [SerializeField] protected AIWaypointHandled waypointHandled;
        [SerializeField] protected CapsuleCollider capsuleCollider;
        [SerializeField] protected Rigidbody controllerRb;
        [SerializeField] protected float alertDuration = 1.15f;
        [SerializeField] protected float rotationAlertDuration = 0.92f;
        [SerializeField] protected float rotationAttackDuration = 0.5f;
        [SerializeField] protected float patrolNextPointDuration = 2;
        [SerializeField] protected float walkAnimDumpTime = 0.2f;
        [SerializeField] protected float idleAnimDumpTime = 0.2f;
        [SerializeField] protected float escapeTime = 2f;
        [SerializeField] protected float canAlertDuration = 5f;
        [SerializeField] protected float canAttackDuration = 1f;
        [SerializeField] protected float minChargeAttackDuration = 1f;
        [SerializeField] protected float maxChargeAttackDuration = 2.5f;
        [SerializeField] protected float definitelyAttackTime = 5f;
        [SerializeField] protected float knockBackTime = 0.15f;
        [SerializeField] protected float rotationToTargetAttack = 0.08f;
        [SerializeField] protected float rotationToTargetGetDamage = 0.2f;
        [SerializeField] private AIAreaAlert areaAlert;
        [SerializeField] private float stoppingDistance;

        protected BaseAIFiniteStateMachine currentState;
        protected BaseAIFiniteStateMachine patrolState;
        protected BaseAIFiniteStateMachine alertState;
        protected BaseAIFiniteStateMachine chaseState;
        protected BaseAIFiniteStateMachine attackState;
        protected BaseAIFiniteStateMachine escapState;
        protected BaseAIFiniteStateMachine deadState;
        protected BaseAIFiniteStateMachine stunState;
        protected BaseAIFiniteStateMachine summonState;
        protected BaseAIFiniteStateMachine healState;
        protected List<BaseAIFiniteStateMachine> statesData = new List<BaseAIFiniteStateMachine>();
        protected bool canGetDamage;
        
        private Sequence rotationToTarget;
        private EnemyStatusData enemyStatusData = new EnemyStatusData();
        private MoveAttackData[] moveAttackData;
        private int currentCombo;
        private int currentShield;
        private float moveAttackTime;
        private float moveAttackAcceleration;
        private Random random = new Random();
        
        #region PROPERTIES

        public GameObject SpAmIcon => spAmIcon;
        public bool IsBreakGuard { get; set; }
        public bool HaveGuard { get; set; }
        public GameObject VfxShield => vfxShield;
        public GameObject VfxStun => vfxStun;
        public HealthBar HealthBar => healthBar;
        public BlinkEffect BlinkEffect => blinkEffect;
        public Rigidbody ControllerRb => controllerRb;
        public EnemyStatusData EnemyStatusData => enemyStatusData;
        public NavMeshAgent Agent => agent;
        public AIWaypointHandled WaypointHandled => waypointHandled;
        public Enemies EnemyType => enemyType;
        public EnemiesLevel EnemyLevel => enemyLevel;
        public Animator Animator => animator;
        public GameObject Target => target;
        public PlayerDataSO PlayerData => playerData;
        public float AlertDuration => alertDuration;
        public float RotationAlertDuration => rotationAlertDuration;
        public float RotationAttackDuration => rotationAttackDuration;
        public float PatrolNextPointDuration => patrolNextPointDuration;
        public float WalkAnimDumpTime => walkAnimDumpTime;
        public float IdleAnimDumpTime => idleAnimDumpTime;
        public float EscapeTime => escapeTime;
        public float CanAlertDuration => canAlertDuration;
        public float CanAttackDuration => canAttackDuration;
        public float MinChargeAttackDuration => minChargeAttackDuration;
        public float MaxChargeAttackDuration => maxChargeAttackDuration;
        public AIAreaAlert AreaAlert => areaAlert;
        
        public bool IsAnimAlertFinish { get; set; } = false;
        public bool IsResetState { get; set; } = false;
        public bool IsCanAlertState { get; set; } = false;
        public bool CanAlert { get; set; } = true;
        public bool IsAttackFinish { get; set; } = true;
        public bool IsAttackState { get; set; } = false;
        public bool IsAttackNear { get; set; } = false;
        public bool CanAttack { get; set; } = true;
        public bool IsEscapeState { get; set; } = false;
        public bool IsDefinitelyAttack { get; set; } = false;
        public bool IsFirstDefinitelyAttack { get; set; } = true;
        public bool IsDead { get; set; } = false;
        public bool IsStun { get; set; } = false;
        public int Damage { get; set; } = 0;
        public EffectAttack EffectAttack { get; set; }
        public float StunTime { get; protected set; }

        #endregion

        /// <summary>
        /// Do the initialize process.
        /// Get data or setting.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Execute the state change process.
        /// </summary>
        /// <param name="nextState"> Next state proceed </param>
        internal void NextState(BaseAIFiniteStateMachine nextState)
        {
            currentState.ExitState();
            currentState = nextState;
            currentState.EnterState();
        }

        protected virtual void OnEnable()
        {
            agent.stoppingDistance = stoppingDistance;
            statesData.Clear();
            capsuleCollider.enabled = true;
            capsuleCollider.isTrigger = false;
            IsAnimAlertFinish = false;
            IsResetState = false;
            IsCanAlertState = false;
            IsAttackState = false;
            IsAttackNear = false;
            IsEscapeState = false;
            CanAlert = true;
            CanAttack = true;
            IsAttackFinish = true;
            IsDefinitelyAttack = false;
            IsFirstDefinitelyAttack = true;
            IsDead = false;
            IsStun = false;
            canGetDamage = true;
            Damage = 0;
            currentShield = 0;
            IsBreakGuard = false;
            HaveGuard = true;
            if (useVfxSummon)
            {
                vfxSummon.SetActive(true);
            }

            spAmIcon.SetActive(false);
        }

        /*protected virtual void OnCollisionEnter()
        {
            if (playerData.CharacterControllerData.Dashing)
            {
                capsuleCollider.isTrigger = true;
            }
        }
        
        protected virtual void OnCollisionStay()
        {
            if (playerData.CharacterControllerData.Dashing)
            {
                capsuleCollider.isTrigger = true;
            }
        }
        
        protected virtual void OnCollisionExit()
        {
            capsuleCollider.isTrigger = false;
        }*/

        protected abstract void SetDamageCollider(int damageCollider, bool isOpen);
        protected abstract void CalculatorDamage(int damagePercent, bool isOpen);
        protected abstract void Dead();

        public void DefinitelyAttack()
        {
            StopCoroutine(nameof(DefinitelyAttackTime));
            StartCoroutine(nameof(DefinitelyAttackTime));
        }
        
        public void SetGuard()
        {
            weakForWeapon = random.Next(3) switch
            {
                0 => WeaponTypes.Gun,
                1 => WeaponTypes.Sword,
                2 => WeaponTypes.Knuckle,
                _ => weakForWeapon
            };

            healthBar.SetIconWeakWeapon(weakForWeapon);
        }

        public IEnumerator GuardCharging()
        {
            yield return new WaitForSeconds(guardChargingTime);
            SetGuard();
            enemyStatusData.CurrentGuard = enemyStatusData.MaxGuard;
            healthBar.ShowBar(enemyStatusData.CurrentHealth, enemyStatusData.CurrentGuard);
            vfxShield.SetActive(true);
            HaveGuard = true;
        }
        
        public IEnumerator DefinitelyAttackTime()
        {
            yield return new WaitForSeconds(definitelyAttackTime);
            IsDefinitelyAttack = true;
            
            if (enemyType is Enemies.Mouse or Enemies.Fish)
            {
                spAmIcon.SetActive(true);
            }
        }
        
        protected void GetStatus()
        {
            if (useEvolution)
            {
                if (enemyType is not Enemies.SharkBoss or Enemies.SkeletonBoss)
                {
                    enemyLevel = farmingManager.CurrentFloor switch
                    {
                        <= 3 => EnemiesLevel.Lv1,
                        <= 6 => EnemiesLevel.Lv2,
                        _ => EnemiesLevel.Lv3
                    };
                }
            }

            enemyStatusData.GetStatus(enemyStatusDataSO.StatusLevel[IndexLevel(enemyLevel)]);

            if (useEvolution)
            {
                if (enemyType is not Enemies.SharkBoss or Enemies.SkeletonBoss)
                {
                    if (farmingManager.CanIncrease())
                    {
                        for (int i = 0; i < farmingManager.CountIncrease; i++)
                        {
                            enemyStatusData.Increase();
                        }
                    }
                }
            }

            #region DEBUG

            Debug.Log(enemyStatusData.MaxHealth);
            Debug.Log(enemyStatusData.MaxGuard);
            Debug.Log(enemyStatusData.Atk);

            #endregion

            healthBar.Init(enemyStatusData.MaxHealth, enemyStatusData.MaxGuard, useGuard);
        }

        public abstract void DropItem();

        protected abstract void TakeDamageBehavior(EffectAttack effectAttack, Vector3 explosionPos,
            float powerKnockback, float radiusKnockback, bool isAOE, float stunTime);

        public int IndexLevel(EnemiesLevel enemyLevel)
        {
            int level = enemyLevel switch
            {
                EnemiesLevel.Lv1 => 0,
                EnemiesLevel.Lv2 => 1,
                EnemiesLevel.Lv3 => 2,
                _ => 0
            };

            return level;
        }

        public abstract void ResetGetDamage();

        public abstract void ResetColliderAttackAndVFX();

        public void SetAnimAlertFinish(bool isFinish)
        {
            IsAnimAlertFinish = isFinish;
        }
        
        public void SetAttackFinish(bool isFinish)
        {
            IsAttackFinish = isFinish;
        }
        
        protected void RotationToTarget(bool isAttack)
        {
            float duration = isAttack ? rotationToTargetAttack : rotationToTargetGetDamage;
            
            Quaternion rotation =
                Quaternion.LookRotation(new Vector3(Target.transform.position.x, 0,
                    Target.transform.position.z) - new Vector3(gameObject.transform.position.x, 0,
                    gameObject.transform.position.z), Vector3.up);
            
            rotationToTarget = DOTween.Sequence();
            rotationToTarget.Append(gameObject.transform.DORotateQuaternion(rotation, duration));
        }

        protected void ShowTextDamage(int damage)
        {
            PoolManager.PoolObjectType poolObj = PoolManager.PoolObjectType.TextDamage;

            var textDamage = PoolManager.Instance.GetPooledObject(poolObj);
            
            if (textDamage)
            {
                if (enemyType == Enemies.SharkBoss)
                {
                    textDamage.transform.position = transform.position + new Vector3(0, 8, 0);
                }
                else if (enemyType == Enemies.SkeletonBoss)
                {
                    textDamage.transform.position = transform.position + new Vector3(0, 8, 0);
                }
                else
                {
                    textDamage.transform.position = transform.position;
                }
                textDamage.transform.rotation = Quaternion.identity;
                textDamage.SetActive(true);
                textDamage.GetComponent<TextDamage>().Init(damage, false);
            }
        }
        
        #region MOVE_ATTACK
        
        public void EnemyMoveAttack()
        {
            if (IsStun) return;
            
            RotationToTarget(true);
            moveAttackTime = moveAttackData[currentCombo].MoveAttackTime;
            moveAttackAcceleration = moveAttackData[currentCombo].MoveAttackAcceleration;
            StartCoroutine(nameof(MoveAttackTime));
        }

        public void ResetCurrentCombo()
        {
            currentCombo = 0;
        }
        
        private IEnumerator MoveAttackTime()
        {
            controllerRb.isKinematic = false;
            
            while (moveAttackTime > 0)
            {
                if (IsStun)
                {
                    break;
                }
                
                if (!playerData.IsUsingUI)
                {
                    var acceleration = transform.forward.normalized * moveAttackAcceleration;

                    controllerRb.AddForce(acceleration, ForceMode.VelocityChange);
                    moveAttackTime -= Time.deltaTime;
                }
                yield return null;
            }

            controllerRb.isKinematic = true;
            currentCombo++;
        }

        public void GetMoveAttackData(MoveAttackData[] moveAttackData)
        {
            currentCombo = 0;
            this.moveAttackData = moveAttackData;
        }
        
        public void EnemySetCollider(int number)
        {
            // 0=Can surpassed.
            // 1=Can't surpassed.
            capsuleCollider.isTrigger = number switch
            {
                0 => true,
                1 => false,
                _ => capsuleCollider.isTrigger
            };
        }

        #endregion

        #region QUEST

        public void UpdateQuest1(WeaponTypes weaponTypes)
        {
            if (questManagerSO.CurrentQuests == null) return;
            
            if (!questManagerSO.CanDoQuest(NameQuest.FirstFightInTheNewWorld) 
                || questManagerSO.CurrentQuests.IsSucceed) return;
            
            if (weaponTypes == WeaponTypes.Sword)
            {
                questManagerSO.UpdateProgressQuest(QuestRequirement.UseSwordToKill3Enemies,out bool finish);
            }
            else if (weaponTypes == WeaponTypes.Gun)
            {
                questManagerSO.UpdateProgressQuest(QuestRequirement.UseGunToKill3Enemies,out bool finish);
            }
            else if (weaponTypes == WeaponTypes.Knuckle)
            {
                questManagerSO.UpdateProgressQuest(QuestRequirement.UseKnuckleToKill3Enemies,out bool finish);
            }
            
            questManagerSO.CurrentQuests.UpdateProgress();
        }
        
        public void UpdateQuestDefeatTheInvader(NameQuest nameQuest)
        {
            if (questManagerSO.CurrentQuests == null) return;
            
            if (!questManagerSO.CanDoQuest(nameQuest) || questManagerSO.CurrentQuests.IsSucceed) return;

            if (nameQuest == NameQuest.DefeatTheInvade)
            {
                if (farmingManager.Dungeons != Dungeons.DeadMansHeaven) return;
                questManagerSO.UpdateProgressQuest(QuestRequirement.Defeat20MonstersInAbandonIslandDeadMansHeaven,
                    out bool finish);
            }
            else if (nameQuest == NameQuest.DefeatTheInvader02)
            {
                if (farmingManager.Dungeons != Dungeons.DeadIsle) return;
                questManagerSO.UpdateProgressQuest(QuestRequirement.Defeat20MonstersInAbandonIslandDeadIsle,
                    out bool finish);
            }
            else if (nameQuest == NameQuest.DefeatTheInvader03)
            {
                if (farmingManager.Dungeons != Dungeons.BootyCove) return;
                questManagerSO.UpdateProgressQuest(QuestRequirement.Defeat20MonstersInAbandonIslandBootyCove,
                    out bool finish);
            }
            else
            {
                return;
            }
            
            questManagerSO.CurrentQuests.UpdateProgress();
        }

        #endregion

        #region SOUND

        protected void GetDataSound(AudioSource audioSource, SoundClip.Sound sound)
        {
            var findTemp = SoundHandler.Instance.GetAudioSourceSFX(sound);
            audioSource.clip = findTemp.audioClip;
            audioSource.loop = findTemp.loop;
            audioSource.volume = findTemp.soundVolume;
            audioSource.playOnAwake = false;
            audioSource.mute = findTemp.mute;
            audioSource.outputAudioMixerGroup = findTemp.audioSource.outputAudioMixerGroup;
        }

        #endregion

        public abstract void StopSfx();
    }
}