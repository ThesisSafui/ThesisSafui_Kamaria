using System;
using Kamaria.Manager;

namespace Kamaria.Player.Data.Quest
{
    [Serializable]
    public sealed class MainQuest11 : BaseQuest
    {
        public MainQuest11()
        {
            IndexNumberQuest = 10;
            NameQuest = NameQuest.KingOfTheSea;
            TeleportToDungeons = Dungeons.None;
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
            TextShowNameQuest = "Defeat the boss skeleton pirate (BOSS)";
            ResetProgress();
        }

        public override void GetData(PlayerData playerData)
        {
            IndexNumberQuest = playerData.Quest.MainQuest.Quest11.IndexNumberQuest;
            NameQuest = playerData.Quest.MainQuest.Quest11.NameQuest;
            TextShowNameQuest = playerData.Quest.MainQuest.Quest11.TextShowNameQuest;
            ItemsReword = playerData.Quest.MainQuest.Quest11.ItemsReword;
            IsSucceed = playerData.Quest.MainQuest.Quest11.IsSucceed;
            IsDoing =  playerData.Quest.MainQuest.Quest11.IsDoing;
            RequirementQuests = playerData.Quest.MainQuest.Quest11.RequirementQuests;
            DetailQuest = playerData.Quest.MainQuest.Quest11.DetailQuest;
            //ItemsReword = playerData.Quest.MainQuest.Quest11.ItemsReword;
            TeleportToDungeons = playerData.Quest.MainQuest.Quest11.TeleportToDungeons;
        }
    }
}