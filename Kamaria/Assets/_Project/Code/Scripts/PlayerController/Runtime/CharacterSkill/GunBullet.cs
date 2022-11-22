using System;
using System.Collections;
using Kamaria.Item;
using UnityEngine;

namespace Kamaria.Player.Controller
{
    public sealed class GunBullet : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float destroyDelay;
        [SerializeField] private float delayAim;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private GameObject vfxBulletNormal;
        [SerializeField] private GameObject vfxBulletRed;
        [SerializeField] private GameObject vfxBulletGreen;
        [SerializeField] private GameObject vfxBulletBlue;

        private float delay;
        
        private void OnEnable()
        {
            StartCoroutine(nameof(LifeTime));
        }

        private void OnDisable()
        {
            StopCoroutine(nameof(LifeTime));
        }

        public void Init(Vector3 direction, KeyStones keyStones, bool isAim)
        {
            delay = isAim ? destroyDelay + delayAim : destroyDelay;

            vfxBulletGreen.SetActive(false);
            vfxBulletRed.SetActive(false);
            vfxBulletNormal.SetActive(false);
            vfxBulletBlue.SetActive(false);

            switch (keyStones)
            {
                case KeyStones.WindStone:
                    vfxBulletGreen.SetActive(true);
                    break;
                case KeyStones.FireStone:
                    vfxBulletRed.SetActive(true);
                    break;
                case KeyStones.PowerStone:
                    vfxBulletBlue.SetActive(true);
                    break;
                default:
                    vfxBulletNormal.SetActive(true);
                    break;
            }

            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 1);
            
            rb.velocity = direction * speed;
        }
        
        private IEnumerator LifeTime()
        {
            yield return new WaitForSeconds(delay);
            gameObject.SetActive(false);
        }
    }
}