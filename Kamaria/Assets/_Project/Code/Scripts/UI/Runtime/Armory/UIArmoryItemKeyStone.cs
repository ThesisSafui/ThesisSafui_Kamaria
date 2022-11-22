using System;
using Kamaria.Item;
using Kamaria.Player.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI.Armory
{
    public sealed class UIArmoryItemKeyStone : MonoBehaviour
    {
        [SerializeField] private UIArmoryKeyStone uiArmoryKeyStone;
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private KeyStones keyStoneType;
        [SerializeField] private Button button;
        [SerializeField] private Image image;
        [SerializeField] private RectTransform usingPanel;
        [SerializeField] private RectTransform equipKeyStonePanel;
        [SerializeField] private RectTransform uiLock;

        public KeyStones KeyStoneType => keyStoneType;
        public Button Button => button;
        public Image Image => image;
        public RectTransform UsingPanel => usingPanel;
        public RectTransform EquipKeyStonePanel => equipKeyStonePanel;
        public RectTransform UiLock => uiLock;

        private void OnEnable()
        {
            button.onClick.AddListener(Clicked);
            
            SetItemKeyStone();
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(Clicked);
        }
        
        private void Clicked()
        {
            uiArmoryKeyStone.CurrentItemKeyStones = this;
            uiArmoryKeyStone.Clicked();
        }

        private void SetItemKeyStone()
        {
            var weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;
            
            // 0 = level1
            // 1 = level2
            // 2 = level3
            uiLock.gameObject.SetActive(true);
            image.enabled = false;
            
            CheckHaveKeyStone();
            
            if (!weapon.WeaponComponentLevel[1].UpgradeComponent2)
            {
                Debug.Log("Not Leve2  UpgradeComponent2");
                uiLock.gameObject.SetActive(true);
                button.enabled = false;
                return;
            }
            
            uiLock.gameObject.SetActive(false);
            CheckCanUesKeyStone();

            if (!weapon.WeaponComponentLevel[2].UpgradeComponent2)
            {
                Debug.Log("Not Leve3 UpgradeComponent2");
                if (keyStoneType == KeyStones.PowerStone)
                {
                    uiLock.gameObject.SetActive(true);
                    button.enabled = false;
                    return;
                }
            }

        }

        private void CheckHaveKeyStone()
        {
            for (int i = 0; i < playerData.Info.Inventory.InventoryKeyStone.KeyStones.Count; i++)
            {
                if (playerData.Info.Inventory.InventoryKeyStone.KeyStones[i].UsedKeyStone == keyStoneType)
                {
                    image.enabled = true;
                    return;
                }
            }
            
            button.enabled = false;
        }
        
        private void CheckCanUesKeyStone()
        {
            for (int i = 0; i < playerData.Info.Inventory.InventoryKeyStone.KeyStones.Count; i++)
            {
                if (playerData.Info.Inventory.InventoryKeyStone.KeyStones[i].UsedKeyStone == keyStoneType)
                {
                    image.enabled = true;
                    button.enabled = true;
                    return;
                }
            }
            
            button.enabled = false;
        }
    }
}