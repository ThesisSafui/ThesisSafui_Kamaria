using System;
using Kamaria.Manager;

namespace Kamaria.Player.Data.Quest
{
    [Serializable]
    public sealed class MainQuest9 : BaseQuest
    {
        public MainQuest9()
        {
            IndexNumberQuest = 8;
            NameQuest = NameQuest.DefeatTheInvader02;
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
            TextShowNameQuest = "Defeat the invader 02";
            ResetProgress();
        }

        public override void GetData(PlayerData playerData)
        {
            IndexNumberQuest = playerData.Quest.MainQuest.Quest9.IndexNumberQuest;
            NameQuest = playerData.Quest.MainQuest.Quest9.NameQuest;
            TextShowNameQuest = playerData.Quest.MainQuest.Quest9.TextShowNameQuest;
            ItemsReword = playerData.Quest.MainQuest.Quest9.ItemsReword;
            IsSucceed = playerData.Quest.MainQuest.Quest9.IsSucceed;
            IsDoing =  playerData.Quest.MainQuest.Quest9.IsDoing;
            RequirementQuests = playerData.Quest.MainQuest.Quest9.RequirementQuests;
            DetailQuest = playerData.Quest.MainQuest.Quest9.DetailQuest;
            //ItemsReword = playerData.Quest.MainQuest.Quest9.ItemsReword;
            TeleportToDungeons = playerData.Quest.MainQuest.Quest9.TeleportToDungeons;
        }
    }
}