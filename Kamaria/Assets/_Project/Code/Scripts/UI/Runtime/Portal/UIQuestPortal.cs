using System;
using System.Collections.Generic;
using System.Linq;
using Kamaria.Item;
using Kamaria.Manager;
using Kamaria.Player.Controller;
using Kamaria.Player.Data;
using Kamaria.Player.Data.Quest;
using Kamaria.SaveLoad;
using Kamaria.UI.UIMainGame;
using Kamaria.Utilities.SaveLoad;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI.Portal
{
    public sealed class UIQuestPortal : MonoBehaviour
    {
        [SerializeField] private UIManager uiManager;
        [SerializeField] private UIQuest uiQuest;
        [SerializeField] private PlayerEvent playerEvent;
        [SerializeField] private UIPortal uiPortal;
        [SerializeField] private TextMeshProUGUI nameMainQuest;
        [SerializeField] private TextMeshProUGUI nameSideQuest1;
        [SerializeField] private TextMeshProUGUI nameSideQuest2;
        [SerializeField] private Button mainQuestButton;
        [SerializeField] private Button sideQuest1Button;
        [SerializeField] private Button sideQuest2Button;
        [SerializeField] private RectTransform parentNextMap;
        [SerializeField] private TextMeshProUGUI nextMap;
        [SerializeField] private Button teleportButton;
        [SerializeField] private Button getQuestButton;
        [SerializeField] private Button cancelButton;
        [SerializeField] private Button claimButton;
        [SerializeField] private UIQuestPortalSelect uiQuestPortalSelect;
        [SerializeField] private QuestManagerSO questManagerSO;
        [SerializeField] private PlayerDataSO playerData;

        private bool isMain;
        private bool isSide1;
        
        private void OnEnable()
        {
            mainQuestButton.interactable = true;
            getQuestButton.gameObject.SetActive(false);
            teleportButton.interactable = false;
            
            mainQuestButton.onClick.AddListener(ClickMainQuest);
            getQuestButton.onClick.AddListener(GetQuest);
            cancelButton.onClick.AddListener(CancelQuest);
            claimButton.onClick.AddListener(ClaimReward);
            teleportButton.onClick.AddListener(Teleport);
            
            sideQuest1Button.onClick.AddListener(ClickSideQuest1);
            sideQuest2Button.onClick.AddListener(ClickSideQuest2);

            if (questManagerSO.CurrentQuests != null)
            {
                if (questManagerSO.CurrentQuests.QuestTypes == QuestTypes.Main)
                {
                    isMain = true;
                    isSide1 = false;
                }
                else if (questManagerSO.CurrentQuests.QuestTypes == QuestTypes.Side1)
                {
                    isMain = false;
                    isSide1 = true;
                }
                else if (questManagerSO.CurrentQuests.QuestTypes == QuestTypes.Side2)
                {
                    isMain = false;
                    isSide1 = false;
                }
            }

            SetUI();
        }

        private void OnDisable()
        {
            mainQuestButton.onClick.RemoveListener(ClickMainQuest);
            getQuestButton.onClick.RemoveListener(GetQuest);
            cancelButton.onClick.RemoveListener(CancelQuest);
            claimButton.onClick.RemoveListener(ClaimReward);
            teleportButton.onClick.RemoveListener(Teleport);
            
            sideQuest1Button.onClick.RemoveListener(ClickSideQuest1);
            sideQuest2Button.onClick.RemoveListener(ClickSideQuest2);
        }
        
        private void Teleport()
        {
            uiPortal.SaveLoadManager.SaveData(true);
        }

        private void ClickSideQuest1()
        {
            isMain = false;
            isSide1 = true;
            sideQuest1Button.interactable = false;
            getQuestButton.gameObject.SetActive(true);
        }
        
        private void ClickSideQuest2()
        {
            isMain = false;
            isSide1 = false;
            sideQuest2Button.interactable = false;
            getQuestButton.gameObject.SetActive(true);
        }

        private void ClaimReward()
        {
            if (!GetItem(questManagerSO.CurrentQuests.ItemsReword))
            {
                uiManager.NotifiedInventoryFull();
                return;
            }

            for (int i = 0; i < questManagerSO.CurrentQuests.ItemsReword.Count; i++)
            {
                if (questManagerSO.CurrentQuests.ItemsReword[i].Info.Types == ItemTypes.CraftMaterial)
                {
                    playerEvent.GetItem(questManagerSO.CurrentQuests.ItemsReword[i].Info.Name[Index.Start],
                        questManagerSO.CurrentQuests.ItemsReword[i].Info.Count, out bool isAdd);
                    if (isAdd)
                    {
                        uiManager.NotifiedGetItem(questManagerSO.CurrentQuests.ItemsReword[i].Info.Name[Index.Start]);
                    }
                    else
                    {
                        uiManager.NotifiedInventoryFull();
                    }
                }
                else if (questManagerSO.CurrentQuests.ItemsReword[i].Info.Types == ItemTypes.KeyStone)
                {
                    playerData.Info.Inventory.InventoryKeyStone.GetKeyStone(questManagerSO.CurrentQuests.ItemsReword[i]
                        .Info);
                    
                    uiManager.NotifiedGetItem(questManagerSO.CurrentQuests.ItemsReword[i].Info.Name[Index.Start]);
                }
            }
            
            claimButton.gameObject.SetActive(false);
            
            if (questManagerSO.CurrentQuests.QuestTypes == QuestTypes.Main)
            {
                questManagerSO.MainQuests[playerData.Info.Quest.MainQuest.indexCurrentQuest].GetReword(playerData.Info);
            }
            else
            {
                questManagerSO.SideQuests.Find(x => x.IsDoing).GetReword(playerData.Info);
            }

            mainQuestButton.interactable = true;
            sideQuest1Button.interactable = true;
            sideQuest2Button.interactable = true;
            uiQuest.SetUI();
            SetUI();
            getQuestButton.gameObject.SetActive(false);
            
        }

        private bool GetItem(List<BaseItemSO> items)
        {
            int temp = items.Sum(x => x.Info.Count);
            int tempItemInventory = playerData.Info.Inventory.InventoryGeneral.Items.Sum(x => x.Count);
            
            return temp + tempItemInventory <= 250;
        }
        
        private void CancelQuest()
        {
            getQuestButton.gameObject.SetActive(false);
            
            if (isMain)
            {
                mainQuestButton.interactable = true;
            }
            else
            {
                if (isSide1)
                {
                    sideQuest1Button.interactable = true;
                }
                else
                {
                    sideQuest2Button.interactable = true;
                }
            }
        }

        private void GetQuest()
        {
            if (questManagerSO.CurrentQuests != null)
            {
                if (!questManagerSO.CurrentQuests.IsSucceed)
                {
                    questManagerSO.CurrentQuests.ResetProgress();
                }
            }

            if (isMain)
            {
                questManagerSO.MainQuests[playerData.Info.Quest.MainQuest.indexCurrentQuest].StartQuest(playerData.Info);
            }
            else
            {
                if (isSide1)
                {
                    questManagerSO.SideQuests.Find(x => x.QuestTypes == QuestTypes.Side1).StartQuest(playerData.Info);
                }
                else
                {
                    questManagerSO.SideQuests.Find(x => x.QuestTypes == QuestTypes.Side2).StartQuest(playerData.Info);
                }
            }
            mainQuestButton.interactable = true;
            sideQuest1Button.interactable = true;
            sideQuest2Button.interactable = true;
            uiQuest.SetUI();
            SetUI();
            getQuestButton.gameObject.SetActive(false);
        }

        private void ClickMainQuest()
        {
            isMain = true;
            mainQuestButton.interactable = false;
            getQuestButton.gameObject.SetActive(true);
        }

        public void SetUI()
        {
            SetUIMapNext();
            
            SetSideQuest1();

            SetSideQuest2();

            if (playerData.Info.Quest.MainQuest.indexCurrentQuest >= playerData.Info.Quest.MainQuest.MainQuestAll.Count)
            {
                mainQuestButton.gameObject.SetActive(false);
            }
            
            if (questManagerSO.CurrentQuests != null)
            {
                if (questManagerSO.CurrentQuests.QuestTypes == QuestTypes.Main)
                {
                    if (playerData.Info.Quest.MainQuest.indexCurrentQuest == questManagerSO.MainQuests.Count)
                    {
                        mainQuestButton.gameObject.SetActive(false);
                    }
                }
            }

            if (playerData.Info.Quest.MainQuest.indexCurrentQuest < questManagerSO.MainQuests.Count)
            {
                nameMainQuest.text = questManagerSO.MainQuests[playerData.Info.Quest.MainQuest.indexCurrentQuest]
                    .TextShowNameQuest;
            }
            
            if (questManagerSO.CurrentQuests != null)
            {
                if (questManagerSO.CurrentQuests.IsDoing)
                {
                    if (questManagerSO.CurrentQuests.QuestTypes == QuestTypes.Main)
                    {
                        mainQuestButton.interactable = false;
                    }
                    
                    if (questManagerSO.CurrentQuests.IsSucceed)
                    {
                        teleportButton.interactable = false;
                    }
                    
                    if (questManagerSO.CurrentQuests.IsSucceed)
                    {
                        mainQuestButton.interactable = false;
                        sideQuest1Button.interactable = false;
                        sideQuest2Button.interactable = false;
                    }
                }

                claimButton.gameObject.SetActive(questManagerSO.CurrentQuests.IsSucceed);
            }
        }

        private void SetUIMapNext()
        {
            parentNextMap.gameObject.SetActive(false);
            teleportButton.interactable = false;
            SetTeleportButtonAndMapNext();
        }
        
        private void SetTeleportButtonAndMapNext()
        {
            if (questManagerSO.CurrentQuests == null) return;

            if (questManagerSO.CurrentQuests.TeleportToDungeons == Dungeons.None)
            {
                teleportButton.interactable = false;
                parentNextMap.gameObject.SetActive(false);
            }
            else
            {
                nextMap.text = questManagerSO.CurrentQuests.TeleportToDungeons.ToString();
                teleportButton.interactable = true;
                parentNextMap.gameObject.SetActive(true);
                switch (questManagerSO.CurrentQuests.TeleportToDungeons)
                {
                    case Dungeons.BootyCove:
                        uiPortal.loadScene = "Dungeon_1";
                        break;
                    case Dungeons.DeadIsle:
                        uiPortal.loadScene = "Dungeon_2";
                        break;
                    case Dungeons.DeadMansHeaven:
                        uiPortal.loadScene = "Dungeon_3";
                        break;
                }
            }

            if (questManagerSO.CurrentQuests.NameQuest == NameQuest.DefeatTheBossSharkPirate)
            {
                uiPortal.loadScene = uiPortal.SharkBossScenes;
                nextMap.text = Manager.Map.ForbiddenIsland.ToString();
                teleportButton.interactable = true;
                parentNextMap.gameObject.SetActive(true);
            }
            else if (questManagerSO.CurrentQuests.NameQuest == NameQuest.KingOfTheSea)
            {
                uiPortal.loadScene = uiPortal.SkeletonBossScenes;
                nextMap.text = Manager.Map.ShipOfDeath.ToString();
                teleportButton.interactable = true;
                parentNextMap.gameObject.SetActive(true);
            }
            else if (questManagerSO.CurrentQuests.NameQuest == NameQuest.NoPainNoGain)
            {
                uiPortal.loadScene = uiPortal.SharkBossScenes;
                nextMap.text = Manager.Map.ForbiddenIsland.ToString();
                teleportButton.interactable = true;
                parentNextMap.gameObject.SetActive(true);
            }
        }

        private void SetSideQuest2()
        {
            if (questManagerSO.CurrentQuests != null)
            {
                if (questManagerSO.CurrentQuests.QuestTypes == QuestTypes.Side2)
                {
                    sideQuest2Button.gameObject.SetActive(true);
                    sideQuest2Button.interactable = false;
                    return;
                }
            }
            
            var mainQuest =
                playerData.Info.Quest.MainQuest.MainQuestAll.Find(x => x.NameQuest == NameQuest.DefeatTheInvader02);

            nameSideQuest2.text = playerData.Info.Quest.SideQuest.Quest2.TextShowNameQuest;

            if (mainQuest.IsSucceed && !mainQuest.IsDoing)
            {
                sideQuest2Button.gameObject.SetActive(true);
                sideQuest2Button.interactable = true;
                
                if (playerData.Info.Quest.SideQuest.Quest2.IsSucceed)
                {
                    sideQuest2Button.gameObject.SetActive(false);
                    sideQuest2Button.interactable = false;
                }
            }
            else
            {
                sideQuest2Button.gameObject.SetActive(false);
                sideQuest2Button.interactable = false;
            }
        }

        private void SetSideQuest1()
        {
            if (questManagerSO.CurrentQuests != null)
            {
                if (questManagerSO.CurrentQuests.QuestTypes == QuestTypes.Side1)
                {
                    sideQuest1Button.gameObject.SetActive(true);
                    sideQuest1Button.interactable = false;
                    return;
                }
            }
            
            var mainQuest =
                playerData.Info.Quest.MainQuest.MainQuestAll.Find(x => x.NameQuest == NameQuest.DefeatTheBossSharkPirate);

            nameSideQuest1.text = playerData.Info.Quest.SideQuest.Quest1.TextShowNameQuest;
            
            if (mainQuest.IsSucceed && !mainQuest.IsDoing)
            {
                sideQuest1Button.gameObject.SetActive(true);
                sideQuest1Button.interactable = true;
                if (playerData.Info.Quest.SideQuest.Quest1.IsSucceed)
                {
                    sideQuest1Button.gameObject.SetActive(false);
                    sideQuest1Button.interactable = false;
                }
            }
            else
            {
                sideQuest1Button.gameObject.SetActive(false);
                sideQuest1Button.interactable = false;
            }
            
            var windStone =
                playerData.Info.Inventory.InventoryKeyStone.KeyStones.Find(x => x.UsedKeyStone == KeyStones.WindStone);
            
            if (windStone != null)
            {
                sideQuest1Button.gameObject.SetActive(false);
                sideQuest1Button.interactable = false;
            }
        }

        public void ShowUiQuestPortalSelectMain()
        {
            uiQuestPortalSelect.SetUI(true,0);
            uiQuestPortalSelect.Parent.gameObject.SetActive(true);
        }
        
        public void ShowUiQuestPortalSelectSide(int indexSide)
        {
            uiQuestPortalSelect.SetUI(false, indexSide);
            uiQuestPortalSelect.Parent.gameObject.SetActive(true);
        }
        
        public void HideUiQuestPortalSelect()
        {
            uiQuestPortalSelect.Parent.gameObject.SetActive(false);
        }
    }
}