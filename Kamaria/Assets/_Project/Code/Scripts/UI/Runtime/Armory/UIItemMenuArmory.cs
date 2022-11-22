using System;
using Kamaria.Item;
using Kamaria.Player.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI.Armory
{
    public sealed class UIItemMenuArmory : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private UIArmorySO uiArmory;
        [SerializeField] private Image image;
        [SerializeField] private Button button;
        [SerializeField] private WeaponTypes weaponMenu;
        [SerializeField] private Color colorSelected;

        public WeaponTypes WeaponMenu => weaponMenu;
        public Button Button => button;
        public BaseItem Item => item;

        private BaseItem item;


        private void OnEnable()
        {
            SetItem();
            
            button.onClick.AddListener(Clicked);

            button.image.color = Color.white;
            button.interactable = true;
            
            if (playerData.Info.DeviceCompartment.SlotWeapon.Info.WeaponType == this.weaponMenu)
            {
                button.interactable = false;
                button.image.color = colorSelected;
                uiArmory.ItemMenuSelect = this;
            }
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(Clicked);
        }

        private void Clicked()
        {
            if (uiArmory.ItemMenuSelect != null)
            {
                uiArmory.ItemMenuSelect.Button.interactable = true;
                uiArmory.ItemMenuSelect.Button.image.color = Color.white;
            }
            
            button.interactable = false;
            button.image.color = colorSelected;
            uiArmory.ItemMenuSelect = this;
            uiArmory.OnClickedMenu(this);
        }

        private void SetItem()
        {
            item = playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == weaponMenu);
            image.sprite = item.Image[item.LevelIndex];
        }
    }
}