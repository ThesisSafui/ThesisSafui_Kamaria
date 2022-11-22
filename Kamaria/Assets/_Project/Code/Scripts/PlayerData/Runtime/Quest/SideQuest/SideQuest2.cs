using System;
using Kamaria.Manager;

namespace Kamaria.Player.Data.Quest
{
    [Serializable]
    public sealed class SideQuest2 : BaseQuest
    {
        public SideQuest2()
        {
            IndexNumberQuest = 1;
            NameQuest = NameQuest.KeystonesPower;
            TeleportToDungeons = Dungeons.BootyCove;
            QuestTypes = QuestTypes.Side2;
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
            
            if (notFinish == null)
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
            TextShowNameQuest = "Keystones power";
            ResetProgress();
        }

        public override void GetData(PlayerData playerData)
        {
            IndexNumberQuest = playerData.Quest.SideQuest.Quest2.IndexNumberQuest;
            NameQuest = playerData.Quest.SideQuest.Quest2.NameQuest;
            TextShowNameQuest = playerData.Quest.SideQuest.Quest2.TextShowNameQuest;
            ItemsReword = playerData.Quest.SideQuest.Quest2.ItemsReword;
            IsSucceed = playerData.Quest.SideQuest.Quest2.IsSucceed;
            IsDoing =  playerData.Quest.SideQuest.Quest2.IsDoing;
            RequirementQuests = playerData.Quest.SideQuest.Quest2.RequirementQuests;
            DetailQuest = playerData.Quest.SideQuest.Quest2.DetailQuest;
            //ItemsReword = playerData.Quest.SideQuest.Quest2.ItemsReword;
            TeleportToDungeons = playerData.Quest.SideQuest.Quest2.TeleportToDungeons;
        }
    }
}