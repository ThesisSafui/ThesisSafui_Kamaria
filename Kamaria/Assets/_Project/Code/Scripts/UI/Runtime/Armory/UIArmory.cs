using System.Collections.Generic;
using Kamaria.Item;
using Kamaria.Manager;
using Kamaria.Player.Data;
using Kamaria.UI.UIMainGame;
using Kamaria.VFX_ALL;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI.Armory
{
    public enum PanelName
    {
        GlassPanel,
        BanglePanel,
        SwordPanel,
        GunPanel,
        KnucklePanel
    }
    
    public sealed class UIArmory : MonoBehaviour
    {
        [SerializeField] private UIGamePlay uiGamePlay;
        [SerializeField] private Button exitButton;
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private UIArmorySO uiArmory;
        [SerializeField] private UIItemMenuArmory startMenuArmory;
        [SerializeField] private List<UIArmoryPanel> panels;
        [SerializeField] private UIManager uiManager;

        private BaseItem currentWeapon = new BaseItem();
        private UIItemMenuArmory currentMenuArmory = new UIItemMenuArmory();

        private void OnEnable()
        {
            playerData.IsUsingUI = true;
            Time.timeScale = 0;
            
            currentWeapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;
            
            if (uiArmory.ItemMenuSelect == null)
            {
                uiArmory.ItemMenuSelect = startMenuArmory;
            }
            
            currentMenuArmory = uiArmory.ItemMenuSelect;

            var item = playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x =>
                x.WeaponType == currentMenuArmory.WeaponMenu);

            uiManager.SetVfxShowWeapon(playerData.Info.DeviceCompartment.SlotWeapon.Info);

            ChangePanelWeaponItem(playerData.Info.DeviceCompartment.SlotWeapon.Info);

            exitButton.onClick.AddListener(Exit);
            uiArmory.ClickedMenu += UiArmoryOnClickedMenu;
            uiArmory.ClickedPocked += UiArmoryOnClickedPocked;
        }
        
        private void OnDisable()
        {
            uiManager.StopVfxShowWeapon();
            exitButton.onClick.RemoveListener(Exit);
            uiArmory.ClickedMenu -= UiArmoryOnClickedMenu;
            uiArmory.ClickedPocked -= UiArmoryOnClickedPocked;
            
            playerData.Info.DeviceCompartment.SlotWeapon.Remove(playerData.Info.DeviceCompartment.SlotWeapon.Info,
                playerData);

            playerData.Info.DeviceCompartment.SlotWeapon.Add(currentWeapon, playerData);
            uiArmory.ItemMenuSelect = currentMenuArmory;
            
            playerData.IsUsingUI = false;
            Time.timeScale = 1;
            uiGamePlay.SetUIParentSkill();
        }
        
        private void UiArmoryOnClickedMenu(UIItemMenuArmory callback)
        {
            BaseItem item =
                playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == callback.WeaponMenu);
            
            playerData.Info.DeviceCompartment.SlotWeapon.Remove(playerData.Info.DeviceCompartment.SlotWeapon.Info,
                playerData);

            playerData.Info.DeviceCompartment.SlotWeapon.Add(item, playerData);

            uiArmory.ItemPocketWeapon.SetImage(item.Image[item.LevelIndex]);

            uiManager.SetVfxShowWeapon(item);
            
            if (uiArmory.ItemPocketSelect.ItemPocketTypes == ItemTypes.Weapon)
            {
                ChangePanelWeapon();
            }
        }
        
        private void UiArmoryOnClickedPocked(UIItemPocketArmory callback)
        {
            if (callback.ItemPocketTypes == ItemTypes.Weapon)
            {
                ChangePanelWeapon();
            }
            else if (callback.ItemPocketTypes == ItemTypes.Support)
            {
                if (callback.SupportName == ItemsName.GlassTech)
                {
                    ShowPanel(PanelName.GlassPanel);
                }
                else if (callback.SupportName == ItemsName.BangleTech)
                {
                    ShowPanel(PanelName.BanglePanel);
                }
            }
        }
        
        private void ChangePanelWeaponItem(BaseItem item)
        {
            switch (item.WeaponType)
            {
                case WeaponTypes.Sword:
                    ShowPanel(PanelName.SwordPanel);
                    break;
                case WeaponTypes.Gun:
                    ShowPanel(PanelName.GunPanel);
                    break;
                case WeaponTypes.Knuckle:
                    ShowPanel(PanelName.KnucklePanel);
                    break;
            }
        }

        private void ChangePanelWeapon()
        {
            /*switch (uiArmory.ItemMenuSelect.Item.WeaponType)
            {
                case WeaponTypes.Sword:
                    ShowPanel(PanelName.SwordPanel);
                    break;
                case WeaponTypes.Gun:
                    ShowPanel(PanelName.GunPanel);
                    break;
                case WeaponTypes.Knuckle:
                    ShowPanel(PanelName.KnucklePanel);
                    break;
            }*/
            var weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;
            switch (weapon.WeaponType)
            {
                case WeaponTypes.Sword:
                    ShowPanel(PanelName.SwordPanel);
                    break;
                case WeaponTypes.Gun:
                    ShowPanel(PanelName.GunPanel);
                    break;
                case WeaponTypes.Knuckle:
                    ShowPanel(PanelName.KnucklePanel);
                    break;
            }
        }

        private void ShowPanel(PanelName panelName)
        {
            for (int i = 0; i < panels.Count; i++)
            {
                panels[i].gameObject.SetActive(panels[i].NamePanel == panelName);
            }
        }
        
        private void Exit()
        {
            gameObject.SetActive(false);
        }
    }
}