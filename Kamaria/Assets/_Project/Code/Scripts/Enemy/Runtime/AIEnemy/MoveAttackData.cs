using System;
using UnityEngine;

namespace Kamaria.Enemy.AIEnemy
{
    [Serializable]
    public sealed class MoveAttackData
    {
        [Tooltip("Will know what's the name.")]
        [SerializeField] private string name;
        [SerializeField] private float moveAttackTime;
        [SerializeField] private float moveAttackAcceleration;
        
        public float MoveAttackTime => moveAttackTime;
        public float MoveAttackAcceleration => moveAttackAcceleration;
    }
}