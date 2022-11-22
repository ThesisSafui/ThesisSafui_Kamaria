using Kamaria.Manager;
using Kamaria.Player.Data;
using Kamaria.Player.Data.Quest;
using UnityEngine;

namespace Kamaria.ObjInteract
{
    public sealed class ItemQuestInteract : MonoBehaviour
    {
        [SerializeField] private QuestManagerSO questManagerSO;
        [SerializeField] private ItemQuest itemQuest;
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
                GetItemQuest();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            playerData.CanInteract = false;
            uiInteract.gameObject.SetActive(false);
        }
        
        private void GetItemQuest()
        {
            if (questManagerSO.CurrentQuests == null) return;

            if (questManagerSO.CurrentQuests.NameQuest == NameQuest.TheLostMemories)
            {
                if (!QuestTheLostMemories()) return;
            }
            else if (questManagerSO.CurrentQuests.NameQuest == NameQuest.TrackingTheTeleport)
            {
                if (!QuestTrackingTheTeleport()) return;
            }
            else if (questManagerSO.CurrentQuests.NameQuest == NameQuest.DefeatTheInvade)
            {
                if (!QuestDefeatTheInvader(questManagerSO.CurrentQuests.NameQuest,
                        QuestRequirement.OfTheLastBossMap3)) return;
            }
            else if (questManagerSO.CurrentQuests.NameQuest == NameQuest.DefeatTheInvader02)
            {
                if (!QuestDefeatTheInvader(questManagerSO.CurrentQuests.NameQuest,
                        QuestRequirement.OfTheLastBossMap2)) return;
            }
            else if (questManagerSO.CurrentQuests.NameQuest == NameQuest.DefeatTheInvader03)
            {
                if (!QuestDefeatTheInvader(questManagerSO.CurrentQuests.NameQuest,
                        QuestRequirement.OfTheLastBossMap1)) return;
            }

            uiManager.NotifiedGetItem(itemQuest.Item);
            
            this.gameObject.SetActive(false);
        }

        private bool QuestTheLostMemories()
        {
            if (!questManagerSO.CanDoQuest(NameQuest.TheLostMemories)
                || questManagerSO.CurrentQuests.IsSucceed) return false;

            var questRequirement =
                questManagerSO.CurrentQuests.RequirementQuests.Find(x =>
                    x.QuestRequirement == QuestRequirement.Get3MemoryChip);

            if (questRequirement == null) return false;
            questManagerSO.UpdateProgressQuest(questRequirement.QuestRequirement,out bool finish);
            questManagerSO.CurrentQuests.UpdateProgress();
            return true;
        }

        private bool QuestTrackingTheTeleport()
        {
            if (!questManagerSO.CanDoQuest(NameQuest.TrackingTheTeleport)
                || questManagerSO.CurrentQuests.IsSucceed) return false;

            var questRequirement =
                questManagerSO.CurrentQuests.RequirementQuests.Find(x =>
                    x.QuestRequirement == QuestRequirement.PlayerKnowsTheBossLocation);

            if (questRequirement == null) return false;
            
            questManagerSO.UpdateProgressQuest(questRequirement.QuestRequirement,out bool finish);
            questManagerSO.CurrentQuests.UpdateProgress();
            return true;
        }

        private bool QuestDefeatTheInvader(NameQuest nameQuest ,QuestRequirement questRequirement)
        {
            if (!questManagerSO.CanDoQuest(nameQuest) || questManagerSO.CurrentQuests.IsSucceed) return false;

            var questRequirementLastBossMap =
                questManagerSO.CurrentQuests.RequirementQuests.Find(x =>
                    x.QuestRequirement == questRequirement);

            if (questRequirementLastBossMap == null) return false;
            
            questManagerSO.UpdateProgressQuest(questRequirementLastBossMap.QuestRequirement,out bool finish);
            questManagerSO.CurrentQuests.UpdateProgress();
            return true;
        }
    }
}