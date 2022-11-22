using System;
using System.Collections.Generic;
using Kamaria.Manager;
using Kamaria.Player.Data;
using Kamaria.Player.Data.Quest;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.Item.Weapon
{
    public sealed class ItemWeaponSwordManager : MonoBehaviour, IBaseItemWeaponManager
    {
        [Header("Referent info")] 
        [SerializeField] private QuestManagerSO questManagerSO;
        [SerializeField] private PlayerDataSO playerData;
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

            BaseItem weaponSword = WeaponSword();
            
            if (IsCanUpgradeComponent(itemRequirementLevels[weaponSword.LevelIndex].ItemsRequirementComponent1))
            {
                if (weaponSword.WeaponComponentLevel[weaponSword.LevelIndex].FullUpgrade ||
                    weaponSword.WeaponComponentLevel[weaponSword.LevelIndex].UpgradeComponent1) return;

                switch (weaponSword.LevelIndex)
                {
                    case 0:
                        UnlockSkillComponent1Level1(weaponSword);
                        break;
                    case 1:
                        UnlockSkillComponent1Level2(weaponSword);
                        break;
                    case 2:
                        UnlockSkillComponent1Level3(weaponSword);
                        break;
                }

                weaponSword.UpgradeComponent(1);
                IncreaseStatus(weaponSword);
                isUpgradeSucceed = true;

                UpdateQuest2();
            }
        }

        public void UnlockSkillComponent2()
        {
            isUpgradeSucceed = false;

            BaseItem weaponSword = WeaponSword();
            
            if (IsCanUpgradeComponent(itemRequirementLevels[weaponSword.LevelIndex].ItemsRequirementComponent2))
            {
                if (weaponSword.WeaponComponentLevel[weaponSword.LevelIndex].FullUpgrade ||
                    weaponSword.WeaponComponentLevel[weaponSword.LevelIndex].UpgradeComponent2) return;

                switch (weaponSword.LevelIndex)
                {
                    case 0:
                        UnlockSkillComponent2Level1(weaponSword);
                        break;
                    case 1:
                        UnlockSkillComponent2Level2(weaponSword);
                        break;
                    case 2:
                        UnlockSkillComponent2Level3(weaponSword);
                        break;
                }

                weaponSword.UpgradeComponent(2);
                IncreaseStatus(weaponSword);
                isUpgradeSucceed = true;
                
                UpdateQuest2();
            }
        }
        
        public void UnlockSkillComponent3()
        {
            isUpgradeSucceed = false;

            BaseItem weaponSword = WeaponSword();
            
            if (IsCanUpgradeComponent(itemRequirementLevels[weaponSword.LevelIndex].ItemsRequirementComponent3))
            {
                if (weaponSword.WeaponComponentLevel[weaponSword.LevelIndex].FullUpgrade ||
                    weaponSword.WeaponComponentLevel[weaponSword.LevelIndex].UpgradeComponent3) return;

                switch (weaponSword.LevelIndex)
                {
                    case 0:
                        UnlockSkillComponent3Level1(weaponSword);
                        break;
                    case 1:
                        UnlockSkillComponent3Level2(weaponSword);
                        break;
                    case 2:
                        UnlockSkillComponent3Level3(weaponSword);
                        break;
                }

                weaponSword.UpgradeComponent(3);
                IncreaseStatus(weaponSword);
                isUpgradeSucceed = true;
                
                UpdateQuest2();
            }
        }
        
        public void Evolution()
        {
            BaseItem weaponSword = WeaponSword();
            if (!weaponSword.WeaponComponentLevel[weaponSword.LevelIndex].FullUpgrade) return;

            weaponSword.WeaponComponentLevel[weaponSword.LevelIndex].IsEvolution = true;
            playerData.Info.DecreaseStatus(weaponSword, playerData);
            weaponSword.UpLevel();
            IncreaseStatus(weaponSword);
            playerData.Info.IncreaseStatus(weaponSword, playerData);
            uiManager.SetVfxShowWeapon(weaponSword);
            
            UpdateQuest2();
        }

        public void ShowEvolution(Button evolutionButton)
        {
            BaseItem weaponSword = WeaponSword();

            if (weaponSword.WeaponComponentLevel[weaponSword.LevelIndex].FullUpgrade)
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

        private BaseItem WeaponSword()
        {
            return playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Sword);
        }

        private void IncreaseStatus(BaseItem weaponSword)
        {
            if (!weaponSword.IsUsedEquip) return;
            
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
        private void UnlockSkillComponent1Level1(BaseItem weaponSword)
        {
            int levelIndexWeaponData = 0;

            for (var i = 0; i < weaponSword.SpeedAttack.Length; i++)
            {
                weaponSword.SpeedAttack[i] += itemWeaponData.SwordData.UnlockSpeedAttack[levelIndexWeaponData];
            }

            playerData.SetSpeedAnimation(playerData.PlayerAnimation.Animator, true);

            if (!weaponSword.IsUsedEquip) return;

            playerData.Info.Status.SpeedAttack +=
                itemWeaponData.SwordData.UnlockSpeedAttack[levelIndexWeaponData];
        }

        /// <summary>
        /// Skill component 1 level 2.
        /// </summary>
        private void UnlockSkillComponent1Level2(BaseItem weaponSword)
        {
            int levelIndexWeaponData = 1;
        }

        /// <summary>
        /// Skill component 1 level 3.
        /// </summary>
        private void UnlockSkillComponent1Level3(BaseItem weaponSword)
        {
            int levelIndexWeaponData = 2;
        }

        #endregion

        #region COMPONENT_2

        /// <summary>
        /// Skill component 2 level 1.
        /// </summary>
        private void UnlockSkillComponent2Level1(BaseItem weaponSword)
        {
            int levelIndexWeaponData = 0;

            for (var i = 0; i < weaponSword.IncreaseDashAcceleration.Length; i++)
            {
                weaponSword.IncreaseDashAcceleration[i] += itemWeaponData.SwordData.UnlockDashAcceleration[levelIndexWeaponData];
            }
            
            if (!weaponSword.IsUsedEquip) return;

            playerData.CharacterControllerData.DashAcceleration +=
                itemWeaponData.SwordData.UnlockDashAcceleration[levelIndexWeaponData];
        }

        /// <summary>
        /// Skill component 2 level 2.
        /// </summary>
        private void UnlockSkillComponent2Level2(BaseItem weaponSword)
        {
            int levelIndexWeaponData = 1;
        }

        /// <summary>
        /// Skill component 2 level 3.
        /// </summary>
        private void UnlockSkillComponent2Level3(BaseItem weaponSword)
        {
            int levelIndexWeaponData = 2;
        }

        #endregion

        #region COMPONENT_3

        /// <summary>
        /// Skill component 3 level 1.
        /// </summary>
        private void UnlockSkillComponent3Level1(BaseItem weaponSword)
        {
            int levelIndexWeaponData = 0;

            /*for (var i = 0; i < weaponSword.Atk.Length; i++)
            {
                weaponSword.Atk[i] += itemWeaponData.SwordData.UnlockPowerAttack[levelIndexWeaponData];
            }*/
            
            int temp =
                (int)((weaponSword.Atk[0] * itemWeaponData.KnuckleData.UnlockPowerAttack[levelIndexWeaponData]) / 100);
            
            weaponSword.Atk[0] += temp;
            
            if (!weaponSword.IsUsedEquip) return;

            playerData.Info.Status.Atk += temp;
        }

        /// <summary>
        /// Skill component 3 level 2.
        /// </summary>
        private void UnlockSkillComponent3Level2(BaseItem weaponSword)
        {
            int levelIndexWeaponData = 1;

            for (int i = 0; i < weaponSword.MaxCombo.Length; i++)
            {
                if (i < weaponSword.LevelIndex)
                {
                    continue;
                }
                weaponSword.MaxCombo[i] = itemWeaponData.SwordData.UnlockMaxCombo;
            }
            
            if (!weaponSword.IsUsedEquip) return;

            playerData.Info.Status.MaxCombo = itemWeaponData.SwordData.UnlockMaxCombo;
        }

        /// <summary>
        /// Skill component 3 level 3.
        /// </summary>
        private void UnlockSkillComponent3Level3(BaseItem weaponSword)
        {
            int levelIndex = 2;
            
        }

        #endregion

#if UNITY_EDITOR
        
        [ContextMenu("Unlock Skill Component (1)")]
        public void UnlockSkillComponent1Editor()
        {
            isUpgradeSucceed = false;

            BaseItem weaponSword = WeaponSword();
            
            switch (weaponSword.LevelIndex)
            {
                case 0:
                    UnlockSkillComponent1Level1(weaponSword);
                    break;
                case 1:
                    UnlockSkillComponent1Level2(weaponSword);
                    break;
                case 2:
                    UnlockSkillComponent1Level3(weaponSword);
                    break;
            }

            weaponSword.UpgradeComponent(1);
            IncreaseStatus(weaponSword);
        }

        [ContextMenu("Unlock Skill Component (2)")]
        public void UnlockSkillComponent2Editor()
        {
            isUpgradeSucceed = false;

            BaseItem weaponSword = WeaponSword();
            
            switch (weaponSword.LevelIndex)
            {
                case 0:
                    UnlockSkillComponent2Level1(weaponSword);
                    break;
                case 1:
                    UnlockSkillComponent2Level2(weaponSword);
                    break;
                case 2:
                    UnlockSkillComponent2Level3(weaponSword);
                    break;
            }

            weaponSword.UpgradeComponent(2);
            IncreaseStatus(weaponSword);
        }

        [ContextMenu("Unlock Skill Component (3)")]
        public void UnlockSkillComponent3Editor()
        {
            BaseItem weaponSword = WeaponSword();
            
            switch (weaponSword.LevelIndex)
            {
                case 0:
                    UnlockSkillComponent3Level1(weaponSword);
                    break;
                case 1:
                    UnlockSkillComponent3Level2(weaponSword);
                    break;
                case 2:
                    UnlockSkillComponent3Level3(weaponSword);
                    break;
            }

            weaponSword.UpgradeComponent(3);
            IncreaseStatus(weaponSword);
        }

        [ContextMenu("Evolution")]
        public void EvolutionEditor()
        {
            BaseItem weaponSword = WeaponSword();

            weaponSword.WeaponComponentLevel[weaponSword.LevelIndex].IsEvolution = true;
            playerData.Info.DecreaseStatus(weaponSword, playerData);
            weaponSword.UpLevel();
            IncreaseStatus(weaponSword);
            playerData.Info.IncreaseStatus(weaponSword, playerData);
        }
        
#endif
    }
}