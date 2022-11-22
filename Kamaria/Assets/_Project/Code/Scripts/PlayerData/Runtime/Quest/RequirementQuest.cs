using System;
using UnityEngine;

namespace Kamaria.Player.Data.Quest
{
    public enum QuestRequirement
    {
        UseSwordToKill3Enemies, UseKnuckleToKill3Enemies, UseGunToKill3Enemies,
        UpgradeAnyWeaponInAnyPart, Find2TreasureBoxesInTheAbandonMapBootyCove, Get3MemoryChip,
        PlayerKnowsTheBossLocation, DefeatBossSharkPirate1, Defeat20MonstersInAbandonIslandDeadMansHeaven,
        DefeatBossSharkPirate2, Defeat20MonstersInAbandonIslandDeadIsle, Defeat20MonstersInAbandonIslandBootyCove,
        DefeatBossSkeletonPirate, PlayerGetWindKeystone, 
        HintOfArtifactMapBootyCove, HintOfArtifactMapDeadIsle, HintOfArtifactMapDeadMansHeaven, OfTheLastBossMap1
        , OfTheLastBossMap2, OfTheLastBossMap3, HintOfArtifactMapBossShark
    }
    
    [Serializable]
    public sealed class RequirementQuest
    {
        [SerializeField] private QuestRequirement questRequirement;
        [SerializeField] private string requirementText;
        [SerializeField] private int count;

        public int CurrentCount;
        public QuestRequirement QuestRequirement => questRequirement;
        public string RequirementText => requirementText;
        public int Count => count;
        public bool IsFinish => CurrentCount == count;
    }
}