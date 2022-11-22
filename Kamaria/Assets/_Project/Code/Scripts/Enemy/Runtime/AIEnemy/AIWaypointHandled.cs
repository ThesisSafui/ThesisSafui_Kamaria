using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kamaria.Enemy.AIEnemy
{
    [Serializable]
    public class AIWaypointHandled
    {
        [SerializeField] private List<Transform> waypoints = new List<Transform>();

        public List<Transform> Waypoints => waypoints;
    }
}