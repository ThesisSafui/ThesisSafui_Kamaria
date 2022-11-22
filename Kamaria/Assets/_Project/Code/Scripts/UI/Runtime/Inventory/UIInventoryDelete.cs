using System;
using System.Globalization;
using Kamaria.Item;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI.Inventory
{
    public sealed class UIInventoryDelete : MonoBehaviour
    {
        [SerializeField] private UIInventoryAndVaultSO uiInventoryAndVaultData;
        [SerializeField] private TextMeshProUGUI count;
        [SerializeField] private Slider slider;
        [SerializeField] private Button confirmButton;
        [SerializeField] private Button cancelButton;

        private BaseItem itemSelect;
        private int countMove;
        
        public void Init(BaseItem itemSelect)
        {
            this.itemSelect = itemSelect;
        }
        
        private void OnEnable()
        {
            slider.value = Single.MinValue;
            count.text = slider.value.ToString(CultureInfo.InvariantCulture);
            slider.maxValue = uiInventoryAndVaultData.ItemSelect.ItemCount;
            
            slider.onValueChanged.AddListener(ChangeCount);
            confirmButton.onClick.AddListener(Confirm);
            cancelButton.onClick.AddListener(Cancel);
        }

        private void OnDisable()
        {
            slider.value = Single.MinValue;
            count.text = slider.value.ToString(CultureInfo.InvariantCulture);
            
            slider.onValueChanged.RemoveListener(ChangeCount);
            confirmButton.onClick.RemoveListener(Confirm);
            cancelButton.onClick.RemoveListener(Cancel);
        }

        private void ChangeCount(float callback)
        {
            countMove = (int)callback;
            count.text = callback.ToString(CultureInfo.InvariantCulture);
        }
        
        private void Confirm()
        {
            itemSelect.Count -= countMove;
            uiInventoryAndVaultData.OnConfirm();
            gameObject.SetActive(false);
        }
        
        private void Cancel()
        {
            gameObject.SetActive(false);
        }
    }
}