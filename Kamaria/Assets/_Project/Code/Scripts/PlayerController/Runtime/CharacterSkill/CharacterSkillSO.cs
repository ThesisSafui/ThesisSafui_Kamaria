using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Kamaria.Player.Controller
{
    [CreateAssetMenu(fileName = "New CharacterSkillData", menuName = "ThesisSafui/Data/CharacterSkillData")]
    public sealed class CharacterSkillSO : ScriptableObject
    {
        public bool IsChangeWeaponCooldown;
        public bool IsChangeWeaponKeyStone;
        public bool IsUseSkillPowerStone;
        public bool IsUseActiveSkill;
        public bool IsUseUltimateSkill;
        public bool IsUsePassiveSkill;
        public bool IsUseSkillCanNotAttack;
        public int CurrentCombo;
        public bool IsChangeWeaponFinish = true;
        public bool IsBowAttackFinish = true;
        public bool IsBowAttacking = false;
        public bool IsBowChargeAttacking = false;
        public bool IsBowCharging = false;
        public Sequence RotationAttackSequence;
        public Sequence RotationDashSequence;
        public bool KnuckleLeft = false;
        public bool KnuckleRight = false;
        public bool KnuckleHeavyPunch = false;
        public bool PunchFinish = true;
        public bool UsingGuard = false;
        public bool ChangeWeaponFinish = true;

        [SerializeField] private float durationAttack;
        [SerializeField] private List<MoveAttackData> swordMoveAttacks = new List<MoveAttackData>();
        [SerializeField] private List<MoveAttackData> knuckleMoveAttacks = new List<MoveAttackData>();
        [Header("Cooldown Skill Sword")]
        [SerializeField] private CooldownSkillData skillSwordComponent1Cooldown;
        [SerializeField] private CooldownSkillData skillSwordRedStoneCooldown;
        [SerializeField] private CooldownSkillData skillSwordPowerStoneCooldown;
        [SerializeField] private CooldownSkillData bowAttackDuration;
        [Header("Cooldown Skill Knuckle")]
        [SerializeField] private CooldownSkillData skillKnuckleComponent1Cooldown;
        [SerializeField] private CooldownSkillData skillKnuckleRedStoneCooldown;
        [SerializeField] private CooldownSkillData skillKnuckleGreenStoneCooldown;
        [SerializeField] private CooldownSkillData skillKnucklePowerStoneCooldown;
        [SerializeField] private CooldownSkillData cannonAttackCooldown;
        [Header("Cooldown Skill Gun")]
        [SerializeField] private CooldownSkillData gunAttackCooldown;
        [SerializeField] private CooldownSkillData skillGunComponent1Cooldown;
        [SerializeField] private CooldownSkillData skillGunRedStoneCooldown;
        [SerializeField] private CooldownSkillData skillGunGreenStoneCooldown;
        [SerializeField] private CooldownSkillData skillGunPowerStoneCooldown;
        [Space] 
        [SerializeField] private float cooldownPotion;
        
        [SerializeField] private float bowAttackRotationSpeed;
        [SerializeField] private float bowChargeTime = 1;
        [SerializeField] private float cooldownChangeWeapon;
        [SerializeField] private float waitNextComboKnuckleTime;
        [SerializeField] private float cannonAttackRotationSpeed;
        [SerializeField] private float limitCannonAttack;
        [SerializeField] private float limitCannonAttackAim;
        [SerializeField] private float limitIndicatorCannonAttackSize;
        [SerializeField] private float limitIndicatorCannonAttackAimSize;
        [SerializeField] private List<float> guardTime;

        public List<float> GuardTime => guardTime;
        public float CooldownPotion => cooldownPotion;
        public float LimitIndicatorCannonAttackSize => limitIndicatorCannonAttackSize;
        public float LimitIndicatorCannonAttackAimSize => limitIndicatorCannonAttackAimSize;
        public float LimitCannonAttack => limitCannonAttack;
        public float LimitCannonAttackAim => limitCannonAttackAim;
        public CooldownSkillData SkillGunRedStoneCooldown => skillGunRedStoneCooldown;
        public CooldownSkillData SkillGunGreenStoneCooldown => skillGunGreenStoneCooldown;
        public CooldownSkillData SkillGunPowerStoneCooldown => skillGunPowerStoneCooldown;
        public CooldownSkillData SkillKnuckleComponent1Cooldown => skillKnuckleComponent1Cooldown;
        public CooldownSkillData SkillGunComponent1Cooldown => skillGunComponent1Cooldown;
        public CooldownSkillData GunAttackCooldown => gunAttackCooldown;
        public CooldownSkillData CannonAttackCooldown => cannonAttackCooldown;
        public CooldownSkillData SkillKnuckleRedStoneCooldown => skillKnuckleRedStoneCooldown;
        public CooldownSkillData SkillKnuckleGreenStoneCooldown => skillKnuckleGreenStoneCooldown;
        public CooldownSkillData SkillKnucklePowerStoneCooldown => skillKnucklePowerStoneCooldown;
        public List<MoveAttackData> SwordMoveAttacks => swordMoveAttacks;
        public List<MoveAttackData> KnuckleMoveAttacks => knuckleMoveAttacks;
        public CooldownSkillData SkillSwordComponent1Cooldown => skillSwordComponent1Cooldown;
        public CooldownSkillData SkillSwordRedStoneCooldown => skillSwordRedStoneCooldown;
        public CooldownSkillData SkillSwordPowerStoneCooldown => skillSwordPowerStoneCooldown;
        public CooldownSkillData BowAttackDuration => bowAttackDuration;
        public float DurationAttack => durationAttack;
        public float BowAttackRotationSpeed => bowAttackRotationSpeed;
        public float CannonAttackRotationSpeed => cannonAttackRotationSpeed;
        public float BowChargeTime => bowChargeTime;
        public float CooldownChangeWeapon => cooldownChangeWeapon;
        public float WaitNextComboKnuckleTime => waitNextComboKnuckleTime;
        
        private void OnEnable()
        {
            Initialized();
        }

        public void Initialized()
        {
            ChangeWeaponFinish = true;
            CurrentCombo = 0;
            IsUseSkillCanNotAttack = false;
            IsUseActiveSkill = false;
            IsUseUltimateSkill = false;
            IsUseSkillPowerStone = false;
            IsBowAttacking = false;
            IsBowCharging = false;
            IsBowChargeAttacking = false;
            IsChangeWeaponFinish = true;
            IsBowAttackFinish = true;
            IsChangeWeaponKeyStone = false;
            IsChangeWeaponCooldown = false;

            KnuckleLeft = false;
            KnuckleRight = false;
            KnuckleHeavyPunch = false;
            PunchFinish = true;
            UsingGuard = false;
        }

        public void ResetComboKnuckle()
        {
            KnuckleLeft = false;
            KnuckleRight = false;
            KnuckleHeavyPunch = false;
        }
    }

    [Serializable]
    public sealed class CooldownSkillData
    {
        [SerializeField] private List<float> cooldown;

        public List<float> Cooldown => cooldown;
    }
}