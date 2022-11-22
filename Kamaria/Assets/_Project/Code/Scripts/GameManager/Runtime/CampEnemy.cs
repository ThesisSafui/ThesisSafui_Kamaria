using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kamaria.Manager
{
    [Serializable]
    public sealed class CampEnemy
    {
        [SerializeField] private List<GameObject> enemies;
        [SerializeField] private Transform spawnsEnemyPos;

        public List<GameObject> Enemies => enemies;
        public Transform SpawnsEnemyPos => spawnsEnemyPos;
    }
}