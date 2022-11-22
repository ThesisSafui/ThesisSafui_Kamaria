using System;
using Kamaria.DropItem;
using Kamaria.Item;
using Kamaria.Manager;
using Kamaria.Player.Controller;
using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.ObjInteract
{
    public sealed class InteractItemDrop : MonoBehaviour
    {
        [SerializeField] private ItemDropPrefab itemDropPrefab;
        [SerializeField] private UIManager uiManager;
        [SerializeField] private ItemsName itemsName;
        [SerializeField] private RectTransform uiInteract;
        [SerializeField] private PlayerDataSO playerData;

        public ItemsName ItemsName => itemsName;
        private bool isAdd;

        private void OnEnable()
        {
            isAdd = false;
        }

        private void OnTriggerStay(Collider other)
        {
            var target = other.GetComponentInParent<IInteractable>();
            //if (!other.TryGetComponent(out IInteractable target)) return;

            playerData.CanInteract = true;
            uiInteract.gameObject.SetActive(true);

            if (!playerData.IsInteract) return;
            
            uiInteract.gameObject.SetActive(false);
            playerData.IsInteract = false;
            target.GetItem(itemsName, 1, out isAdd);
                
            if (isAdd)
            {
                uiManager.NotifiedGetItem(itemsName);
                itemDropPrefab.gameObject.SetActive(false);
            }
            else
            {
                uiManager.NotifiedInventoryFull();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            playerData.CanInteract = false;
            uiInteract.gameObject.SetActive(false);
        }
    }
}