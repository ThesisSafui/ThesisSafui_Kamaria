using System;
using Kamaria.Manager;

namespace Kamaria.Player.Data.Quest
{
    [Serializable]
    public sealed class MainQuest5 : BaseQuest
    {
        public MainQuest5()
        {
            IndexNumberQuest = 4;
            NameQuest = NameQuest.TrackingTheTeleport;
            TeleportToDungeons = Dungeons.DeadIsle;
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
            TextShowNameQuest = "Tracking the teleport";
            ResetProgress();
        }

        public override void GetData(PlayerData playerData)
        {
            IndexNumberQuest = playerData.Quest.MainQuest.Quest5.IndexNumberQuest;
            NameQuest = playerData.Quest.MainQuest.Quest5.NameQuest;
            TextShowNameQuest = playerData.Quest.MainQuest.Quest5.TextShowNameQuest;
            ItemsReword = playerData.Quest.MainQuest.Quest5.ItemsReword;
            IsSucceed = playerData.Quest.MainQuest.Quest5.IsSucceed;
            IsDoing =  playerData.Quest.MainQuest.Quest5.IsDoing;
            RequirementQuests = playerData.Quest.MainQuest.Quest5.RequirementQuests;
            DetailQuest = playerData.Quest.MainQuest.Quest5.DetailQuest;
            //ItemsReword = playerData.Quest.MainQuest.Quest5.ItemsReword;
            TeleportToDungeons = playerData.Quest.MainQuest.Quest5.TeleportToDungeons;
        }
    }
}