using System;
using System.Collections.Generic;

namespace Kamaria.Player.Data.Quest
{
    [Serializable]
    public sealed class SideQuest : IPlayerData
    {
        public SideQuest1 Quest1 = new SideQuest1();
        public SideQuest2 Quest2 = new SideQuest2();
        
        public int indexCurrentQuest;
        
        public List<BaseQuest> SideQuestAll = new List<BaseQuest>()
        {
            new SideQuest1(), new SideQuest2()
        };

        public void Initialized()
        {
            indexCurrentQuest = 0;
            
            Quest1.Initialized();
            Quest2.Initialized();

            SideQuestAll[0] = Quest1;
            SideQuestAll[1] = Quest2;
        }

        public void GetData(PlayerData playerData)
        {
            indexCurrentQuest = playerData.Quest.SideQuest.indexCurrentQuest;
            
            Quest1.GetData(playerData);
            Quest2.GetData(playerData);
        }
    }
}