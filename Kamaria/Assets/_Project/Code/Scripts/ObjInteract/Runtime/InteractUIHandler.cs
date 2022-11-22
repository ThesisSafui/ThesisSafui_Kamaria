using System;
using Kamaria.Player.Controller;
using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.ObjInteract
{
    public sealed class InteractUIHandler : MonoBehaviour
    {
        [SerializeField] private GameObject ui;
        [SerializeField] private RectTransform uiInteract;
        [SerializeField] private PlayerDataSO playerData;
        
        private void OnTriggerStay(Collider other)
        {
            if (ui.activeInHierarchy) return;
            
            if (!other.TryGetComponent(out IInteractable target)) return;

            playerData.CanInteract = true;
            uiInteract.gameObject.SetActive(true);
            
            if (playerData.IsInteract)
            {
                uiInteract.gameObject.SetActive(false);
                playerData.IsInteract = false;
                target.OpenUI(ui, true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            playerData.CanInteract = false;
            uiInteract.gameObject.SetActive(false);
        }
    }
}