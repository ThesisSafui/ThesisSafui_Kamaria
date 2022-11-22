using System.Collections;
using Cinemachine;
using Kamaria.Player.Controller;
using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.ObjInteract
{
    public sealed class InteractTeleport : MonoBehaviour
    {
        [SerializeField] private bool useWithQuest;
        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
        [SerializeField] private Transform spawnPos;
        [SerializeField] private RectTransform uiInteract;
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private float defaultTime = 0.8f;
        [SerializeField] private GameObject uiQuest;

        private CinemachineComposer cinemachineComposer;
        private float deadZoneHeight;
        private float deadZoneWidth;

        private void OnTriggerStay(Collider other)
        {
            if (!other.TryGetComponent(out IInteractable target)) return;
            
            //var target = other.GetComponentInParent<PlayerEvent>();
            /*playerData.CanInteract = true;
            uiInteract.gameObject.SetActive(true);
            
            if (playerData.IsInteract)
            {
                if (useWithQuest)
                {
                    uiInteract.gameObject.SetActive(false);
                    playerData.IsInteract = false;
                    uiQuest.SetActive(true);
                    StartCoroutine(WaitQuestSucceed(other));
                    return;
                }
                
                playerData.TelePort = true;
                Debug.Log($"Teleport = {gameObject.name}");
                uiInteract.gameObject.SetActive(false);
                playerData.IsInteract = false;
                other.gameObject.SetActive(false);
                Teleport(other);
            }*/

            if (useWithQuest)
            {
                playerData.CanInteract = true;
                uiInteract.gameObject.SetActive(true);
                
                if (playerData.IsInteract)
                {
                    uiInteract.gameObject.SetActive(false);
                    playerData.IsInteract = false;
                    uiQuest.SetActive(true);
                    StartCoroutine(WaitQuestSucceed(other));
                }
               
                return;
            }
            
            playerData.TelePort = true;
            Debug.Log($"Teleport = {gameObject.name}");
            //uiInteract.gameObject.SetActive(false);
            //playerData.IsInteract = false;
            other.gameObject.SetActive(false);
            Teleport(other);
        }

        private void OnTriggerExit(Collider other)
        {
            playerData.CanInteract = false;
            uiInteract.gameObject.SetActive(false);
            StopAllCoroutines();
        }

        private IEnumerator WaitQuestSucceed(Collider other)
        {
            yield return new WaitUntil((() => playerData.QuestPasswordSucceed));
            playerData.QuestPasswordSucceed = false;
            other.gameObject.SetActive(false);
            Teleport(other);
        }
        
        private void Teleport(Collider other)
        {
            other.gameObject.transform.position = spawnPos.position;
            cinemachineComposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineComposer>();
            deadZoneHeight = cinemachineComposer.m_DeadZoneHeight;
            deadZoneWidth = cinemachineComposer.m_DeadZoneWidth;
            cinemachineComposer.m_DeadZoneHeight = 0;
            cinemachineComposer.m_DeadZoneWidth = 0;
            //StartCoroutine(OpenObj(other));
            StartCoroutine(DefaultTime(other));
        }

        private IEnumerator OpenObj(Collider other)
        {
            yield return new WaitForSeconds(0.1f);
            other.gameObject.SetActive(true);
        }

        private IEnumerator DefaultTime(Collider other)
        {
            yield return new WaitForSeconds(defaultTime);
            other.gameObject.SetActive(true);
            cinemachineComposer.m_DeadZoneHeight = deadZoneHeight;
            cinemachineComposer.m_DeadZoneWidth = deadZoneWidth;
        }
    }
}