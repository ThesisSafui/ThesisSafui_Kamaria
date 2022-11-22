using System;
using Kamaria.Item.Weapon;
using UnityEngine;

namespace Kamaria.Item
{
    [CreateAssetMenu(fileName = "New Item", menuName = "ThesisSafui/Item")]
    public sealed class BaseItemSO : ScriptableObject
    {
        [SerializeField] private BaseItem defaultItem = new BaseItem();
        
        public BaseItem Info = new BaseItem();
        
        [ContextMenu("Change value to default")]
        public void SetValueDefault()
        {
            Info.TempGetInfo(defaultItem);
        }
        
        #region USE_GUI_EDITOR
        
        
        [ContextMenu("Reset item")]
        public void ResetItem()
        {
            Info.Initialized();
        }

        [ContextMenu("Add count")]
        public void AddCount()
        {
            Info.IncreaseCount(10);
        }
        
        [ContextMenu("Reset count")]
        public void ResetCount()
        {
            Info.ResetCount();
        }

        [ContextMenu("Set item key stone")]
        public void SetItemKeyStone()
        {
            Info.MaxLevel = 1;
            Info.Level = 1;
            Info.Types = ItemTypes.KeyStone;
            InitializedGeneral();
        }
        
        [ContextMenu("Set item craft material")]
        public void SetItemCraftMaterial()
        {
            Info.MaxLevel = 1;
            Info.Level = 1;
            Info.Types = ItemTypes.CraftMaterial; 
            InitializedGeneral();
        }
        
        [ContextMenu("Set item potion")]
        public void SetItemPotion()
        {
            Info.MaxLevel = 1;
            Info.Level = 1;
            Info.Types = ItemTypes.Potion;
            InitializedGeneral();
        }
        
        [ContextMenu("Set item support")]
        public void SetItemSupport()
        {
            Info.MaxLevel = 3;
            Info.Level = 1;
            Info.Types = ItemTypes.Support;
            InitializedGeneral();
        }

        [ContextMenu("Set item weapon sword")]
        public void SetItemWeaponSword()
        {
            Info.MaxLevel = 3;
            Info.Level = 1;
            Info.WeaponType = WeaponTypes.Sword;
            Info.TypeAttackRange = AttacksRange.MeleeAttack;
            InitializedWeapon();
        }
        
        [ContextMenu("Set item weapon knuckle")]
        public void SetItemWeaponKnuckle()
        {
            Info.MaxLevel = 3;
            Info.Level = 1;
            Info.WeaponType = WeaponTypes.Knuckle;
            Info.TypeAttackRange = AttacksRange.MeleeAttack;
            InitializedWeapon();
        }
        
        [ContextMenu("Set item weapon gun")]
        public void SetItemWeaponGun()
        {
            Info.MaxLevel = 3;
            Info.Level = 1;
            Info.WeaponType = WeaponTypes.Gun;
            Info.TypeAttackRange = AttacksRange.RangeAttack;
            InitializedWeapon();
        }
        
        [ContextMenu("Weapon refresh index create")]
        public void RefreshIndexItemWeapon()
        {
            Info.Name = new ItemsName[Info.MaxLevel];
            Info.Image = new Sprite[Info.MaxLevel];
            Info.MaxHealth = new int[Info.MaxLevel];
            Info.Atk = new int[Info.MaxLevel];
            Info.CritRate = new int[Info.MaxLevel];
            Info.ReductionDamage = new float[Info.MaxLevel];
            Info.SpeedAttack = new float[Info.MaxLevel];
            Info.MaxCombo = new int[Info.MaxLevel];
            Info.AttackRange = new float[Info.MaxLevel];
            Info.IncreaseDashAcceleration = new float[Info.MaxLevel];
            Info.WeaponComponentLevel = new WeaponComponent[Info.MaxLevel];
        }
        
        [ContextMenu("General refresh index create")]
        public void RefreshIndexItemGeneral()
        {
            Info.Name = new ItemsName[Info.MaxLevel];
            Info.Image = new Sprite[Info.MaxLevel];
            Info.MaxHealth = Array.Empty<int>();
            Info.Atk = Array.Empty<int>();
            Info.CritRate = Array.Empty<int>();
            Info.ReductionDamage = Array.Empty<float>();
            Info.SpeedAttack = Array.Empty<float>();
            Info.MaxCombo = Array.Empty<int>();
            Info.AttackRange = Array.Empty<float>();
            Info.IncreaseDashAcceleration = Array.Empty<float>();
            Info.WeaponComponentLevel = Array.Empty<WeaponComponent>();
        }

        private void InitializedWeapon()
        {
            Info.Name = new ItemsName[Info.MaxLevel];
            Info.Types = ItemTypes.Weapon;
            Info.UsedKeyStone = KeyStones.None;
            Info.Image = new Sprite[Info.MaxLevel];
            Info.MaxHealth = new int[Info.MaxLevel];
            Info.IncreaseCurrentHealth = 0;
            Info.Atk = new int[Info.MaxLevel];
            Info.CritRate = new int[Info.MaxLevel];
            Info.ReductionDamage = new float[Info.MaxLevel];
            Info.SpeedAttack = new float[Info.MaxLevel];
            Info.MaxCombo = new int[Info.MaxLevel];
            Info.AttackRange = new float[Info.MaxLevel];
            Info.IncreaseDashAcceleration = new float[Info.MaxLevel];
            Info.WeaponComponentLevel = new WeaponComponent[Info.MaxLevel];
            Info.IsUsedEquip = false;
            Info.Count = 0;
        }

        private void InitializedGeneral()
        {
            Info.Name = new ItemsName[Info.MaxLevel];
            Info.WeaponType = WeaponTypes.None;
            Info.UsedKeyStone = KeyStones.None;
            Info.TypeAttackRange = AttacksRange.None;
            Info.Image = new Sprite[Info.MaxLevel];
            Info.MaxHealth = Array.Empty<int>();
            Info.IncreaseCurrentHealth = 0;
            Info.Atk = Array.Empty<int>();
            Info.CritRate = Array.Empty<int>();
            Info.ReductionDamage = Array.Empty<float>();
            Info.SpeedAttack = Array.Empty<float>();
            Info.MaxCombo = Array.Empty<int>();
            Info.AttackRange = Array.Empty<float>();
            Info.IncreaseDashAcceleration = Array.Empty<float>();
            Info.WeaponComponentLevel = Array.Empty<WeaponComponent>();
            Info.IsUsedEquip = false;
            Info.Count = 0;
        }

        #endregion
    }
}