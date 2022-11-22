using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Enemy.AIEnemy.Mouse
{
    public enum MouseCollidersDamage
    {
        NormalAttack,
        SpineAttack,
        JumpAttack
    }
    
    public sealed class MouseDamageCollider : MonoBehaviour
    {
        [SerializeField] private MouseCollidersDamage colliderDamage;
        [SerializeField] private int damagePercent;
        [SerializeField] private EffectAttack effectAttack;

        public MouseCollidersDamage CollidersDamage => colliderDamage;
        public int DamagePercent => damagePercent;
        public EffectAttack EffectAttack => effectAttack;
    }
}