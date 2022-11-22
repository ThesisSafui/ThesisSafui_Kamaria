using System.Collections;
using Kamaria.Utilities.PoolingPattern;
using UnityEngine;

namespace Kamaria.Player.Controller
{
    public sealed class BulletGreenStone : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float destroyDelay;
        [SerializeField] private Rigidbody rb;
        
        private PoolManager.PoolObjectType explosionPlayerLv;

        private void OnEnable()
        {
            StartCoroutine(nameof(LifeTime));
        }

        private void OnDisable()
        {
            StopCoroutine(nameof(LifeTime));
        }

        public void Init(Vector3 direction, PoolManager.PoolObjectType explosionPlayerLv)
        {
            this.explosionPlayerLv = explosionPlayerLv switch
            {
                PoolManager.PoolObjectType.BulletGunGreenStoneLv1 => PoolManager.PoolObjectType.ExplosionBulletGunGreenStoneLv1,
                PoolManager.PoolObjectType.BulletGunGreenStoneLv2 => PoolManager.PoolObjectType.ExplosionBulletGunGreenStoneLv2,
                _ => this.explosionPlayerLv
            };
            
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 1);
            
            rb.velocity = direction * speed;
        }

        private void OnTriggerEnter(Collider other)
        {
            PoolExplosion();

            gameObject.SetActive(false);
        }

        private IEnumerator LifeTime()
        {
            yield return new WaitForSeconds(destroyDelay);
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