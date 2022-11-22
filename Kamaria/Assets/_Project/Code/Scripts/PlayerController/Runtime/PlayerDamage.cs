using System;
using Kamaria.Item;
using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Player.Controller
{
    [Serializable]
    public sealed class PlayerDamage
    {
        [SerializeField] private CollidersDamage collidersDamage;
        [SerializeField] private int damagePercent;
        [SerializeField] private float damageCharge;
        [SerializeField] private EffectAttack effectAttack;
        [SerializeField] [Range(0, 10)] private int knucklePercentStun;

        public CollidersDamage CollidersDamage => collidersDamage;
        public int DamagePercent => damagePercent;
        public int KnucklePercentStun => knucklePercentStun;
        public float DamageCharge => damageCharge;
        public EffectAttack EffectAttack => effectAttack;
    }
}