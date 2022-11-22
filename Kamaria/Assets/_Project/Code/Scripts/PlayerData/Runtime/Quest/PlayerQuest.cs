using System;

namespace Kamaria.Player.Data.Quest
{
    [Serializable]
    public sealed class PlayerQuest : IPlayerData
    {
        public MainQuest MainQuest = new MainQuest();
        public SideQuest SideQuest = new SideQuest();
        
        public void Initialized()
        {
            MainQuest.Initialized();
            SideQuest.Initialized();
        }

        public void GetData(PlayerData playerData)
        {
            MainQuest.GetData(playerData);
            SideQuest.GetData(playerData);
        }
        
        public void ResetQuestDoing()
        {
            for (int i = 0; i < MainQuest.MainQuestAll.Count; i++)
            {
                MainQuest.MainQuestAll[i].IsDoing = false;
            }
            
            for (int i = 0; i < SideQuest.SideQuestAll.Count; i++)
            {
                SideQuest.SideQuestAll[i].IsDoing = false;
            }
        }
    }
}