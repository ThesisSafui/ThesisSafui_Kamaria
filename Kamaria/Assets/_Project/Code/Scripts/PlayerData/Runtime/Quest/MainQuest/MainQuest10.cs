using System;
using Kamaria.Manager;

namespace Kamaria.Player.Data.Quest
{
    [Serializable]
    public sealed class MainQuest10 : BaseQuest
    {
        public MainQuest10()
        {
            IndexNumberQuest = 9;
            NameQuest = NameQuest.DefeatTheInvader03;
            TeleportToDungeons = Dungeons.BootyCove;
            QuestTypes = QuestTypes.Main;
        }
        
        public override void StartQuest(PlayerData playerData)
        {
            playerData.Quest.ResetQuestDoing();
            IsDoing = true;
        }
        
        public override void GetReword(PlayerData playerData)
        {
            if (!IsSucceed) return;
            playerData.Quest.MainQuest.indexCurrentQuest = IndexNumberQuest + 1;
            IsDoing = false;
            IsSucceed = true;
        }

        public override void UpdateProgress()
        {
            var notFinish = RequirementQuests.Find(x => x.IsFinish == false);
            
            if (notFinish==null)
            {
                IsSucceed = true;
            }
        }

        public override void ResetProgress()
        {
            for (int i = 0; i < RequirementQuests.Count; i++)
            {
                RequirementQuests[i].CurrentCount = 0;
            }

            IsDoing = false;
            IsSucceed = false;
        }

        public override void Initialized()
        {
            TextShowNameQuest = "Defeat the invader 03";
            ResetProgress();
        }

        public override void GetData(PlayerData playerData)
        {
            IndexNumberQuest = playerData.Quest.MainQuest.Quest10.IndexNumberQuest;
            NameQuest = playerData.Quest.MainQuest.Quest10.NameQuest;
            TextShowNameQuest = playerData.Quest.MainQuest.Quest10.TextShowNameQuest;
            ItemsReword = playerData.Quest.MainQuest.Quest10.ItemsReword;
            IsSucceed = playerData.Quest.MainQuest.Quest10.IsSucceed;
            IsDoing =  playerData.Quest.MainQuest.Quest10.IsDoing;
            RequirementQuests = playerData.Quest.MainQuest.Quest10.RequirementQuests;
            DetailQuest = playerData.Quest.MainQuest.Quest10.DetailQuest;
            //ItemsReword = playerData.Quest.MainQuest.Quest10.ItemsReword;
            TeleportToDungeons = playerData.Quest.MainQuest.Quest10.TeleportToDungeons;
        }
    }
}