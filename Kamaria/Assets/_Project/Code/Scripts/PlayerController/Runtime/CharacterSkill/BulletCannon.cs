using Kamaria.Utilities.PoolingPattern;
using Kamaria.VFX_ALL;
using UnityEngine;

namespace Kamaria.Player.Controller
{
    public sealed class BulletCannon : MonoBehaviour
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float time = 1f;

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
            PoolExplosion();
            
            gameObject.SetActive(false);
        }

        private void PoolExplosion()
        {
            PoolManager.PoolObjectType poolObj = PoolManager.PoolObjectType.ExplosionCannon;
            
            var explosion = PoolManager.Instance.GetPooledObject(poolObj);

            if (explosion)
            {
                explosion.transform.position = this.transform.position;
                explosion.transform.rotation = Quaternion.identity;
                explosion.GetComponent<VfxShockWave>().Init(transform.position);
                explosion.SetActive(true);
            }
        }
    }
}