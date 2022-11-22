using System;
using UnityEngine;

namespace Kamaria.Manager
{
    [Serializable]
    public sealed class PatternSpawn
    {
        [SerializeField] private PatternsSpawn patternsSpawn;
        [SerializeField] private Transform spawnsPlayerPos;
        [SerializeField] private GameObject warpGate;
        
        public PatternsSpawn PatternsSpawn => patternsSpawn;
        public Transform SpawnPlayerPos => spawnsPlayerPos;
        public GameObject WarpGate => warpGate;
    }
}