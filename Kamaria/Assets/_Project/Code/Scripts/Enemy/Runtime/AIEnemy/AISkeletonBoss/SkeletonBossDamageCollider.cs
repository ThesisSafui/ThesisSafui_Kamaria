using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Enemy.AIEnemy.SkeletonBoss
{
    public enum SkeletonBossCollidersDamage
    {
        NormalAttack, PistolAttack, MeteorAttack
    }
    public sealed class SkeletonBossDamageCollider : MonoBehaviour
    {
        [SerializeField] private SkeletonBossCollidersDamage colliderDamage;
        [SerializeField] private int damagePercent;
        [SerializeField] private EffectAttack effectAttack;
       
        public SkeletonBossCollidersDamage CollidersDamage => colliderDamage;
        public int DamagePercent => damagePercent;
        public EffectAttack EffectAttack => effectAttack;
    }
}