using System;
using Kamaria.Item;
using Kamaria.Player.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI.Armory
{
    public sealed class UIItemPocketArmory : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private UIArmorySO uiArmory;
        [SerializeField] private Image image;
        [SerializeField] private Button button;
        [SerializeField] private ItemTypes itemPocketTypes;
        [SerializeField] private ItemsName supportName;

        public Button Button => button;
        public ItemTypes ItemPocketTypes => itemPocketTypes;
        public ItemsName SupportName => supportName;
        public BaseItem Item => item;
        
        private BaseItem item;

        private void OnEnable()
        {
            button.onClick.AddListener(Clicked);

            if (uiArmory.ItemPocketSelect != null)
            {
                if (uiArmory.ItemPocketSelect.SupportName == this.supportName)
                {
                    uiArmory.ItemPocketSelect = this;
                    button.interactable = false;
                }
            }
            else
            {
                if (itemPocketTypes == ItemTypes.Weapon)
                {
                    uiArmory.ItemPocketSelect = this;
                    button.interactable = false;
                }
            }

            SetItem();
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(Clicked);
        }

        private void Clicked()
        {
            if (uiArmory.ItemPocketSelect != null)
            {
                uiArmory.ItemPocketSelect.Button.interactable = true;
            }
            
            button.interactable = false;
            uiArmory.ItemPocketSelect = this;
            uiArmory.OnClickedPocked(this);
        }

        private void SetItem()
        {
            if (itemPocketTypes == ItemTypes.Weapon)
            {
                if (uiArmory.ItemMenuSelect.Item == null)
                {
                    item = playerData.Info.DeviceCompartment.SlotWeapon.Info;
                }
                else
                {
                    item = uiArmory.ItemMenuSelect.Item;
                }
                uiArmory.ItemPocketWeapon = this;
            }
            else if (itemPocketTypes == ItemTypes.Support)
            {
                if (supportName == ItemsName.GlassTech)
                {
                    item = playerData.Info.DeviceCompartment.SlotGlassTech.Info;
                }
                else if (supportName == ItemsName.BangleTech)
                {
                    item = playerData.Info.DeviceCompartment.SlotBangleTech.Info;
                }
            }
            
            SetImage(item.Image[item.LevelIndex]);
        }
        
        public void SetImage(Sprite sprite)
        {
            image.sprite = sprite;
        }
    }
}