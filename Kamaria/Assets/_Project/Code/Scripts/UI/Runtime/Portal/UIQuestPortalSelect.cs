using System;
using System.Collections.Generic;
using Kamaria.Player.Data;
using Kamaria.Player.Data.Quest;
using Kamaria.UI.UIMainGame;
using TMPro;
using UnityEngine;

namespace Kamaria.UI.Portal
{
    [Serializable]
    public sealed class UIQuestPortalSelect
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private QuestManagerSO questManagerSO;
        [SerializeField] private RectTransform parent;
        [SerializeField] private TextMeshProUGUI nameQuest;
        [SerializeField] private TextMeshProUGUI detailQuest;
        [SerializeField] private List<UIRequirementQuest> requirementQuests = new List<UIRequirementQuest>();
        [SerializeField] private List<UIRewardQuest> rewardQuest = new List<UIRewardQuest>();

        public RectTransform Parent => parent;
        public TextMeshProUGUI NameQuest => nameQuest;
        public TextMeshProUGUI DetailQuest => detailQuest;
        public List<UIRequirementQuest> RequirementQuests => requirementQuests;
        public List<UIRewardQuest> RewardQuest => rewardQuest;
        
        public void SetUI(bool isMain ,int indexSide)
        {
            nameQuest.gameObject.SetActive(false);
            detailQuest.gameObject.SetActive(false);
            
            for (int i = 0; i < requirementQuests.Count; i++)
            {
                requirementQuests[i].Parent.gameObject.SetActive(false);
            }

            for (int i = 0; i < rewardQuest.Count; i++)
            {
                rewardQuest[i].Parent.gameObject.SetActive(false);
            }
            
            nameQuest.gameObject.SetActive(true);
            detailQuest.gameObject.SetActive(true);

            var quest = isMain ? questManagerSO.MainQuests : questManagerSO.SideQuests;
            var indexQuest = isMain
                ? playerData.Info.Quest.MainQuest.indexCurrentQuest
                : playerData.Info.Quest.SideQuest.indexCurrentQuest;

            if (!isMain)
            {
                indexQuest = indexSide;
            }
            
            nameQuest.text = quest[indexQuest]
                .TextShowNameQuest;
            detailQuest.text = quest[indexQuest]
                .DetailQuest;

            UpdateProgress(isMain, indexSide);
        }

        private void UpdateProgress(bool isMain, int indexSide)
        {
            var quest = isMain ? questManagerSO.MainQuests : questManagerSO.SideQuests;
            var indexQuest = isMain
                ? playerData.Info.Quest.MainQuest.indexCurrentQuest
                : playerData.Info.Quest.SideQuest.indexCurrentQuest;
            
            if (!isMain)
            {
                indexQuest = indexSide;
            }
            
            for (int i = 0; i < quest[indexQuest].RequirementQuests.Count; i++)
            {
                requirementQuests[i].Parent.gameObject.SetActive(true);
                requirementQuests[i].Requirement.text =
                    quest[indexQuest].RequirementQuests[i].RequirementText;
                requirementQuests[i].CountProgress.text =
                    $"x { quest[indexQuest].RequirementQuests[i].CurrentCount} / { quest[indexQuest].RequirementQuests[i].Count}";
            }

            for (int i = 0; i < quest[indexQuest].ItemsReword.Count; i++)
            {
                rewardQuest[i].Parent.gameObject.SetActive(true);
                rewardQuest[i].Reward.sprite =  quest[indexQuest].ItemsReword[i].Info.Image[Index.Start];
                rewardQuest[i].Count.text =  quest[indexQuest].ItemsReword[i].Info.Count.ToString();
            }
        }
    }
}