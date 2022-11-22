using Kamaria.Utilities.PoolingPattern;
using UnityEngine;

namespace Kamaria.Player.Controller
{
    public sealed class Grenade : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float time = 1f;
        private PoolManager.PoolObjectType explosionPlayerLv;

        public void Init(Vector3 target, Vector3 origin, PoolManager.PoolObjectType explosionPlayerLv)
        {
            this.explosionPlayerLv = explosionPlayerLv switch
            {
                PoolManager.PoolObjectType.GrenadeLv1 => PoolManager.PoolObjectType.ExplosionPlayerLv1,
                PoolManager.PoolObjectType.GrenadeLv2 => PoolManager.PoolObjectType.ExplosionPlayerLv2,
                _ => this.explosionPlayerLv
            };
            
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
            PoolExplosion();
            
            gameObject.SetActive(false);
        }

        private void PoolExplosion()
        {
            var explosion = PoolManager.Instance.GetPooledObject(explosionPlayerLv);

            if (explosion)
            {
                explosion.transform.position = this.transform.position;
                explosion.transform.rotation = Quaternion.identity;
                explosion.SetActive(true);
            }
        }
    }
}