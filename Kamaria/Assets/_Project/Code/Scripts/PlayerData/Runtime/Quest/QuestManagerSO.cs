using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kamaria.Player.Data.Quest
{
    [CreateAssetMenu(fileName = "New QuestManager", menuName = "ThesisSafui/Data/Quest")]
    public class QuestManagerSO : ScriptableObject
    {
        public event Action UpdateProgress;
        
        [SerializeField] private PlayerDataSO playerData;
        
        public List<BaseQuest> MainQuests => playerData.Info.Quest.MainQuest.MainQuestAll;
        public List<BaseQuest> SideQuests => playerData.Info.Quest.SideQuest.SideQuestAll;
        public BaseQuest CurrentMainQuests => playerData.Info.Quest.MainQuest.MainQuestAll.Find(x => x.IsDoing);
        public BaseQuest CurrentSideQuests => playerData.Info.Quest.SideQuest.SideQuestAll.Find(x => x.IsDoing);
        public BaseQuest CurrentQuests => CurrentMainQuests ?? CurrentSideQuests;

        public void UpdateProgressQuest(QuestRequirement questRequirement, out bool finish)
        {
            var quest = CurrentQuests.RequirementQuests.Find(x => x.QuestRequirement == questRequirement);
            finish = false;

            if (quest == null) return;
            
            if (!quest.IsFinish)
            {
                quest.CurrentCount++;
            }
            
            finish = quest.IsFinish;
            UpdateProgress?.Invoke();
        }
        
        public bool CanDoQuest(NameQuest nameQuest)
        {
            if (CurrentQuests == null) return false;
            
            return CurrentQuests.NameQuest == nameQuest;
        }
    }
}