using System;
using Kamaria.Item;
using Kamaria.Player.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI.Armory
{
    public sealed class UIArmoryEquipKeyStone : MonoBehaviour
    {
        [SerializeField] private UIArmoryKeyStone uiArmoryKeyStone;
        [SerializeField] private UIArmoryItemKeyStone uiArmoryItemKeyStone;
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private Button exitButton;
        [SerializeField] private Button equipButton;
        [SerializeField] private Button unEquipButton;

        private void OnEnable()
        {
            exitButton.onClick.AddListener(Exit);
            equipButton.onClick.AddListener(Equip);
            unEquipButton.onClick.AddListener(UnEquip);
            
            SetEquipButton();
        }

        private void OnDisable()
        {
            exitButton.onClick.RemoveListener(Exit);
            equipButton.onClick.RemoveListener(Equip);
            unEquipButton.onClick.RemoveListener(UnEquip);
        }
        
        private void Exit()
        {
            this.gameObject.SetActive(false);
        }
        
        private void Equip()
        {
            playerData.Info.DeviceCompartment.SlotWeapon.Info.UsedKeyStone = uiArmoryItemKeyStone.KeyStoneType;
            uiArmoryKeyStone.SetUsingKeyStone();
            SetEquipButton();
        }

        private void UnEquip()
        {
            playerData.Info.DeviceCompartment.SlotWeapon.Info.UsedKeyStone = KeyStones.None;
            uiArmoryKeyStone.SetUsingKeyStone();
            SetEquipButton();
        }

        private void SetEquipButton()
        {
            if (playerData.Info.DeviceCompartment.SlotWeapon.Info.UsedKeyStone == uiArmoryItemKeyStone.KeyStoneType)
            {
                equipButton.gameObject.SetActive(false);
                unEquipButton.gameObject.SetActive(true);
            }
            else
            {
                equipButton.gameObject.SetActive(true);
                unEquipButton.gameObject.SetActive(false);
            }
        }
    }
}