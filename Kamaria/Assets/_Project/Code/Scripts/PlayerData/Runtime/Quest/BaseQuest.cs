using System;
using System.Collections.Generic;
using Kamaria.Item;
using Kamaria.Manager;
using UnityEngine;

namespace Kamaria.Player.Data.Quest
{
    public enum QuestTypes
    {
        Main, Side1, Side2
    }
    public enum NameQuest
    {
        FirstFightInTheNewWorld, TheFirstCraft, FindTheTreasure, TheLostMemories, TrackingTheTeleport,
        DefeatTheBossSharkPirate, DefeatTheInvade, NoPainNoGain, DefeatTheInvader02,
        DefeatTheInvader03, KingOfTheSea,
        FindKeystonesLocation, KeystonesPower 
    }
    
    [Serializable]
    public abstract class BaseQuest
    {
        public QuestTypes QuestTypes;
        public int IndexNumberQuest;
        public NameQuest NameQuest;
        public string TextShowNameQuest;
        public bool IsDoing;
        public bool IsSucceed;
        public List<RequirementQuest> RequirementQuests;
        [TextArea(minLines: 1, maxLines: 10)] public string DetailQuest;
        public List<BaseItemSO> ItemsReword = new List<BaseItemSO>();
        public Dungeons TeleportToDungeons;

        public abstract void StartQuest(PlayerData playerData);

        public abstract void GetReword(PlayerData playerData);

        public abstract void UpdateProgress();
        public abstract void ResetProgress();

        public abstract void Initialized();

        public abstract void GetData(PlayerData playerData);
    }
}