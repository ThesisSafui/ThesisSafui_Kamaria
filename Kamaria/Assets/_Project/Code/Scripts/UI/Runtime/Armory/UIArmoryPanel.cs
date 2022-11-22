using System;
using System.Collections.Generic;
using System.Linq;
using Kamaria.Item;
using Kamaria.Item.Weapon;
using Kamaria.Player.Data;
using Kamaria.Utilities.GameEvent;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Kamaria.UI.Armory
{
    public enum PanelUpgradeLevels
    {
        LV1,
        LV2,
        LV3
    }

    public enum NumberComponents
    {
        Component1,
        Component2,
        Component3
    }
    
    [Serializable]
    public sealed class UIArmoryPanel : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private UIItemRequirement prefabItemRequirement;
        [SerializeField] private Button lv1Button;
        [SerializeField] private Button lv2Button;
        [SerializeField] private Button lv3Button;
        [SerializeField] private Button keyStoneButton;
        [SerializeField] private Button upgradeButton;
        [SerializeField] private GameObject weaponManager;
        [SerializeField] private RectTransform keyStonePanel;
        [SerializeField] private RectTransform upgradePanel;
        [SerializeField] private RectTransform levelButtonPanel;
        [SerializeField] private RectTransform panel; 
        [SerializeField] private PanelName namePanel;
        [SerializeField] private List<UIArmoryPartUpgradeLevel> uiPartUpgradesLevels;

        public RectTransform Panel => panel;
        public PanelName NamePanel => namePanel;
        public List<UIArmoryPartUpgradeLevel> UIPartUpgradesLevels => uiPartUpgradesLevels;

        private BaseItem item;
        
        private void OnEnable()
        {
            SetUIUpgradeDefault();
            
            lv1Button.onClick.AddListener(Lv1ButtonClicked);
            lv2Button.onClick.AddListener(Lv2ButtonClicked);
            lv3Button.onClick.AddListener(Lv3ButtonClicked);
            keyStoneButton.onClick.AddListener(KeyStoneClicked);
            upgradeButton.onClick.AddListener(UpgradeClicked);
            
            for (int i = 0; i < uiPartUpgradesLevels.Count; i++)
            {
                if (uiPartUpgradesLevels[i].Level != PanelUpgradeLevels.LV3)
                {
                    uiPartUpgradesLevels[i].EvolutionButton.onClick.AddListener(ClickedEvolution(i));
                    uiPartUpgradesLevels[i].ConfirmEvolutionButton.onClick.AddListener(ConfirmEvolution(i));
                    uiPartUpgradesLevels[i].CancelEvolutionButton.onClick.AddListener(CancelEvolution(i));
                }

                for (int j = 0; j < uiPartUpgradesLevels[i].Components.Count; j++)
                {
                    uiPartUpgradesLevels[i].Components[j].Button.onClick.AddListener(UpgradeComponent(i, j));
                }
            }

            SetUI();
        }

        private void OnDisable()
        {
            lv1Button.onClick.RemoveListener(Lv1ButtonClicked);
            lv2Button.onClick.RemoveListener(Lv2ButtonClicked);
            lv3Button.onClick.RemoveListener(Lv3ButtonClicked);
            keyStoneButton.onClick.RemoveListener(KeyStoneClicked);
            upgradeButton.onClick.RemoveListener(UpgradeClicked);
            
            for (int i = 0; i < uiPartUpgradesLevels.Count; i++)
            {
                if (uiPartUpgradesLevels[i].Level != PanelUpgradeLevels.LV3)
                {
                    uiPartUpgradesLevels[i].EvolutionButton.onClick.RemoveListener(ClickedEvolution(i));
                    uiPartUpgradesLevels[i].ConfirmEvolutionButton.onClick.RemoveListener(ConfirmEvolution(i));
                    uiPartUpgradesLevels[i].CancelEvolutionButton.onClick.RemoveListener(CancelEvolution(i));
                }

                for (int j = 0; j < uiPartUpgradesLevels[i].Components.Count; j++)
                {
                    uiPartUpgradesLevels[i].Components[j].Button.onClick.RemoveListener(UpgradeComponent(i, j));
                }
            }
        }

        private UnityAction ConfirmEvolution(int indexUIPartUpgradesLevel)
        {
            return uiPartUpgradesLevels[indexUIPartUpgradesLevel].EventConfirmEvolution.TriggerEvent;
        }
        
        private UnityAction CancelEvolution(int indexUIPartUpgradesLevel)
        {
            return uiPartUpgradesLevels[indexUIPartUpgradesLevel].EventCancelEvolution.TriggerEvent;
        }

        private UnityAction ClickedEvolution(int indexUIPartUpgradesLevel)
        {
            return uiPartUpgradesLevels[indexUIPartUpgradesLevel].EventClickedEvolution.TriggerEvent;
        }

        private UnityAction UpgradeComponent(int indexUIPartUpgradesLevel, int indexComponent)
        {
            return uiPartUpgradesLevels[indexUIPartUpgradesLevel].Components[indexComponent].EventUpgradeComponent
                .TriggerEvent;
        }
        
        private void Lv1ButtonClicked()
        {
            lv1Button.interactable = false;
            lv2Button.interactable = true;
            lv3Button.interactable = true;
            
            ShowUpgradesLevelsPanel(PanelUpgradeLevels.LV1);
        }
        
        private void Lv2ButtonClicked()
        {
            lv1Button.interactable = true;
            lv2Button.interactable = false;
            lv3Button.interactable = true;
            
            ShowUpgradesLevelsPanel(PanelUpgradeLevels.LV2);
        }
        
        private void Lv3ButtonClicked()
        {
            lv1Button.interactable = true;
            lv2Button.interactable = true;
            lv3Button.interactable = false;
            
            ShowUpgradesLevelsPanel(PanelUpgradeLevels.LV3);
        }
        
        private void KeyStoneClicked()
        {
            keyStoneButton.interactable = false;
            upgradeButton.interactable = true;
            upgradePanel.gameObject.SetActive(false);
            keyStonePanel.gameObject.SetActive(true);
            levelButtonPanel.gameObject.SetActive(false);
        }
        
        private void UpgradeClicked()
        {
            keyStoneButton.interactable = true;
            upgradeButton.interactable = false;
            upgradePanel.gameObject.SetActive(true);
            keyStonePanel.gameObject.SetActive(false);
            levelButtonPanel.gameObject.SetActive(true);
        }

        private void SetUIUpgradeDefault()
        {
            lv1Button.interactable = false;
            lv2Button.interactable = true;
            lv3Button.interactable = true;
            keyStoneButton.interactable = true;
            upgradeButton.interactable = false;

            ShowUpgradesLevelsPanel(PanelUpgradeLevels.LV1);
            upgradePanel.gameObject.SetActive(true);
            keyStonePanel.gameObject.SetActive(false);
            levelButtonPanel.gameObject.SetActive(true);
        }

        private void ShowUpgradesLevelsPanel(PanelUpgradeLevels panelUpgradeLevels)
        {
            for (int i = 0; i < uiPartUpgradesLevels.Count; i++)
            {
                uiPartUpgradesLevels[i].PanelLevel.gameObject
                    .SetActive(uiPartUpgradesLevels[i].Level == panelUpgradeLevels);
            }
        }
        
        public void SetUI()
        {
            if (namePanel == PanelName.SwordPanel)
            {
                item =
                    playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Sword);

                ItemWeaponSwordManager swordManager = weaponManager.GetComponent<ItemWeaponSwordManager>();
                
                for (int i = 0; i < item.MaxLevel; i++)
                {
                    SetItemRequirement(item, swordManager.ItemRequirementLevels[i].ItemsRequirementComponent1,
                        NumberComponents.Component1, uiPartUpgradesLevels[i].Level);
                    SetItemRequirement(item, swordManager.ItemRequirementLevels[i].ItemsRequirementComponent2,
                        NumberComponents.Component2, uiPartUpgradesLevels[i].Level);
                    SetItemRequirement(item, swordManager.ItemRequirementLevels[i].ItemsRequirementComponent3,
                        NumberComponents.Component3, uiPartUpgradesLevels[i].Level);
                }
            }
            else if (namePanel == PanelName.GunPanel)
            {
                item =
                    playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Gun);

                ItemWeaponGunManager gunManager = weaponManager.GetComponent<ItemWeaponGunManager>();
                
                for (int i = 0; i < item.MaxLevel; i++)
                {
                    SetItemRequirement(item, gunManager.ItemRequirementLevels[i].ItemsRequirementComponent1,
                        NumberComponents.Component1, uiPartUpgradesLevels[i].Level);
                    SetItemRequirement(item, gunManager.ItemRequirementLevels[i].ItemsRequirementComponent2,
                        NumberComponents.Component2, uiPartUpgradesLevels[i].Level);
                    SetItemRequirement(item, gunManager.ItemRequirementLevels[i].ItemsRequirementComponent3,
                        NumberComponents.Component3, uiPartUpgradesLevels[i].Level);
                }
            }
            else if (namePanel == PanelName.KnucklePanel)
            {
                item =
                    playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Knuckle);

                ItemWeaponKnuckleManager knuckleManager = weaponManager.GetComponent<ItemWeaponKnuckleManager>();
                
                for (int i = 0; i < item.MaxLevel; i++)
                {
                    SetItemRequirement(item, knuckleManager.ItemRequirementLevels[i].ItemsRequirementComponent1,
                        NumberComponents.Component1, uiPartUpgradesLevels[i].Level);
                    SetItemRequirement(item, knuckleManager.ItemRequirementLevels[i].ItemsRequirementComponent2,
                        NumberComponents.Component2, uiPartUpgradesLevels[i].Level);
                    SetItemRequirement(item, knuckleManager.ItemRequirementLevels[i].ItemsRequirementComponent3,
                        NumberComponents.Component3, uiPartUpgradesLevels[i].Level);
                }
            }
            else if (namePanel == PanelName.GlassPanel)
            {
                
            }
            else if (namePanel == PanelName.BanglePanel)
            {
                
            }
            
            SetButtonWeapon();
        }
        
        private void SetItemRequirement(BaseItem baseItem , List<ItemRequirementData> itemRequirementComponent,
            NumberComponents numberComponents, PanelUpgradeLevels panelUpgradeLevels)
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

            for (int i = 0; i < uiPartUpgradesLevels.Count; i++)
            {
                for (int j = 0; j < uiPartUpgradesLevels[i].Components.Count; j++)
                {
                    if (uiPartUpgradesLevels[i].Level != panelUpgradeLevels) continue;
                    if (uiPartUpgradesLevels[i].Components[j].Number != numberComponents) continue;
                    
                    if (uiPartUpgradesLevels[i].Components[j].RequirementPanel.transform.childCount > 0)
                    {
                        for (int k = 0; k < uiPartUpgradesLevels[i].Components[j].RequirementPanel.transform.childCount; k++)
                        {
                            Destroy(uiPartUpgradesLevels[i].Components[j].RequirementPanel
                                .GetComponentsInChildren<UIItemRequirement>()[k].gameObject);
                        }
                    }
                            
                    for (int k = 0; k < items.Count; k++)
                    {
                        var itemRequirement = Instantiate(prefabItemRequirement,
                            uiPartUpgradesLevels[i].Components[j].RequirementPanel);

                        string itemName = $"{items[k].Name[Index.Start].ToString()}";
                        itemName = string.Concat(itemName.Select(x => Char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
                        
                        string itemCount = $"{items[k].Count} / {itemRequirementComponent[k].Count}";

                        itemRequirement.Init(items[k].Image[Index.Start], itemName, itemCount,
                            items.Count == isCanUpgrade.Count);
                    }
                }
            }
        }
        
        private void SetButtonWeapon()
        {
            for (int i = 0; i < item.WeaponComponentLevel.Length; i++)
            {
                if (item.WeaponComponentLevel[i].IsEvolution)
                {
                    for (int j = 0; j < uiPartUpgradesLevels.Count; j++)
                    {
                        if (uiPartUpgradesLevels[j].Level != PanelUpgradeLevels.LV3)
                        {
                            uiPartUpgradesLevels[j].EvolutionButton.interactable = false;
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < uiPartUpgradesLevels.Count; j++)
                    {
                        if (uiPartUpgradesLevels[j].Level != PanelUpgradeLevels.LV3)
                        {
                            if (item.WeaponComponentLevel[j].FullUpgrade && !item.WeaponComponentLevel[j].IsEvolution)
                            {
                                uiPartUpgradesLevels[j].EvolutionButton.interactable = true;
                            }
                            else
                            {
                                uiPartUpgradesLevels[j].EvolutionButton.interactable = false;
                            }
                        }
                    }
                }
                
                if (item.LevelIndex < i)
                {
                    CloseButtonLvComponent(i, NumberComponents.Component1);
                    CloseButtonLvComponent(i, NumberComponents.Component2);
                    CloseButtonLvComponent(i, NumberComponents.Component3);
                }
                else if(item.LevelIndex == i)
                {
                    ShowButtonLvComponent(i, NumberComponents.Component1);
                    ShowButtonLvComponent(i, NumberComponents.Component2);
                    ShowButtonLvComponent(i, NumberComponents.Component3);
                }

                if (item.WeaponComponentLevel[i].UpgradeComponent1)
                {
                    CloseButtonComponent(NumberComponents.Component1);
                }

                if (item.WeaponComponentLevel[i].UpgradeComponent2)
                {
                    CloseButtonComponent(NumberComponents.Component2);
                }

                if (item.WeaponComponentLevel[i].UpgradeComponent3)
                {
                    CloseButtonComponent(NumberComponents.Component3);
                }
            }
        }
        
        private void CloseButtonComponent(NumberComponents component)
        {
            for (int j = 0; j < uiPartUpgradesLevels.Count; j++)
            {
                for (int k = 0; k < uiPartUpgradesLevels[j].Components.Count; k++)
                {
                    if (uiPartUpgradesLevels[j].Components[k].Number == component)
                    {
                        uiPartUpgradesLevels[j].Components[k].Button.interactable = false;
                    }
                }
            }
        }
        
        private void CloseButtonLvComponent(int level ,NumberComponents component)
        {
            for (int i = 0; i < uiPartUpgradesLevels[level].Components.Count; i++)
            {
                if (uiPartUpgradesLevels[level].Components[i].Number == component)
                { 
                    uiPartUpgradesLevels[level].Components[i].Button.interactable = false;
                }
            }
        }
        
        private void ShowButtonLvComponent(int level ,NumberComponents component)
        {
            for (int i = 0; i < uiPartUpgradesLevels[level].Components.Count; i++)
            {
                if (uiPartUpgradesLevels[level].Components[i].Number == component)
                { 
                    uiPartUpgradesLevels[level].Components[i].Button.interactable = true;
                }
            }
        }
    }

    [Serializable]
    public sealed class UIArmoryPartUpgradeLevel
    {
        [SerializeField] private Button evolutionButton;
        [SerializeField] private Button confirmEvolutionButton;
        [SerializeField] private Button cancelEvolutionButton;
        [SerializeField] private GameEventSO eventClickedEvolution;
        [SerializeField] private GameEventSO eventConfirmEvolution;
        [SerializeField] private GameEventSO eventCancelEvolution;
        [SerializeField] private RectTransform panelLevel;
        [SerializeField] private RectTransform panelPart;
        [SerializeField] private PanelUpgradeLevels level;
        [SerializeField] private List<UIArmoryPartUpgradeComponent> components;

        public RectTransform PanelLevel => panelLevel;
        public RectTransform PanelPart => panelPart;
        public PanelUpgradeLevels Level => level;
        public List<UIArmoryPartUpgradeComponent> Components => components;
        public Button EvolutionButton => evolutionButton;
        public Button ConfirmEvolutionButton => confirmEvolutionButton;
        public Button CancelEvolutionButton => cancelEvolutionButton;
        public GameEventSO EventClickedEvolution => eventClickedEvolution;
        public GameEventSO EventConfirmEvolution => eventConfirmEvolution;
        public GameEventSO EventCancelEvolution => eventCancelEvolution;
    }

    [Serializable]
    public sealed class UIArmoryPartUpgradeComponent
    {
        [SerializeField] private RectTransform requirementPanel;
        [SerializeField] private GameEventSO eventUpgradeComponent;
        [SerializeField] private NumberComponents number;
        [SerializeField] private RectTransform panel;
        [SerializeField] private Button button;

        public RectTransform RequirementPanel => requirementPanel;
        public GameEventSO EventUpgradeComponent => eventUpgradeComponent;
        public NumberComponents Number => number;
        public RectTransform Panel => panel;
        public Button Button => button;
    }
}