using System;
using System.Collections.Generic;
using Kamaria.Player.Data.Quest;
using TMPro;
using UnityEngine;

namespace Kamaria.UI.UIMainGame
{
    public sealed class UIQuest : MonoBehaviour
    {
        [SerializeField] private QuestManagerSO questManagerSO;
        [SerializeField] private TextMeshProUGUI nameQuest;
        [SerializeField] private TextMeshProUGUI detailQuest;
        [SerializeField] private List<UIRequirementQuest> requirementQuests = new List<UIRequirementQuest>();
        [SerializeField] private List<UIRewardQuest> rewardQuest = new List<UIRewardQuest>();

        private void OnEnable()
        {
            questManagerSO.UpdateProgress += UpdateProgress;
            SetUI();
        }

        private void OnDisable()
        {
            questManagerSO.UpdateProgress -= UpdateProgress;
        }

        public void SetUI()
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
            
            if (questManagerSO.CurrentQuests == null) return;
            
            nameQuest.gameObject.SetActive(true);
            detailQuest.gameObject.SetActive(true);
            
            nameQuest.text = questManagerSO.CurrentQuests.TextShowNameQuest;
            detailQuest.text = questManagerSO.CurrentQuests.DetailQuest;

            UpdateProgress();
        }

        public void UpdateProgress()
        {
            if (questManagerSO.CurrentQuests == null) return;
            
            for (int i = 0; i < questManagerSO.CurrentQuests.RequirementQuests.Count; i++)
            {
                requirementQuests[i].Parent.gameObject.SetActive(true);
                requirementQuests[i].Requirement.text =
                    questManagerSO.CurrentQuests.RequirementQuests[i].RequirementText;
                requirementQuests[i].CountProgress.text =
                    $"x {questManagerSO.CurrentQuests.RequirementQuests[i].CurrentCount} / {questManagerSO.CurrentQuests.RequirementQuests[i].Count}";
            }

            for (int i = 0; i < questManagerSO.CurrentQuests.ItemsReword.Count; i++)
            {
                rewardQuest[i].Parent.gameObject.SetActive(true);
                rewardQuest[i].Reward.sprite = questManagerSO.CurrentQuests.ItemsReword[i].Info.Image[Index.Start];
                rewardQuest[i].Count.text = questManagerSO.CurrentQuests.ItemsReword[i].Info.Count.ToString();
            }
        }
    }
}