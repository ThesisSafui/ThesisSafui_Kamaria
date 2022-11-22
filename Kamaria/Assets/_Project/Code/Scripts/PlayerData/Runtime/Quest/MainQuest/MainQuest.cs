using System;
using System.Collections.Generic;

namespace Kamaria.Player.Data.Quest
{
    [Serializable]
    public sealed class MainQuest : IPlayerData
    {
        public MainQuest1 Quest1 = new MainQuest1();
        public MainQuest2 Quest2 = new MainQuest2();
        public MainQuest3 Quest3 = new MainQuest3();
        public MainQuest4 Quest4 = new MainQuest4();
        public MainQuest5 Quest5 = new MainQuest5();
        public MainQuest6 Quest6 = new MainQuest6();
        public MainQuest7 Quest7 = new MainQuest7();
        public MainQuest8 Quest8 = new MainQuest8();
        public MainQuest9 Quest9 = new MainQuest9();
        public MainQuest10 Quest10 = new MainQuest10();
        public MainQuest11 Quest11 = new MainQuest11();

        public int indexCurrentQuest;
        
        public List<BaseQuest> MainQuestAll = new List<BaseQuest>()
        {
            new MainQuest1(), new MainQuest2(), new MainQuest3(), new MainQuest4(), new MainQuest5(), new MainQuest6(),
            new MainQuest7(), new MainQuest8(), new MainQuest9(), new MainQuest10(), new MainQuest11()
        };

        public void Initialized()
        {
            indexCurrentQuest = 0;
            
            Quest1.Initialized();
            Quest2.Initialized();
            Quest3.Initialized();
            Quest4.Initialized();
            Quest5.Initialized();
            Quest6.Initialized();
            Quest7.Initialized();
            Quest8.Initialized();
            Quest9.Initialized();
            Quest10.Initialized();
            Quest11.Initialized();

            MainQuestAll[0] = Quest1;
            MainQuestAll[1] = Quest2;
            MainQuestAll[2] = Quest3;
            MainQuestAll[3] = Quest4;
            MainQuestAll[4] = Quest5;
            MainQuestAll[5] = Quest6;
            MainQuestAll[6] = Quest7;
            MainQuestAll[7] = Quest8;
            MainQuestAll[8] = Quest9;
            MainQuestAll[9] = Quest10;
            MainQuestAll[10] = Quest11;
        }

        public void GetData(PlayerData playerData)
        {
            indexCurrentQuest = playerData.Quest.MainQuest.indexCurrentQuest;
            
            Quest1.GetData(playerData);
            Quest2.GetData(playerData);
            Quest3.GetData(playerData);
            Quest4.GetData(playerData);
            Quest5.GetData(playerData);
            Quest6.GetData(playerData);
            Quest7.GetData(playerData);
            Quest8.GetData(playerData);
            Quest9.GetData(playerData);
            Quest10.GetData(playerData);
            Quest11.GetData(playerData);
        }
    }
}