using Kamaria.Item;
using Kamaria.Manager;
using Kamaria.Player.Data;
using Kamaria.Player.Data.Quest;
using UnityEngine;

namespace Kamaria.ObjInteract
{
    public sealed class KeyStoneInteract : MonoBehaviour
    {
        [SerializeField] private ItemQuest itemQuest;
        [SerializeField] private BaseItem keyStone = new BaseItem();
        [SerializeField] private RectTransform uiInteract;
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private UIManager uiManager;
        
        private void OnTriggerStay(Collider other)
        {
            //if (!other.TryGetComponent(out IInteractable target)) return;

            playerData.CanInteract = true;
            uiInteract.gameObject.SetActive(true);
            
            if (playerData.IsInteract)
            {
                uiInteract.gameObject.SetActive(false);
                playerData.IsInteract = false;
                GetKeyStone();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            playerData.CanInteract = false;
            uiInteract.gameObject.SetActive(false);
        }

        private void GetKeyStone()
        {
            uiManager.NotifiedGetItem(itemQuest.Item);
            playerData.Info.Inventory.InventoryKeyStone.GetKeyStone(keyStone);
            this.gameObject.SetActive(false);
        }
    }
}