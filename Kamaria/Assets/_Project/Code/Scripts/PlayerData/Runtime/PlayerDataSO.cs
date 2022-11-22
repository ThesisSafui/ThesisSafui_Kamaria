using System.Collections.Generic;
using Kamaria.Item;
using Kamaria.Player.Animation;
using Kamaria.Player.Controller;
using UnityEngine;
using Random = System.Random;

namespace Kamaria.Player.Data
{
    [CreateAssetMenu(fileName = "New PlayerData", menuName = "ThesisSafui/Data/Player")]
    public sealed class PlayerDataSO : ScriptableObject
    {
        [Header("Reference")]
        [Tooltip("Info CharacterControllerDataSO as ScriptableObject")]
        [SerializeField] private CharacterControllerDataSO characterControllerData;
        [SerializeField] private CharacterSkillSO characterSkillData;
        [SerializeField] private BaseItemSO bangleTech;
        [SerializeField] private BaseItemSO glassTech;

        [Space]
        public PlayerData Info = new PlayerData();
        public PlayerAnimationData PlayerAnimation = new PlayerAnimationData();

        public CharacterControllerDataSO CharacterControllerData => characterControllerData;
        public CharacterSkillSO CharacterSkillData => characterSkillData;
        public List<PlayerDamageCollier> PlayerDamageColliers;

        public bool IsBowChargeFull;
        public bool IsDead;
        public bool IsStun;
        public bool IsInteract;
        public bool CanInteract;
        public bool IsUsingUI;
        public bool TelePort;
        public List<GameObject> UIsLinkEsc;
        public bool IsStop => IsDead || IsStun || IsUsingUI;
        public bool QuestPasswordSucceed = false;

        private Random random = new Random();
        
        public void DebugLog()
        {
            string json = JsonUtility.ToJson(Info, true);
            Debug.Log($"DDD{json}");
        }

        private void OnEnable()
        {
            Info.Initialized();
            Info.DeviceCompartment.SlotBangleTech.Info.GetInfo(bangleTech.Info);
            Info.DeviceCompartment.SlotGlassTech.Info.GetInfo(glassTech.Info);
            Info.DeviceCompartment.SlotBangleTech.IsEmpty = false;
            Info.DeviceCompartment.SlotGlassTech.IsEmpty = false;

            /*if (Info.DeviceCompartment.SlotWeapon.IsEmpty)
            {
                Info.DeviceCompartment.SlotWeapon.Add(
                    Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Sword), this);
            }*/
           
            Initialized();
        }

        public void UseWeapon(WeaponTypes weaponTypes)
        {
            Info.DeviceCompartment.SlotWeapon.Remove(Info.DeviceCompartment.SlotWeapon.Info, this);
            Info.DeviceCompartment.SlotWeapon.Add(
                Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == weaponTypes), this);
        }

        public void Initialized()
        {
            if (!TelePort)
            {
                Info.Status.CurrentHealth = Info.Status.MaxHealth;
            }
            
            PlayerAnimation.IsAttacking = false;
            PlayerAnimation.IsAnimationAttackNotMove = false;

            IsDead = false;
            IsStun = false;
            IsBowChargeFull = false;
            IsInteract = false;
            CanInteract = false;
            IsUsingUI = false;
            TelePort = false;
            QuestPasswordSucceed = false;
        }

        public void SetAnimator(out Animator animator)
        {
            BaseItem weapon = Info.DeviceCompartment.SlotWeapon.Info;
            
            PlayerAnimation.Animator.runtimeAnimatorController = PlayerAnimation.AnimatorControllers;
            
            switch (weapon.WeaponType)
            {
                case WeaponTypes.Sword:
                {
                    SetSpeedAnimation(PlayerAnimation.Animator, weapon.WeaponComponentLevel[0].UpgradeComponent1);
                    break;
                }
                case WeaponTypes.Gun:
                {
                    SetSpeedAnimation(PlayerAnimation.Animator, weapon.WeaponComponentLevel[0].UpgradeComponent1); 
                    break;
                }
                case WeaponTypes.Knuckle:
                {
                    break;
                }
            }
            
            animator = PlayerAnimation.Animator;
        }

        public void CloseAllColliderDamage()
        {
            foreach (var playerDamageCollier in PlayerDamageColliers)
            {
                playerDamageCollier.gameObject.SetActive(false);
            }
        }

        public void SetSpeedAnimation(Animator animator, bool isUpSpeed)
        {
            BaseItem weapon = Info.DeviceCompartment.SlotWeapon.Info;

            switch (weapon.WeaponType)
            {
                case WeaponTypes.Sword:
                {
                    SpeedAnimation speedAnimation =
                        PlayerAnimation.SpeedAnimations.Find(x => x.WeaponTypes == WeaponTypes.Sword);

                    if (isUpSpeed)
                    {
                        animator.SetFloat(PlayerAnimation.AnimCombo1Speed, speedAnimation.SpeedAnimCombo1);
                        animator.SetFloat(PlayerAnimation.AnimCombo2Speed, speedAnimation.SpeedAnimCombo2);
                        animator.SetFloat(PlayerAnimation.AnimCombo3Speed, speedAnimation.SpeedAnimCombo3);
                        animator.SetFloat(PlayerAnimation.AnimCombo4Speed, speedAnimation.SpeedAnimCombo4);
                    }
                    else
                    {
                        animator.SetFloat(PlayerAnimation.AnimCombo1Speed, speedAnimation.DefaultSpeedAnimCombo1);
                        animator.SetFloat(PlayerAnimation.AnimCombo2Speed, speedAnimation.DefaultSpeedAnimCombo2);
                        animator.SetFloat(PlayerAnimation.AnimCombo3Speed, speedAnimation.DefaultSpeedAnimCombo3);
                        animator.SetFloat(PlayerAnimation.AnimCombo4Speed, speedAnimation.DefaultSpeedAnimCombo4);
                    }

                    break;
                }
                case WeaponTypes.Knuckle:
                    break;
                case WeaponTypes.Gun:
                    SpeedAnimation speedAnimationGun =
                        PlayerAnimation.SpeedAnimations.Find(x => x.WeaponTypes == WeaponTypes.Gun);
                    
                    animator.SetFloat(PlayerAnimation.AnimGunSpeed,
                        isUpSpeed ? speedAnimationGun.SpeedAnimCombo1 : speedAnimationGun.DefaultSpeedAnimCombo1);
                    break;
            }
        }
        
        public void SetDataAttack(bool isOpen, PlayerDamageCollier playerDamageCollier, out int damage, 
            out EffectAttack effectAttack, out WeaponTypes weaponTypes, out KeyStones keyStones)
        {
            EffectAttack tempEffectAttack = playerDamageCollier.PlayerDamage.EffectAttack;
            
            if (playerDamageCollier.CanCharge)
            {
                if (playerDamageCollier.PlayerDamage.CollidersDamage == CollidersDamage.ArrowCollider)
                {
                    tempEffectAttack = IsBowChargeFull ? playerDamageCollier.PlayerDamage.EffectAttack : EffectAttack.None;
                }
            }

            weaponTypes = Info.DeviceCompartment.SlotWeapon.Info.WeaponType;
            keyStones = Info.DeviceCompartment.SlotWeapon.Info.UsedKeyStone;
            playerDamageCollier.gameObject.SetActive(isOpen);
            CalculatorDamage(playerDamageCollier, isOpen, out damage);
            effectAttack = tempEffectAttack;
            
            if (playerDamageCollier.PlayerDamage.CollidersDamage == CollidersDamage.KnuckleComboHeavyPunchStun)
            {
                effectAttack = random.Next(10) < playerDamageCollier.PlayerDamage.KnucklePercentStun 
                    ? tempEffectAttack : EffectAttack.None;
            }
        }

        private void CalculatorDamage(PlayerDamageCollier playerDamageCollier, bool isOpen, out int damage)
        {
            float tempDamage = 0;
           
            if (isOpen)
            {
                tempDamage = (Info.Status.Atk * playerDamageCollier.PlayerDamage.DamagePercent) / 100;
            }

            if (playerDamageCollier.CanCharge)
            {
                if (playerDamageCollier.PlayerDamage.CollidersDamage == CollidersDamage.ArrowCollider)
                {
                    if (IsBowChargeFull)
                    {
                        Debug.Log("BowChargeFull Damage");
                        tempDamage *= playerDamageCollier.PlayerDamage.DamageCharge;
                    }
                }
            }

            damage = (int)tempDamage;
        }
    }
}