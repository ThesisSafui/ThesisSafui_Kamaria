using System;
using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Enemy.AIEnemy.SharkBoss
{
    public enum SharkBossCollidersDamage
    {
        NormalAttack1, NormalAttack2,
        SlamAttack, SlamEndAttack, RushAttack
    }
    
    public sealed class SharkBossDamageCollider : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private SharkBossCollidersDamage colliderDamage;
        [SerializeField] private int damagePercent;
        [SerializeField] private EffectAttack effectAttack;
        public int CountStun { get; set; }
        public int CurrentStun { get; set; }
        public SharkBossCollidersDamage CollidersDamage => colliderDamage;
        public int DamagePercent => damagePercent;
        public EffectAttack EffectAttack => effectAttack;

        public void RestCountStun()
        {
            CountStun = 1;
            CurrentStun = 0;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IDamageable target)) return;
            if (playerData.CharacterControllerData.Dashing) return;
            
            if (colliderDamage == SharkBossCollidersDamage.SlamAttack)
            {
                CurrentStun++;
            }
        }
    }
}