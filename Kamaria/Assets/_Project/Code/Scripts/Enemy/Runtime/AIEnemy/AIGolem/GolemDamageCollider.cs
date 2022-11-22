using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Enemy.AIEnemy.Golem
{
    public enum GolemCollidersDamage
    {
        NormalAttack,
        SpineAttack
    }
    
    public sealed class GolemDamageCollider : MonoBehaviour
    {
        [SerializeField] private GolemCollidersDamage colliderDamage;
        [SerializeField] private int damagePercent;
        [SerializeField] private EffectAttack effectAttack;

        public GolemCollidersDamage CollidersDamage => colliderDamage;
        public int DamagePercent => damagePercent;
        public EffectAttack EffectAttack => effectAttack;
    }
}