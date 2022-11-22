using System.Collections.Generic;
using Kamaria.Item;
using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.UI.Armory
{
    public sealed class UIArmoryKeyStone : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private List<UIArmoryItemKeyStone> itemKeyStones;
        [SerializeField] private Color colorUnUsingKeyStone;
        [SerializeField] private Color colorUsingKeyStone;

        [HideInInspector] public UIArmoryItemKeyStone CurrentItemKeyStones;

        public Color ColorUnUsingKeyStone => colorUnUsingKeyStone;
        public Color ColorUsingKeyStone => colorUsingKeyStone;

        private void OnEnable()
        {
            SetItemKeyStone();
        }

        private void OnDisable()
        {
        }

        private void SetItemKeyStone()
        {
            for (int i = 0; i < itemKeyStones.Count; i++)
            {
                itemKeyStones[i].EquipKeyStonePanel.gameObject.SetActive(false);
            }
            
            SetUsingKeyStone();
        }

        public void Clicked()
        {
            for (int i = 0; i < itemKeyStones.Count; i++)
            {
                if (itemKeyStones[i].KeyStoneType == CurrentItemKeyStones.KeyStoneType)
                {
                    itemKeyStones[i].EquipKeyStonePanel.gameObject.SetActive(true);
                    continue;
                }
                
                itemKeyStones[i].EquipKeyStonePanel.gameObject.SetActive(false);
            }
        }

        public void SetUsingKeyStone()
        {
            if (playerData.Info.DeviceCompartment.SlotWeapon.Info.UsedKeyStone != KeyStones.None)
            {
                var keyStone = itemKeyStones.Find(x =>
                    x.KeyStoneType == playerData.Info.DeviceCompartment.SlotWeapon.Info.UsedKeyStone);
                
                keyStone.Image.color = colorUsingKeyStone;
                keyStone.UsingPanel.gameObject.SetActive(true);
            }
            
            for (int i = 0; i < itemKeyStones.Count; i++)
            {
                if (playerData.Info.DeviceCompartment.SlotWeapon.Info.UsedKeyStone != itemKeyStones[i].KeyStoneType)
                {
                    itemKeyStones[i].Image.color = colorUnUsingKeyStone;
                    itemKeyStones[i].UsingPanel.gameObject.SetActive(false);
                }
            }
            
        }
    }
}