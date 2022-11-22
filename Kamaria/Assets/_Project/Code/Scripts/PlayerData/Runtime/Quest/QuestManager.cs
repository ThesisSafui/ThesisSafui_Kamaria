using System;
using System.Collections;
using System.Collections.Generic;
using Kamaria.Item;
using Kamaria.Manager;
using Kamaria.Utilities.SaveLoad;
using UnityEngine;
using Random = System.Random;

namespace Kamaria.Player.Data.Quest
{
    public sealed class QuestManager : MonoBehaviour
    {
        [SerializeField] private List<ItemQuest> itemQuests = new List<ItemQuest>();
        [SerializeField] private GameManagerFarming gameManagerFarming;
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private QuestManagerSO questManagerSO;
        [SerializeField] private SaveLoadDataSO saveLoadDataSO;

        [Header("ADD NEW")] 
        [SerializeField] private List<ItemQuest> itemsQuest4TheLostMemories = new List<ItemQuest>();
        [SerializeField] private List<ItemQuest> itemsQuestKeystonesPower = new List<ItemQuest>();
        [SerializeField] private ItemQuest itemsQuest5TrackingTheTeleport;
        [SerializeField] private ItemQuest itemQuestDefeatTheInvader;

        private Random random = new Random();

        private void OnEnable()
        {
            gameManagerFarming.RestartGame += GameManagerFarmingOnRestartGame;
            
            Initialized();

            StartCoroutine(WaitData());
        }

        private void Initialized()
        {
            foreach (var itemQuest in itemQuests)
            {
                itemQuest.gameObject.SetActive(false);
            }

            foreach (var itemQuest in itemsQuest4TheLostMemories)
            {
                itemQuest.gameObject.SetActive(false);
            }

            if (itemsQuest5TrackingTheTeleport != null)
            {
                itemsQuest5TrackingTheTeleport.gameObject.SetActive(false);
            }

            if (itemQuestDefeatTheInvader != null)
            {
                itemQuestDefeatTheInvader.gameObject.SetActive(false);
            }
            
            foreach (var itemQuest in itemsQuestKeystonesPower)
            {
                itemQuest.gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {
            gameManagerFarming.RestartGame -= GameManagerFarmingOnRestartGame;
        }

        private void GameManagerFarmingOnRestartGame()
        {
            StartCoroutine(WaitData());
        }

        private IEnumerator WaitData()
        {
            saveLoadDataSO.LoadDataFinish = false;
            yield return new WaitUntil((() => saveLoadDataSO.LoadDataFinish));
            SetItemQuest();
            QuestKeyStoneWind();
        }

        private void QuestKeyStoneWind()
        {
            var windStone =
                playerData.Info.Inventory.InventoryKeyStone.KeyStones.Find(x => x.UsedKeyStone == KeyStones.WindStone);
            
            if (windStone != null)
            {
                if (!questManagerSO.CanDoQuest(NameQuest.FindKeystonesLocation) || questManagerSO.CurrentQuests.IsSucceed) return;

                var questRequirement =
                    questManagerSO.CurrentQuests.RequirementQuests.Find(x =>
                        x.QuestRequirement == QuestRequirement.PlayerGetWindKeystone);
            
                if (questRequirement == null) return;

                questManagerSO.UpdateProgressQuest(QuestRequirement.PlayerGetWindKeystone, out bool finish);
                questManagerSO.CurrentQuests.UpdateProgress();
            }
        }
        
        public void SetItemQuest()
        {
            switch (gameManagerFarming.Dungeon)
            {
                case Dungeons.BootyCove:
                    SetItemQuestDungeon1();
                    break;
                case Dungeons.DeadIsle:
                    SetItemQuestDungeon2();
                    break;
                case Dungeons.DeadMansHeaven:
                    SetItemQuestDungeon3();
                    break;
            }
        }

        private void SetItemQuestDungeon1()
        {
            var windStone =
                playerData.Info.Inventory.InventoryKeyStone.KeyStones.Find(x => x.UsedKeyStone == KeyStones.WindStone);

            if (windStone != null)
            {
                itemQuests.Find(x => x.Item == ItemsQuest.WindStone).gameObject.SetActive(false);
            }
            else
            {
                itemQuests.Find(x => x.Item == ItemsQuest.WindStone).gameObject.SetActive(true);
            }

            if (questManagerSO.CurrentQuests == null) return;

            QuestDefeatTheInvader(NameQuest.DefeatTheInvader03, Dungeons.BootyCove);
            QuestKeystonesPower();
        }

        private void SetItemQuestDungeon2()
        {
            if (questManagerSO.CurrentQuests == null) return;
            
            QuestTrackingTheTeleport();
            QuestDefeatTheInvader(NameQuest.DefeatTheInvader02, Dungeons.DeadIsle);
        }
        
        private void SetItemQuestDungeon3()
        {
            if (questManagerSO.CurrentQuests == null) return;
            
            QuestTheLostMemories();
            QuestDefeatTheInvader(NameQuest.DefeatTheInvade, Dungeons.DeadMansHeaven);
        }

        private void QuestDefeatTheInvader(NameQuest nameQuest,Dungeons dungeons)
        {
            if (!questManagerSO.CanDoQuest(nameQuest) || questManagerSO.CurrentQuests.IsSucceed) return;
            
            if (questManagerSO.CurrentQuests.TeleportToDungeons != dungeons) return;

            RequirementQuest requirementQuest = dungeons switch
            {
                Dungeons.BootyCove => questManagerSO.CurrentQuests.RequirementQuests.Find(x =>
                    x.QuestRequirement == QuestRequirement.OfTheLastBossMap1),
                Dungeons.DeadIsle => questManagerSO.CurrentQuests.RequirementQuests.Find(x =>
                    x.QuestRequirement == QuestRequirement.OfTheLastBossMap2),
                Dungeons.DeadMansHeaven => questManagerSO.CurrentQuests.RequirementQuests.Find(x =>
                    x.QuestRequirement == QuestRequirement.OfTheLastBossMap3),
                _ => null
            };

            if (requirementQuest == null || requirementQuest.IsFinish) return;

            itemQuestDefeatTheInvader.gameObject.SetActive(true);
        }

        private void QuestTrackingTheTeleport()
        {
            if (!questManagerSO.CanDoQuest(NameQuest.TrackingTheTeleport) 
                || questManagerSO.CurrentQuests.IsSucceed) return;

            var questRequirement =
                questManagerSO.CurrentQuests.RequirementQuests.Find(x =>
                    x.QuestRequirement == QuestRequirement.PlayerKnowsTheBossLocation);
            
            if (questRequirement == null) return;
            
            itemsQuest5TrackingTheTeleport.gameObject.SetActive(true);
        }
        
        private void QuestTheLostMemories()
        {
            if (!questManagerSO.CanDoQuest(NameQuest.TheLostMemories) 
                || questManagerSO.CurrentQuests.IsSucceed) return;

            var questRequirement =
                questManagerSO.CurrentQuests.RequirementQuests.Find(x =>
                    x.QuestRequirement == QuestRequirement.Get3MemoryChip);

            if (questRequirement == null) return;
            
            int countItemSpawn = questRequirement.Count - questRequirement.CurrentCount;

            if (countItemSpawn == questRequirement.Count)
            {
                for (int i = 0; i < questRequirement.Count; i++)
                {
                    itemsQuest4TheLostMemories[i].gameObject.SetActive(true);
                }
                
                return;
            }
            
            Debug.Log($"countItemSpawn {countItemSpawn}");
            int lastIndex;
            int indexActive = 0;
            List<ItemQuest> itemQuestActive = new List<ItemQuest>();

            foreach (var itemQuest in itemsQuest4TheLostMemories)
            {
                itemQuest.gameObject.SetActive(false);
            }

            for (int i = 0; i < countItemSpawn; i++)
            {
                lastIndex = indexActive;
                indexActive = random.Next(itemsQuest4TheLostMemories.Count);

                if (itemQuestActive.Count != 0)
                {
                    while (indexActive == lastIndex)
                    {
                        indexActive = random.Next(itemsQuest4TheLostMemories.Count);
                    }
                }
                
                itemQuestActive.Add(itemsQuest4TheLostMemories[indexActive]);
            }
            
            foreach (var itemQuest in itemQuestActive)
            {
                itemQuest.gameObject.SetActive(true);
            }
        }

        private void QuestKeystonesPower()
        {
            if (!questManagerSO.CanDoQuest(NameQuest.KeystonesPower) || questManagerSO.CurrentQuests.IsSucceed) return;

            var questRequirement =
                questManagerSO.CurrentQuests.RequirementQuests.Find(x =>
                    x.QuestRequirement == QuestRequirement.HintOfArtifactMapBootyCove);

            if (questRequirement == null) return;
            
            int countItemSpawn = questRequirement.Count - questRequirement.CurrentCount;
            
            if (countItemSpawn == questRequirement.Count)
            {
                for (int i = 0; i < questRequirement.Count; i++)
                {
                    itemsQuestKeystonesPower[i].gameObject.SetActive(true);
                }
                
                return;
            }
            
            int lastIndex;
            int indexActive = 0;
            List<ItemQuest> itemQuestActive = new List<ItemQuest>();

            foreach (var itemQuest in itemsQuestKeystonesPower)
            {
                itemQuest.gameObject.SetActive(false);
            }

            for (int i = 0; i < countItemSpawn; i++)
            {
                lastIndex = indexActive;
                indexActive = random.Next(itemsQuestKeystonesPower.Count);

                if (itemQuestActive.Count != 0)
                {
                    while (indexActive == lastIndex)
                    {
                        indexActive = random.Next(itemsQuestKeystonesPower.Count);
                    }
                }
                
                itemQuestActive.Add(itemsQuestKeystonesPower[indexActive]);
            }
            
            foreach (var itemQuest in itemQuestActive)
            {
                itemQuest.gameObject.SetActive(true);
            }
        }
    }
}