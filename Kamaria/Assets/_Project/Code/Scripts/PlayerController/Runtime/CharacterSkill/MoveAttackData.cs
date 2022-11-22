using System;
using UnityEngine;

namespace Kamaria.Player.Controller
{
    [Serializable]
    public sealed class MoveAttackData
    {
        [SerializeField] private float acceleration;
        [SerializeField] private float time;
        
        public float Time => time;
        public float Acceleration => acceleration;
    }
}