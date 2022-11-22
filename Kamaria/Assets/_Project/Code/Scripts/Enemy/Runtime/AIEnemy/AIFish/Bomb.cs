using System;
using Kamaria.Item;
using Kamaria.Player.Data;
using Kamaria.Utilities.PoolingPattern;
using UnityEngine;

namespace Kamaria.Enemy.AIEnemy.Fish
{
    public sealed class Bomb : MonoBehaviour
    {
        [SerializeField] private int damagePercent;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float time = 1f;
        [SerializeField] private float radiusDamage;
        [SerializeField] private LayerMask playerLayer;

        private int damage;
        private EffectAttack effectAttack;

        public void InitDamage(int damage, EffectAttack effectAttack, Vector3 explosionPos, float powerKnockback,
            float radiusKnockback, bool isAOE, float stunTime)
        {
            this.damage = (int)(damage * damagePercent) / 100;
            this.effectAttack = effectAttack;
        }
        
        public void Init(Vector3 target, Vector3 origin)
        {
            Move(target, origin);
        }

        private void Move(Vector3 target, Vector3 origin)
        {
            rb.velocity = CalculateProjectile(target, origin);
        }

        private Vector3 CalculateProjectile(Vector3 target, Vector3 origin)
        {
            var distance = target - origin;
            var distanceVectorX = distance;
            distanceVectorX.y = 0f;

            var distanceScalarX = distanceVectorX.magnitude;
            var distanceScalarY = distance.y;

            var velocityX = distanceScalarX / time;
            var velocityY = distanceScalarY / time + 0.5f * Mathf.Abs(Physics.gravity.y) * time;

            var result = distanceVectorX.normalized;
            result *= velocityX;
            result.y = velocityY;

            return result;
        }

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log(other.name);
            Collider[] hitColliders = new Collider[1];
            int numColliders =
                Physics.OverlapSphereNonAlloc(this.transform.position, radiusDamage, hitColliders, playerLayer);

            for (int i = 0; i < numColliders; i++)
            {
                if (!hitColliders[i].TryGetComponent(out IDamageable target)) return;

                target.TakeDamage(damage, effectAttack, this.transform.position, 0, radiusDamage,
                    false, 0, WeaponTypes.None, KeyStones.None);
            }

            PoolExplosion();
            
            gameObject.SetActive(false);
        }

        private void PoolExplosion()
        {
            PoolManager.PoolObjectType poolObj = PoolManager.PoolObjectType.Explosion;
            
            var explosion = PoolManager.Instance.GetPooledObject(poolObj);

            if (explosion)
            {
                explosion.transform.position = this.transform.position;
                explosion.transform.rotation = Quaternion.identity;
                explosion.SetActive(true);
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(this.transform.position, radiusDamage);
        }
#endif
    }
}