using System;
using Kamaria.Enemy.AIEnemy.Mouse;
using Kamaria.Item;
using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Enemy.AIEnemy
{
    public sealed class EnemyDamageCollider : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private BaseAI ai;
        [SerializeField] private bool isOneHit;
        [SerializeField] private bool isAOE;
        [SerializeField] private float powerKnockback;
        [SerializeField] private float radiusKnockback;
        [SerializeField] private float stunTime;

        private delegate void Hit();
        private AIMouse aiMouse;
       

        private void OnTriggerStay(Collider other)
        {
            if (!other.TryGetComponent(out IDamageable target)) return;
            if (playerData.CharacterControllerData.Dashing || playerData.CharacterSkillData.UsingGuard) return;
            
            target.TakeDamage(ai.Damage, ai.EffectAttack, this.transform.position, powerKnockback, radiusKnockback,
                isAOE, stunTime, WeaponTypes.None, KeyStones.None);

            Hit hit = isOneHit ? OneHit : ManyHit;

            hit();
        }

        private void OneHit()
        {
            this.gameObject.SetActive(false);
        }

        private void ManyHit()
        {
            
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(this.transform.position, radiusKnockback);
        }
#endif
        
    }
}