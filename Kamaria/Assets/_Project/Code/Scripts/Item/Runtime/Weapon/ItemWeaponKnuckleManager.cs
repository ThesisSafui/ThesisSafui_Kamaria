using System;
using System.Collections.Generic;
using Kamaria.Manager;
using Kamaria.Player.Data;
using Kamaria.Player.Data.Quest;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.Item.Weapon
{
    public sealed class ItemWeaponKnuckleManager : MonoBehaviour, IBaseItemWeaponManager
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
        
        public void UnlockSkillComponent1()
        {
            isUpgradeSucceed = false;
            
            BaseItem weaponKnuckle = WeaponKnuckle();
            
            if (IsCanUpgradeComponent(itemRequirementLevels[weaponKnuckle.LevelIndex].ItemsRequirementComponent1))
            {
                if (weaponKnuckle.WeaponComponentLevel[weaponKnuckle.LevelIndex].FullUpgrade ||
                    weaponKnuckle.WeaponComponentLevel[weaponKnuckle.LevelIndex].UpgradeComponent1) return;
            
                switch (weaponKnuckle.LevelIndex)
                {
                    case 0:
                        UnlockSkillComponent1Level1(weaponKnuckle);
                        break;
                    case 1:
                        UnlockSkillComponent1Level2(weaponKnuckle);
                        break;
                    case 2:
                        UnlockSkillComponent1Level3(weaponKnuckle);
                        break;
                }

                weaponKnuckle.UpgradeComponent(1);
                IncreaseStatus(weaponKnuckle);
                isUpgradeSucceed = true;

                UpdateQuest2();
            }
        }
        
        public void UnlockSkillComponent2()
        {
            isUpgradeSucceed = false;

            BaseItem weaponKnuckle = WeaponKnuckle();
            
            if (IsCanUpgradeComponent(itemRequirementLevels[weaponKnuckle.LevelIndex].ItemsRequirementComponent2))
            {
                if (weaponKnuckle.WeaponComponentLevel[weaponKnuckle.LevelIndex].FullUpgrade ||
                    weaponKnuckle.WeaponComponentLevel[weaponKnuckle.LevelIndex].UpgradeComponent2) return;
            
                switch (weaponKnuckle.LevelIndex)
                {
                    case 0:
                        UnlockSkillComponent2Level1(weaponKnuckle);
                        break;
                    case 1:
                        UnlockSkillComponent2Level2(weaponKnuckle);
                        break;
                    case 2:
                        UnlockSkillComponent2Level3(weaponKnuckle);
                        break;
                }
            
                weaponKnuckle.UpgradeComponent(2);
                IncreaseStatus(weaponKnuckle);
                isUpgradeSucceed = true;
                
                UpdateQuest2();
            }
        }
        
        public void UnlockSkillComponent3()
        {
            isUpgradeSucceed = false;
            
            BaseItem weaponKnuckle = WeaponKnuckle();
            
            if (IsCanUpgradeComponent(itemRequirementLevels[weaponKnuckle.LevelIndex].ItemsRequirementComponent3))
            {
                if (weaponKnuckle.WeaponComponentLevel[weaponKnuckle.LevelIndex].FullUpgrade ||
                    weaponKnuckle.WeaponComponentLevel[weaponKnuckle.LevelIndex].UpgradeComponent3) return;

                switch (weaponKnuckle.LevelIndex)
                {
                    case 0:
                        UnlockSkillComponent3Level1(weaponKnuckle);
                        break;
                    case 1:
                        UnlockSkillComponent3Level2(weaponKnuckle);
                        break;
                    case 2:
                        UnlockSkillComponent3Level3(weaponKnuckle);
                        break;
                }
            
                weaponKnuckle.UpgradeComponent(3);
                IncreaseStatus(weaponKnuckle);
                isUpgradeSucceed = true;
                
                UpdateQuest2();
            }
        }

        [ContextMenu("Evolution")]
        public void Evolution()
        {
            BaseItem weaponKnuckle = WeaponKnuckle();
            if (!weaponKnuckle.WeaponComponentLevel[weaponKnuckle.LevelIndex].FullUpgrade) return;

            weaponKnuckle.WeaponComponentLevel[weaponKnuckle.LevelIndex].IsEvolution = true;
            playerData.Info.DecreaseStatus(weaponKnuckle, playerData);
            weaponKnuckle.UpLevel();
            IncreaseStatus(weaponKnuckle);
            playerData.Info.IncreaseStatus(weaponKnuckle, playerData);
            uiManager.SetVfxShowWeapon(weaponKnuckle);
            
            UpdateQuest2();
        }
        
        public void ShowEvolution(Button evolutionButton)
        {
            BaseItem weaponKnuckle = WeaponKnuckle();

            if (weaponKnuckle.WeaponComponentLevel[weaponKnuckle.LevelIndex].FullUpgrade)
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

        private BaseItem WeaponKnuckle()
        {
            return playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Knuckle);
        }
        
        private void IncreaseStatus(BaseItem weaponKnuckle)
        {
            if (!weaponKnuckle.IsUsedEquip) return;
            
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
        private void UnlockSkillComponent1Level1(BaseItem weaponKnuckle)
        {
            int levelIndexWeaponData = 0;

            for (var i = 0; i < weaponKnuckle.MaxHealth.Length; i++)
            {
                weaponKnuckle.MaxHealth[i] += itemWeaponData.KnuckleData.UnlockMaxHealth[levelIndexWeaponData];
            }
            
            if (!weaponKnuckle.IsUsedEquip) return;

            playerData.Info.Status.MaxHealth +=
                itemWeaponData.SwordData.UnlockMaxHealth[levelIndexWeaponData];
        }
        
        /// <summary>
        /// Skill component 1 level 2.
        /// </summary>
        private void UnlockSkillComponent1Level2(BaseItem weaponKnuckle)
        {
            int levelIndexWeaponData = 1;
        }
        
        /// <summary>
        /// Skill component 1 level 3.
        /// </summary>
        private void UnlockSkillComponent1Level3(BaseItem weaponKnuckle)
        {
            int levelIndexWeaponData = 2;
        }

        #endregion

        #region COMPONENT_2

        /// <summary>
        /// Skill component 2 level 1.
        /// </summary>
        private void UnlockSkillComponent2Level1(BaseItem weaponKnuckle)
        {
            int levelIndexWeaponData = 0;
            
            for (var i = 0; i < weaponKnuckle.ReductionDamage.Length; i++)
            {
                weaponKnuckle.ReductionDamage[i] += itemWeaponData.KnuckleData.UnlockReductionDamage[levelIndexWeaponData];
            }
            
            if (!weaponKnuckle.IsUsedEquip) return;

            playerData.Info.Status.ReductionDamage +=
                itemWeaponData.SwordData.UnlockReductionDamage[levelIndexWeaponData];
        }
        
        /// <summary>
        /// Skill component 2 level 2.
        /// </summary>
        private void UnlockSkillComponent2Level2(BaseItem weaponKnuckle)
        {
            int levelIndexWeaponData = 1;
        }
        
        /// <summary>
        /// Skill component 2 level 3.
        /// </summary>
        private void UnlockSkillComponent2Level3(BaseItem weaponKnuckle)
        {
            int levelIndexWeaponData = 2;
        }

        #endregion
        
        #region COMPONENT_3

        /// <summary>
        /// Skill component 3 level 1.
        /// </summary>
        private void UnlockSkillComponent3Level1(BaseItem weaponKnuckle)
        {
            int levelIndexWeaponData = 0;

            /*for (var i = 0; i < weaponKnuckle.Atk.Length; i++)
            {
                weaponKnuckle.Atk[i] += itemWeaponData.KnuckleData.UnlockPowerAttack[levelIndexWeaponData];
            }*/

            int temp =
                (int)((weaponKnuckle.Atk[0] * itemWeaponData.KnuckleData.UnlockPowerAttack[levelIndexWeaponData]) / 100);
            
            weaponKnuckle.Atk[0] += temp;
            
            if (!weaponKnuckle.IsUsedEquip) return;

            playerData.Info.Status.Atk += temp;
        }
        
        /// <summary>
        /// Skill component 3 level 2.
        /// </summary>
        private void UnlockSkillComponent3Level2(BaseItem weaponKnuckle)
        {
            int levelIndexWeaponData = 1;
        }
        
        /// <summary>
        /// Skill component 3 level 3.
        /// </summary>
        private void UnlockSkillComponent3Level3(BaseItem weaponKnuckle)
        {
            int levelIndexWeaponData = 2;
        }

        #endregion
        
#if UNITY_EDITOR
        
        [ContextMenu("Unlock Skill Component (1)")]
        public void UnlockSkillComponent1Editor()
        {
            isUpgradeSucceed = false;

            BaseItem weaponKnuckle = WeaponKnuckle();
            
            switch (weaponKnuckle.LevelIndex)
            {
                case 0:
                    UnlockSkillComponent1Level1(weaponKnuckle);
                    break;
                case 1:
                    UnlockSkillComponent1Level2(weaponKnuckle);
                    break;
                case 2:
                    UnlockSkillComponent1Level3(weaponKnuckle);
                    break;
            }

            weaponKnuckle.UpgradeComponent(1);
            IncreaseStatus(weaponKnuckle);
        }

        [ContextMenu("Unlock Skill Component (2)")]
        public void UnlockSkillComponent2Editor()
        {
            isUpgradeSucceed = false;

            BaseItem weaponKnuckle = WeaponKnuckle();
            
            switch (weaponKnuckle.LevelIndex)
            {
                case 0:
                    UnlockSkillComponent2Level1(weaponKnuckle);
                    break;
                case 1:
                    UnlockSkillComponent2Level2(weaponKnuckle);
                    break;
                case 2:
                    UnlockSkillComponent2Level3(weaponKnuckle);
                    break;
            }

            weaponKnuckle.UpgradeComponent(2);
            IncreaseStatus(weaponKnuckle);
        }

        [ContextMenu("Unlock Skill Component (3)")]
        public void UnlockSkillComponent3Editor()
        {
            BaseItem weaponKnuckle = WeaponKnuckle();
            
            switch (weaponKnuckle.LevelIndex)
            {
                case 0:
                    UnlockSkillComponent3Level1(weaponKnuckle);
                    break;
                case 1:
                    UnlockSkillComponent3Level2(weaponKnuckle);
                    break;
                case 2:
                    UnlockSkillComponent3Level3(weaponKnuckle);
                    break;
            }

            weaponKnuckle.UpgradeComponent(3);
            IncreaseStatus(weaponKnuckle);
        }

        [ContextMenu("Evolution")]
        public void EvolutionEditor()
        {
            BaseItem weaponKnuckle = WeaponKnuckle();

            weaponKnuckle.WeaponComponentLevel[weaponKnuckle.LevelIndex].IsEvolution = true;
            playerData.Info.DecreaseStatus(weaponKnuckle, playerData);
            weaponKnuckle.UpLevel();
            IncreaseStatus(weaponKnuckle);
            playerData.Info.IncreaseStatus(weaponKnuckle, playerData);
        }
        
#endif
        
    }
}