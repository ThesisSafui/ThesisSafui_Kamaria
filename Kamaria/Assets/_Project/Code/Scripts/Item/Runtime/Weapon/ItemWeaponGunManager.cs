using System;
using System.Collections.Generic;
using Kamaria.Manager;
using Kamaria.Player.Data;
using Kamaria.Player.Data.Quest;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.Item.Weapon
{
    public sealed class ItemWeaponGunManager : MonoBehaviour, IBaseItemWeaponManager
    {
        [Header("Referent info")]
        [SerializeField] private QuestManagerSO questManagerSO;
        [SerializeField] private  PlayerDataSO playerData;
        [SerializeField] private ItemWeaponManagerSO itemWeaponData;
        [SerializeField] private List<ItemWeaponRequirementLevel> itemRequirementLevels;
        [SerializeField] private UIManager uiManager;
        public List<ItemWeaponRequirementLevel> ItemRequirementLevels => itemRequirementLevels;

        private RectTransform currentUpgradPanel;
        private RectTransform currentEvolutionPanel;
        private bool isUpgradeSucceed;
        
        private void UpdateQuest2()
        {
            if (questManagerSO.CurrentQuests == null) return;
            
            if (!questManagerSO.CanDoQuest(NameQuest.TheFirstCraft) 
                || questManagerSO.CurrentQuests.IsSucceed) return;
            
            questManagerSO.UpdateProgressQuest(QuestRequirement.UpgradeAnyWeaponInAnyPart,out bool finish);

            questManagerSO.CurrentQuests.UpdateProgress();
        }
        
        [ContextMenu("Unlock Skill Component (1)")]
        public void UnlockSkillComponent1()
        {
            isUpgradeSucceed = false;
            
            BaseItem weaponGun = WeaponGun();

            if (IsCanUpgradeComponent(itemRequirementLevels[weaponGun.LevelIndex].ItemsRequirementComponent1))
            {
                if (weaponGun.WeaponComponentLevel[weaponGun.LevelIndex].FullUpgrade ||
                    weaponGun.WeaponComponentLevel[weaponGun.LevelIndex].UpgradeComponent1) return;
            
                switch (weaponGun.LevelIndex)
                {
                    case 0:
                        UnlockSkillComponent1Level1(weaponGun);
                        break;
                    case 1:
                        UnlockSkillComponent1Level2(weaponGun);
                        break;
                    case 2:
                        UnlockSkillComponent1Level3(weaponGun);
                        break;
                }

                weaponGun.UpgradeComponent(1);
                IncreaseStatus(weaponGun);
                isUpgradeSucceed = true;

                UpdateQuest2();
            }
        }

        [ContextMenu("Unlock Skill Component (2)")]
        public void UnlockSkillComponent2()
        {
            isUpgradeSucceed = false;
            
            BaseItem weaponGun = WeaponGun();

            if (IsCanUpgradeComponent(itemRequirementLevels[weaponGun.LevelIndex].ItemsRequirementComponent2))
            {
                if (weaponGun.WeaponComponentLevel[weaponGun.LevelIndex].FullUpgrade ||
                    weaponGun.WeaponComponentLevel[weaponGun.LevelIndex].UpgradeComponent2) return;
            
                switch (weaponGun.LevelIndex)
                {
                    case 0:
                        UnlockSkillComponent2Level1(weaponGun);
                        break;
                    case 1:
                        UnlockSkillComponent2Level2(weaponGun);
                        break;
                    case 2:
                        UnlockSkillComponent2Level3(weaponGun);
                        break;
                }
            
                weaponGun.UpgradeComponent(2);
                IncreaseStatus(weaponGun);
                isUpgradeSucceed = true;
                
                UpdateQuest2();
            }
        }
        
        [ContextMenu("Unlock Skill Component (3)")]
        public void UnlockSkillComponent3()
        {
            isUpgradeSucceed = false;
            
            BaseItem weaponGun = WeaponGun();
            
            if (IsCanUpgradeComponent(itemRequirementLevels[weaponGun.LevelIndex].ItemsRequirementComponent3))
            {
                if (weaponGun.WeaponComponentLevel[weaponGun.LevelIndex].FullUpgrade ||
                    weaponGun.WeaponComponentLevel[weaponGun.LevelIndex].UpgradeComponent3) return;
            
                switch (weaponGun.LevelIndex)
                {
                    case 0:
                        UnlockSkillComponent3Level1(weaponGun);
                        break;
                    case 1:
                        UnlockSkillComponent3Level2(weaponGun);
                        break;
                    case 2:
                        UnlockSkillComponent3Level3(weaponGun);
                        break;
                }
            
                weaponGun.UpgradeComponent(3);
                IncreaseStatus(weaponGun);
                isUpgradeSucceed = true;
                
                UpdateQuest2();
            }
        }

        [ContextMenu("Evolution")]
        public void Evolution()
        {
            BaseItem weaponGun = WeaponGun();
            if (!weaponGun.WeaponComponentLevel[weaponGun.LevelIndex].FullUpgrade) return;
            
            weaponGun.WeaponComponentLevel[weaponGun.LevelIndex].IsEvolution = true;
            playerData.Info.DecreaseStatus(weaponGun, playerData);
            weaponGun.UpLevel();
            IncreaseStatus(weaponGun);
            playerData.Info.IncreaseStatus(weaponGun, playerData);
            uiManager.SetVfxShowWeapon(weaponGun);
            
            UpdateQuest2();
        }
        
        public void ShowEvolution(Button evolutionButton)
        {
            BaseItem weaponGun = WeaponGun();

            if (weaponGun.WeaponComponentLevel[weaponGun.LevelIndex].FullUpgrade)
            {
                evolutionButton.interactable = true;
            }
        }

        public void CloseEvolution(Button evolutionButton)
        {
            evolutionButton.interactable = false;
        }

        public void CloseUpgradeButton(Button upgradeButton)
        {
            if (isUpgradeSucceed)
            {
                upgradeButton.interactable = false;
            }
        }

        public void ShowConfirmEvolutionPanel(RectTransform confirmEvolutionPanel)
        {
            confirmEvolutionPanel.gameObject.SetActive(true);
        }

        public void CloseConfirmEvolutionPanel(RectTransform confirmEvolutionPanel)
        {
            confirmEvolutionPanel.gameObject.SetActive(false);
        }

        private BaseItem WeaponGun()
        {
            return playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Gun);
        }
        
        private void IncreaseStatus(BaseItem weaponGun)
        {
            if (!weaponGun.IsUsedEquip) return;
            
            playerData.Info.GetData(playerData.Info);
        }
        
        private bool IsCanUpgradeComponent(List<ItemRequirementData> itemRequirementComponent)
        {
            List<BaseItem> items = new List<BaseItem>();
            List<bool> isCanUpgrade = new List<bool>();

            for (int i = 0; i < itemRequirementComponent.Count; i++)
            {
                items.Add(playerData.Info.Inventory.InventoryGeneral.Items.Find(x => x.Name[Index.Start] ==
                    itemRequirementComponent[i].ItemsName));
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Count >= itemRequirementComponent[i].Count)
                {
                    isCanUpgrade.Add(true);
                }
            }
            
            if (items.Count == isCanUpgrade.Count)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].DecreaseCount(itemRequirementComponent[i].Count);
                }
                
                return true;
            }
            
            return false;
        }

        #region COMPONENT_1

        /// <summary>
        /// Skill component 1 level 1.
        /// </summary>
        private void UnlockSkillComponent1Level1(BaseItem weaponGun)
        {
            int levelIndexWeaponData = 0;
            
            for (var i = 0; i < weaponGun.SpeedAttack.Length; i++)
            {
                weaponGun.SpeedAttack[i] += itemWeaponData.GunData.UnlockSpeedAttack[levelIndexWeaponData];
            }

            playerData.SetSpeedAnimation(playerData.PlayerAnimation.Animator, true);
            
            if (!weaponGun.IsUsedEquip) return;

            playerData.Info.Status.SpeedAttack +=
                itemWeaponData.SwordData.UnlockSpeedAttack[levelIndexWeaponData];
        }
        
        /// <summary>
        /// Skill component 1 level 2.
        /// </summary>
        private void UnlockSkillComponent1Level2(BaseItem weaponGun)
        {
            int levelIndexWeaponData = 1;
        }
        
        /// <summary>
        /// Skill component 1 level 3.
        /// </summary>
        private void UnlockSkillComponent1Level3(BaseItem weaponGun)
        {
            int levelIndexWeaponData = 2;
        }

        #endregion

        #region COMPONENT_2

        /// <summary>
        /// Skill component 2 level 1.
        /// </summary>
        private void UnlockSkillComponent2Level1(BaseItem weaponGun)
        {
            int levelIndexWeaponData = 0;
            
            for (var i = 0; i < weaponGun.AttackRange.Length; i++)
            {
                weaponGun.AttackRange[i] += itemWeaponData.GunData.UnlockAttackRange[levelIndexWeaponData];
            }
            
            if (!weaponGun.IsUsedEquip) return;

            playerData.Info.Status.AttackRange +=
                itemWeaponData.SwordData.UnlockAttackRange[levelIndexWeaponData];
        }
        
        /// <summary>
        /// Skill component 2 level 2.
        /// </summary>
        private void UnlockSkillComponent2Level2(BaseItem weaponGun)
        {
            int levelIndexWeaponData = 1;
        }
        
        /// <summary>
        /// Skill component 2 level 3.
        /// </summary>
        private void UnlockSkillComponent2Level3(BaseItem weaponGun)
        {
            int levelIndexWeaponData = 2;
        }

        #endregion
        
        #region COMPONENT_3

        /// <summary>
        /// Skill component 3 level 1.
        /// </summary>
        private void UnlockSkillComponent3Level1(BaseItem weaponGun)
        {
            int levelIndexWeaponData = 0;
            
            /*for (var i = 0; i < weaponGun.Atk.Length; i++)
            {
                weaponGun.Atk[i] += itemWeaponData.GunData.UnlockPowerAttack[levelIndexWeaponData];
            }*/

            int temp =
                (int)((weaponGun.Atk[0] * itemWeaponData.KnuckleData.UnlockPowerAttack[levelIndexWeaponData]) / 100);
            
            weaponGun.Atk[0] += temp;

            if (!weaponGun.IsUsedEquip) return;

            playerData.Info.Status.Atk += temp;
        }
        
        /// <summary>
        /// Skill component 3 level 2.
        /// </summary>
        private void UnlockSkillComponent3Level2(BaseItem weaponGun)
        {
            int levelIndexWeaponData = 1;
        }
        
        /// <summary>
        /// Skill component 3 level 3.
        /// </summary>
        private void UnlockSkillComponent3Level3(BaseItem weaponGun)
        {
            int levelIndexWeaponData = 2;
        }

        #endregion
        
#if UNITY_EDITOR
        
        [ContextMenu("Unlock Skill Component (1)")]
        public void UnlockSkillComponent1Editor()
        {
            isUpgradeSucceed = false;

            BaseItem weaponGun = WeaponGun();
            
            switch (weaponGun.LevelIndex)
            {
                case 0:
                    UnlockSkillComponent1Level1(weaponGun);
                    break;
                case 1:
                    UnlockSkillComponent1Level2(weaponGun);
                    break;
                case 2:
                    UnlockSkillComponent1Level3(weaponGun);
                    break;
            }

            weaponGun.UpgradeComponent(1);
            IncreaseStatus(weaponGun);
        }

        [ContextMenu("Unlock Skill Component (2)")]
        public void UnlockSkillComponent2Editor()
        {
            isUpgradeSucceed = false;

            BaseItem weaponGun = WeaponGun();
            
            switch (weaponGun.LevelIndex)
            {
                case 0:
                    UnlockSkillComponent2Level1(weaponGun);
                    break;
                case 1:
                    UnlockSkillComponent2Level2(weaponGun);
                    break;
                case 2:
                    UnlockSkillComponent2Level3(weaponGun);
                    break;
            }

            weaponGun.UpgradeComponent(2);
            IncreaseStatus(weaponGun);
        }

        [ContextMenu("Unlock Skill Component (3)")]
        public void UnlockSkillComponent3Editor()
        {
            BaseItem weaponGun = WeaponGun();
            
            switch (weaponGun.LevelIndex)
            {
                case 0:
                    UnlockSkillComponent3Level1(weaponGun);
                    break;
                case 1:
                    UnlockSkillComponent3Level2(weaponGun);
                    break;
                case 2:
                    UnlockSkillComponent3Level3(weaponGun);
                    break;
            }

            weaponGun.UpgradeComponent(3);
            IncreaseStatus(weaponGun);
        }

        [ContextMenu("Evolution")]
        public void EvolutionEditor()
        {
            BaseItem weaponGun = WeaponGun();
            
            weaponGun.WeaponComponentLevel[weaponGun.LevelIndex].IsEvolution = true;
            playerData.Info.DecreaseStatus(weaponGun, playerData);
            weaponGun.UpLevel();
            IncreaseStatus(weaponGun);
            playerData.Info.IncreaseStatus(weaponGun, playerData);
        }
        
#endif
    }
}