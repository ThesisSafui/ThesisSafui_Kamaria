using System;
using UnityEngine;

namespace Kamaria.UI.Armory
{
    [CreateAssetMenu(fileName = "New UIArmorySO", menuName = "ThesisSafui/Data/UI/Armory")]
    public sealed class UIArmorySO : ScriptableObject
    {
        public UIItemMenuArmory ItemMenuSelect;
        public UIItemPocketArmory ItemPocketSelect;
        public UIItemPocketArmory ItemPocketWeapon;
        
        public event Action<UIItemPocketArmory> ClickedPocked;
        public event Action<UIItemMenuArmory> ClickedMenu;

        public void OnClickedPocked(UIItemPocketArmory itemPocket)
        {
            ClickedPocked?.Invoke(itemPocket);
        }

        public void OnClickedMenu(UIItemMenuArmory itemMenu)
        {
            ClickedMenu?.Invoke(itemMenu);
        }
    }
}