using System;
using System.Collections.Generic;
using Kamaria.Item.Weapon;
using UnityEngine;

namespace Kamaria.Item
{
    public enum ItemTypes
    {
        None,
        Weapon,
        Support,
        CraftMaterial,
        KeyStone,
        Potion,
        Equipment1, Equipment2,
    }

    public enum ItemsName
    {
        None,
        Cutlass, JunkSword, VitricaSword,
        MittKnuckle, IronKnuckle, IonGauntlets,
        FlintlockPistol, ScrapSniper, Zatvor,
        FireStone, WindStone, PowerStone,
        GlassTech, BangleTech,
        HealthPotion,
        Wood, Rope, Rock, Hide, Iron, Obsidian, Sedimentary, Gold, Diamond, GripLv1, GripLv2, Pommel, SharpeningStone,
        Power, Glove, HandArmor, Knuckle, Armor, GunGrip, GunBody, Muzzle
    }

    public enum KeyStones
    {
        None,
        FireStone, WindStone, PowerStone,
    }

    public enum WeaponTypes
    {
        None,
        Sword, Knuckle, Gun
    }

    public enum AttacksRange
    {
        None,
        MeleeAttack, RangeAttack
    }
    
    [Serializable]
    public sealed class BaseItem
    {
        public ItemsName[] Name;
        public ItemTypes Types = ItemTypes.None;
        public WeaponTypes WeaponType = WeaponTypes.None;
        public KeyStones UsedKeyStone = KeyStones.None;
        public AttacksRange TypeAttackRange = AttacksRange.None;
        public int MaxLevel;
        public int Level;
        public Sprite[] Image;
        public int[] MaxHealth;
        [Range(0, 100)] public int IncreaseCurrentHealth;
        public int[] Atk;
        [Range(0, 100)] public int[] CritRate;
        public float[] ReductionDamage;
        public float[] SpeedAttack;
        public int[] MaxCombo;
        [Range(0, 100)] public float[] AttackRange;
        [Range(0, 100)] public float[] IncreaseDashAcceleration;
        public WeaponComponent[] WeaponComponentLevel;
        public bool IsUsedEquip = false;
        public int Count;
        
        public int LevelIndex => Level - 1;

        public void Initialized()
        {
            MaxLevel = 0;
            Level = 0;
            Name = new ItemsName[MaxLevel];
            Types = ItemTypes.None;
            WeaponType = WeaponTypes.None;
            UsedKeyStone = KeyStones.None;
            TypeAttackRange = AttacksRange.None;
            Image = new Sprite[MaxLevel];
            MaxHealth = new int[MaxLevel];
            IncreaseCurrentHealth = 0;
            Atk = new int[MaxLevel];
            CritRate = new int[MaxLevel];
            ReductionDamage = new float[MaxLevel];
            SpeedAttack = new float[MaxLevel];
            MaxCombo = new int[MaxLevel];
            AttackRange = new float[MaxLevel];
            IncreaseDashAcceleration = new float[MaxLevel];
            WeaponComponentLevel = new WeaponComponent[MaxLevel];
            IsUsedEquip = false;
            Count = 0;
        }

        /// <summary>
        /// Get info item.
        /// </summary>
        /// <param name="item"> Item info </param>
        public void GetInfo(BaseItem item)
        {
            Name = item.Name;
            Types = item.Types;
            WeaponType = item.WeaponType;
            UsedKeyStone = item.UsedKeyStone;
            TypeAttackRange = item.TypeAttackRange;
            MaxLevel = item.MaxLevel;
            Level = item.Level;
            Image = item.Image;
            MaxHealth = item.MaxHealth;
            IncreaseCurrentHealth = item.IncreaseCurrentHealth;
            Atk = item.Atk;
            CritRate = item.CritRate;
            ReductionDamage = item.ReductionDamage;
            SpeedAttack = item.SpeedAttack;
            MaxCombo = item.MaxCombo;
            AttackRange = item.AttackRange;
            IncreaseDashAcceleration = item.IncreaseDashAcceleration;
            WeaponComponentLevel = item.WeaponComponentLevel;
            IsUsedEquip = item.IsUsedEquip;
            Count = item.Count;
        }
        
        public void TempGetInfo(BaseItem item)
        {
            MaxLevel = item.MaxLevel;
            Level = item.Level;
            Name = new ItemsName[item.Name.Length];
            Types = item.Types;
            WeaponType = item.WeaponType;
            UsedKeyStone = item.UsedKeyStone;
            TypeAttackRange = item.TypeAttackRange;
            Image = new Sprite[item.Image.Length];
            MaxHealth = new int[item.MaxHealth.Length];
            IncreaseCurrentHealth = item.IncreaseCurrentHealth;
            Atk = new int[item.Atk.Length];
            CritRate = new int[item.CritRate.Length];
            ReductionDamage = new float[item.ReductionDamage.Length];
            SpeedAttack = new float[item.SpeedAttack.Length];
            MaxCombo = new int[item.MaxCombo.Length];
            AttackRange = new float[item.AttackRange.Length];
            IncreaseDashAcceleration = new float[item.IncreaseDashAcceleration.Length];
            WeaponComponentLevel = new WeaponComponent[item.WeaponComponentLevel.Length];
            IsUsedEquip = item.IsUsedEquip;
            Count = item.Count;
            
            for (int i = 0; i < item.Name.Length; i++)
            {
                Name[i] = item.Name[i];
            }
            
            for (int i = 0; i < item.Image.Length; i++)
            {
                Image[i] = item.Image[i];
            }

            for (int i = 0; i < item.MaxHealth.Length; i++)
            {
                MaxHealth[i] = item.MaxHealth[i];
            }
            
            for (int i = 0; i < item.Atk.Length; i++)
            {
                Atk[i] = item.Atk[i];
            }
            
            for (int i = 0; i < item.CritRate.Length; i++)
            {
                CritRate[i] = item.CritRate[i];
            }
            
            for (int i = 0; i < item.ReductionDamage.Length; i++)
            {
                ReductionDamage[i] = item.ReductionDamage[i];
            }
            
            for (int i = 0; i < item.SpeedAttack.Length; i++)
            {
                SpeedAttack[i] = item.SpeedAttack[i];
            }
            
            for (int i = 0; i < item.MaxCombo.Length; i++)
            {
                MaxCombo[i] = item.MaxCombo[i];
            }
            
            for (int i = 0; i < item.AttackRange.Length; i++)
            {
                AttackRange[i] = item.AttackRange[i];
            }
            
            for (int i = 0; i < item.IncreaseDashAcceleration.Length; i++)
            {
                IncreaseDashAcceleration[i] = item.IncreaseDashAcceleration[i];
            }
            
            for (int i = 0; i < item.WeaponComponentLevel.Length; i++)
            {
                WeaponComponentLevel[i] = new WeaponComponent
                {
                    IsEvolution = item.WeaponComponentLevel[i].IsEvolution,
                    UpgradeComponent1 = item.WeaponComponentLevel[i].UpgradeComponent1,
                    UpgradeComponent2 = item.WeaponComponentLevel[i].UpgradeComponent2,
                    UpgradeComponent3 = item.WeaponComponentLevel[i].UpgradeComponent3
                };
            }
        }
        
        /// <summary>
        /// Up level weapon.
        /// </summary>
        public void UpLevel()
        {
            if (Types != ItemTypes.Weapon && Types != ItemTypes.Support) return;

            if (Types == ItemTypes.Weapon )
            {
                if (!WeaponComponentLevel[LevelIndex].FullUpgrade)
                { 
                    return;
                }
            }

            if (Level + 1 <= MaxLevel) 
            {
               Level++; 
            }
        }

        /// <summary>
        /// Upgrade component.
        /// </summary>
        /// <param name="component"> Upgrade number component. </param>
        public void UpgradeComponent(int component)
        {
            if (component == 1)
            {
                WeaponComponentLevel[LevelIndex].UpgradeComponent1 = true;
            }
            else if (component == 2)
            {
                WeaponComponentLevel[LevelIndex].UpgradeComponent2 = true;
            }
            else if (component == 3)
            {
                WeaponComponentLevel[LevelIndex].UpgradeComponent3 = true;
            }
        }

        /// <summary>
        /// Add key stone to Weapon.
        /// </summary>
        /// <param name="itemPlayer"> Item weapon. </param>
        public void AddKeyStone(BaseItem itemPlayer)
        {
            if (itemPlayer.Types is not ItemTypes.Weapon) return;

            if (itemPlayer.UsedKeyStone != KeyStones.None || IsUsedEquip) return;

            itemPlayer.UsedKeyStone = Name[LevelIndex] switch
            {
                ItemsName.PowerStone => KeyStones.PowerStone,
                ItemsName.WindStone => KeyStones.WindStone,
                ItemsName.FireStone => KeyStones.FireStone,
                _ => itemPlayer.UsedKeyStone
            };
            
            //IsUsedEquip = true;
        }
        
        /// <summary>
        /// Remove key stone from Weapon.
        /// </summary>
        /// <param name="itemPlayer"> Item weapon. </param>
        public void RemoveKeyStone(BaseItem itemPlayer)
        {
            if (itemPlayer.Types is not ItemTypes.Weapon) return;

            itemPlayer.UsedKeyStone = KeyStones.None; 
            //IsUsedEquip = false;
        }
        
        /// <summary>
        /// Increase count item.
        /// </summary>
        /// <param name="count"> Increase count. </param>
        public void IncreaseCount(int count)
        {
            Count += count;
        }

        /// <summary>
        /// Decrease count item.
        /// </summary>
        /// <param name="count"> Decrease count </param>
        public void DecreaseCount(int count)
        {
            if (Count - count >= 0)
            {
                Count -= count;
            }
        }

        /// <summary>
        /// Reset count item.
        /// </summary>
        public void ResetCount()
        {
            Count = 0;
        }
        
        /// <summary>
        /// Out count stack and fraction.
        /// </summary>
        /// <param name="stack"> Max stack </param>
        /// <param name="resultCountStack"> Out count stack </param>
        /// <param name="resultCountFraction"> Out count fraction </param>
        public void ResultStack(int stack, out int resultCountStack, out int resultCountFraction)
        {
            resultCountStack = (int)Count / stack; 
            resultCountFraction = (int)Count % stack;
        }
        
        /// <summary>
        /// Out count stack and fraction.
        /// </summary>
        /// <param name="stack"> Max stack </param>
        /// <param name="resultCountStack"> Out count stack </param>
        /// <param name="resultCountFraction"> Out count fraction </param>
        /// <param name="countAdd"> Count add </param>
        public void ResultStack(int stack, out int resultCountStack, out int resultCountFraction, int countAdd)
        {
            int temp = Count + countAdd;
            resultCountStack = (int)temp / stack; 
            resultCountFraction = (int)temp % stack;
        }
    }
}