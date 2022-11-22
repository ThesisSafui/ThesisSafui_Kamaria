using Kamaria.Item;
using UnityEngine;

namespace Kamaria.Player.Controller
{
    public interface IInteractable
    {
        public void GetItem(ItemsName itemName, int count, out bool isAdd);
        public void OpenUI(GameObject ui, bool isOpen);
    }
}