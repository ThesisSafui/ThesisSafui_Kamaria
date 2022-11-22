using System;
using Kamaria.Item;
using Kamaria.Utilities;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Kamaria.UI
{
    public enum SelectTypes
    {
        Inventory,
        Vault
    }
    
    public sealed class UIItemInventoryAndVaultItem : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private UIInventoryAndVaultSO uiInventoryAndVaultData;
        [SerializeField] private Image outLine;
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI count;

        private SelectTypes selectTypes;
        private BaseItem baseItem;
        private int itemCount;

        public SelectTypes SelectTypes => selectTypes;
        public Image OutLine => outLine;
        public BaseItem BaseItem => baseItem;
        public int ItemCount => itemCount;

        private void OnEnable()
        {
            outLine.enabled = false;
        }

        private void OnDisable()
        {
           
        }

        private void Clicked()
        {
            if (uiInventoryAndVaultData.ItemSelect != null)
            {
                uiInventoryAndVaultData.ItemSelect.OutLine.enabled = false;
            }

            SoundHandler.Instance.PlayUI(SoundClip.Sound.Click);
            uiInventoryAndVaultData.ItemSelect = this;
            uiInventoryAndVaultData.OnClicked(this);
        }

        public void Init(Sprite image, int count, SelectTypes selectTypes, BaseItem baseItem)
        {
            this.image.sprite = image;
            itemCount = count;
            this.count.text = count.ToString();
            this.selectTypes = selectTypes;
            this.baseItem = baseItem;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                Clicked();
            }
        }
    }
}