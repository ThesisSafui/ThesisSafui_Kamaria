using System;
using Kamaria.Manager;

namespace Kamaria.Player.Data.Quest
{
    [Serializable]
    public sealed class SideQuest1 : BaseQuest
    {
        public SideQuest1()
        {
            IndexNumberQuest = 0;
            NameQuest = NameQuest.FindKeystonesLocation;
            TeleportToDungeons = Dungeons.BootyCove;
            QuestTypes = QuestTypes.Side1;
        }
        
        public override void StartQuest(PlayerData playerData)
        {
            playerData.Quest.ResetQuestDoing();
            IsDoing = true;
        }

        public override void GetReword(PlayerData playerData)
        {
            if (!IsSucceed) return;
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
            TextShowNameQuest = "Find keystones location";
            ResetProgress();
        }

        public override void GetData(PlayerData playerData)
        {
            IndexNumberQuest = playerData.Quest.SideQuest.Quest1.IndexNumberQuest;
            NameQuest = playerData.Quest.SideQuest.Quest1.NameQuest;
            TextShowNameQuest = playerData.Quest.SideQuest.Quest1.TextShowNameQuest;
            ItemsReword = playerData.Quest.SideQuest.Quest1.ItemsReword;
            IsSucceed = playerData.Quest.SideQuest.Quest1.IsSucceed;
            IsDoing =  playerData.Quest.SideQuest.Quest1.IsDoing;
            RequirementQuests = playerData.Quest.SideQuest.Quest1.RequirementQuests;
            DetailQuest = playerData.Quest.SideQuest.Quest1.DetailQuest;
            //ItemsReword = playerData.Quest.SideQuest.Quest1.ItemsReword;
            TeleportToDungeons = playerData.Quest.SideQuest.Quest1.TeleportToDungeons;
        }
    }
}