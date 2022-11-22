using Kamaria.Item;
using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Enemy.AIEnemy.SkeletonBoss
{
    public sealed class MeteorDamageCollider : MonoBehaviour
    {
        [SerializeField] private int damagePercent;
        
        private int damage;
        private EffectAttack effectAttack;
        
        public void InitDamage(int damage, EffectAttack effectAttack, Vector3 explosionPos, float powerKnockback,
            float radiusKnockback, bool isAOE, float stunTime)
        {
            this.damage = (int)(damage * damagePercent) / 100;
            this.effectAttack = effectAttack;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IDamageable target)) return;
            
            target.TakeDamage(damage, effectAttack, this.transform.position, 0, 0,
                false, 0, WeaponTypes.None, KeyStones.None);
            
            gameObject.SetActive(false);
        }
    }
}