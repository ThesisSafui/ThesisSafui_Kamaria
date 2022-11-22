using System;
using UnityEngine;

namespace Kamaria.UI
{
    [CreateAssetMenu(fileName = "New UIInventoryAndVaultSO", menuName = "ThesisSafui/Data/UI/InventoryAndVault")]
    public sealed class UIInventoryAndVaultSO : ScriptableObject
    {
        public UIItemInventoryAndVaultItem ItemSelect;

        public event Action<UIItemInventoryAndVaultItem> Clicked;
        public event Action Confirm;
        
        public void OnClicked(UIItemInventoryAndVaultItem item)
        {
            Clicked?.Invoke(item);
        }

        public void OnConfirm()
        {
            Confirm?.Invoke();
        }
    }
}