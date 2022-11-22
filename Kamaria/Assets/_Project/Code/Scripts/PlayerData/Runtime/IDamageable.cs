using Kamaria.Item;
using UnityEngine;

namespace Kamaria.Player.Data
{
    public enum EffectAttack
    {
        None,
        KnockBack,
        Stun
    }
    
    public interface IDamageable
    {
        public void TakeDamage(int damage, EffectAttack effectAttack, Vector3 explosionPos, float powerKnockback,
            float radiusKnockback, bool isAOE, float stunTime, WeaponTypes weaponTypes, KeyStones keyStones);
    }
}