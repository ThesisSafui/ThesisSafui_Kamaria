using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Kamaria.DropItem;
using Kamaria.Item;
using Kamaria.Manager;
using Kamaria.Player.Controller;
using Kamaria.Player.Data;
using Kamaria.Player.Data.Quest;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.ObjInteract
{
    public sealed class InteractTreasureChest : MonoBehaviour
    {
        [SerializeField] private QuestManagerSO questManagerSO;
        [SerializeField] private FarmingManagerSO farmingManagerSO;
        [SerializeField] private CreateItemDropManager createItemDropManager;
        [SerializeField] private GameObject mainObj;
        [SerializeField] private DropItemHandler dropItemHandler;
        [SerializeField] private RectTransform uiInteract;
        [SerializeField] private Image uiCooldownFill;
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private float unLockerTime;
        [SerializeField] private float fadeSpeed;
        
        private List<Renderer> renderers = new List<Renderer>();
        private List<float> tempAlpha = new List<float>();
        private bool isUnlocking;
        private bool unlockFinish;
        private float currentTime;
        private bool isDropItem;
        private bool canInteract;
        
        private readonly int opacity = Shader.PropertyToID("_Opacity");
        private const string RT_SHADER = "Universal Render Pipeline/RealToon/Version 5/Default/Default";

        private void Awake()
        {
            if (renderers.Count == 0)
            {
                renderers.AddRange( mainObj.GetComponentsInChildren<Renderer>());
            }

            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i].material.shader.name == "Hidden/VFX/vfx_LootDrop/System/GLOW")
                {
                    renderers.Remove(renderers[i]);
                    continue;
                }
                tempAlpha.Add(renderers[i].material.shader.name == RT_SHADER
                    ? renderers[i].material.GetFloat(opacity)
                    : renderers[i].material.color.a);
            }
        }

        private void OnEnable()
        {
            Initialized();
        }

        private void OnDisable()
        {
            if (uiInteract != null)
            {
                uiInteract.gameObject.SetActive(false);
            }
        }

        private void Initialized()
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i].material.shader.name == RT_SHADER)
                {
                    renderers[i].material.SetFloat(opacity, tempAlpha[i]);
                }
                else
                {
                    renderers[i].material.color = new Color(
                        renderers[i].material.color.r,
                        renderers[i].material.color.g,
                        renderers[i].material.color.b,
                        tempAlpha[i]
                    );
                }
            }

            isUnlocking = false;
            unlockFinish = false;
            isDropItem = false;
            canInteract = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            //if (!other.TryGetComponent(out IInteractable target)) return;

            uiCooldownFill.fillAmount = 0;
            uiInteract.gameObject.SetActive(true);
            currentTime = unLockerTime;
            canInteract = true;
            playerData.CanInteract = true;
        }

        private void OnTriggerStay(Collider other)
        {
            //if (!other.TryGetComponent(out IInteractable target)) return;
            //var target = other.GetComponentInParent<IInteractable>();
            
            playerData.CanInteract = true;
            canInteract = true;
        }

        private void OnTriggerExit(Collider other)
        {
            ResetProcess();
        }

        private void ResetProcess()
        {
            uiInteract.gameObject.SetActive(false);
            isUnlocking = false;
            canInteract = false;
            playerData.CanInteract = false;
            playerData.IsInteract = false;
        }

        private void Update()
        {
            if (!canInteract) return;

            isUnlocking = playerData.IsInteract;
            
            if (playerData.CharacterControllerData.Dashing)
            {
                isUnlocking = false;
            }

            if (unlockFinish)
            {
                DropItem();
                FadeObj();
                return;
            }

            if (isUnlocking)
            {
                if (currentTime > 0)
                {
                    SetMovementAndAnimation(true);
                    currentTime -= Time.deltaTime;
                    uiCooldownFill.fillAmount = Mathf.Clamp((float)currentTime / unLockerTime, 0, 1);
                }
                else
                {
                    SetMovementAndAnimation(false);
                    uiInteract.gameObject.SetActive(false);
                    uiCooldownFill.fillAmount = 0;
                    unlockFinish = true;
                }
            }
            else
            {
                SetMovementAndAnimation(false);
                currentTime = unLockerTime;
                uiCooldownFill.fillAmount = 0;
                unlockFinish = false;
            }

            if (playerData.IsDead)
            {
                ResetProcess();
            }
        }

        private void SetMovementAndAnimation(bool isStop)
        {
            if (isStop)
            {
                Quaternion rotation =
                    Quaternion.LookRotation(new Vector3(transform.position.x, 0,
                            transform.position.z) - new Vector3(
                            playerData.PlayerAnimation.Animator.gameObject.transform.position.x, 0,
                            playerData.PlayerAnimation.Animator.gameObject.transform.position.z),
                        Vector3.up);

                playerData.PlayerAnimation.Animator.gameObject.transform.DORotate(rotation.eulerAngles, 0.2f);
            }
            
            playerData.PlayerAnimation.IsAttacking = isStop;
            playerData.PlayerAnimation.IsAnimationAttackNotMove = isStop;
            playerData.PlayerAnimation.Animator.SetBool(playerData.PlayerAnimation.AnimIsInteract, isStop);
            playerData.PlayerAnimation.Animator.SetBool(playerData.PlayerAnimation.AnimIsCanMove, !isStop);
        }

        private void DropItem()
        {
            if (isDropItem) return;
            isDropItem = true;
            UpdateProgressQuest3();
            ResetProcess();
            
            for (int i = 0; i < dropItemHandler.CountDropItem; i++)
            {
                //if (!dropItemHandler.CanDrop()) continue; // If need random can drop.
            
                dropItemHandler.DropItem(out ItemsName dropItem);
                Debug.Log("DropItem");

                createItemDropManager.CreateItemDrop(dropItem, this.transform);
            }
            StartCoroutine(FadeTime());
        }

        private void UpdateProgressQuest3()
        {
            if (farmingManagerSO.Dungeons != Dungeons.BootyCove) return;
            if (questManagerSO.CurrentQuests == null) return;
            
            if (!questManagerSO.CanDoQuest(NameQuest.FindTheTreasure) 
                || questManagerSO.CurrentQuests.IsSucceed) return;
            
            questManagerSO.UpdateProgressQuest(QuestRequirement.Find2TreasureBoxesInTheAbandonMapBootyCove,out bool finish);

            questManagerSO.CurrentQuests.UpdateProgress();
        }

        private void FadeObj()
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i].material.shader.name == RT_SHADER)
                {
                    renderers[i].material.SetFloat(opacity, Mathf.Lerp(renderers[i].material.GetFloat(opacity),
                        0, fadeSpeed * Time.deltaTime));
                }
                else
                {
                    renderers[i].material.color = new Color(
                        renderers[i].material.color.r,
                        renderers[i].material.color.g,
                        renderers[i].material.color.b,
                        Mathf.Lerp(renderers[i].material.color.a, 0, fadeSpeed * Time.deltaTime)
                    );
                }
            }
        }

        private IEnumerator FadeTime()
        {
            yield return new WaitForSeconds(fadeSpeed);
            mainObj.SetActive(false);
        }
    }
}