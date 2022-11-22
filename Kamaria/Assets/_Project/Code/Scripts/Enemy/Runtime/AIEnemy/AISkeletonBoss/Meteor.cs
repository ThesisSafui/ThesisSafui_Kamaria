using System;
using System.Collections;
using Kamaria.Player.Data;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace Kamaria.Enemy.AIEnemy.SkeletonBoss
{
    public sealed class Meteor : MonoBehaviour
    {
        [SerializeField] private MeteorDamageCollider damageCollider;
        [SerializeField] private VisualEffect vfxMeteor;
        [SerializeField] private float maxRate;
        [SerializeField] private float minRate;
        [SerializeField] private GameObject indicator;
        [SerializeField] private float showIndicatorTime;
        [SerializeField] private float meteorTime;
        [SerializeField] private float damageDealTimePerSecond;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float speed;

        private bool isEnd;
        private ExposedProperty propertyRate = "Rate";

        public void Init(Vector3 target, int damage, EffectAttack effectAttack, Vector3 explosionPos, float powerKnockback,
            float radiusKnockback, bool isAOE, float stunTime)
        {
            damageCollider.InitDamage(
                damage, effectAttack, explosionPos, powerKnockback, radiusKnockback, isAOE, stunTime);
            damageCollider.gameObject.SetActive(false);
            vfxMeteor.Stop();
            isEnd = false;
            this.transform.position = target;
            this.transform.position += offset;
            StartCoroutine(nameof(MeteorTime));
        }

        private IEnumerator MeteorTime()
        {
            indicator.SetActive(true);
            yield return new WaitForSeconds(showIndicatorTime);
            vfxMeteor.Play();
            //indicator.SetActive(false);
            
            StartCoroutine(nameof(DamageDealTimePerSecond));
            
            while (Math.Abs(vfxMeteor.GetFloat(propertyRate) - maxRate) > 0.1f)
            {
                vfxMeteor.SetFloat(propertyRate, Mathf.Lerp(vfxMeteor.GetFloat(propertyRate), maxRate, Time.deltaTime * speed));
                yield return null;
            }

            yield return new WaitForSeconds(meteorTime);
            indicator.SetActive(false);
            isEnd = true;
        }

        private IEnumerator DamageDealTimePerSecond()
        {
            while (!isEnd)
            {
                yield return new WaitForSeconds(damageDealTimePerSecond);
                damageCollider.gameObject.SetActive(true);
            }
            
            while (Math.Abs(vfxMeteor.GetFloat(propertyRate) - minRate) > 0.1f)
            {
                vfxMeteor.SetFloat(propertyRate, Mathf.Lerp(vfxMeteor.GetFloat(propertyRate), minRate, Time.deltaTime * speed));
                yield return null;
            }

            gameObject.SetActive(false);
        }
    }
}