using Kamaria.Item;
using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Enemy.AIEnemy.SkeletonBoss
{
    public sealed class BulletEnemy : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private float damagePercent;
        [SerializeField] private float showTime;
        [SerializeField] private Rigidbody rb;
        [SerializeField] private float speed;
        
        private int damage;
        private EffectAttack effectAttack;
        
        public void Init(int damage, EffectAttack effectAttack, Vector3 explosionPos, float powerKnockback,
            float radiusKnockback, bool isAOE, float stunTime, Vector3 direction)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, 1);
            
            this.damage = (int)(damage * damagePercent) / 100;
            this.effectAttack = effectAttack;
            rb.velocity = direction * speed;
            Invoke(nameof(Close), showTime);
        }

        private void Close()
        {
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IDamageable target)) return;
            if (playerData.CharacterControllerData.Dashing || playerData.CharacterSkillData.UsingGuard) return;
            
            target.TakeDamage(damage, effectAttack, this.transform.position, 0, 0,
                false, 0, WeaponTypes.None, KeyStones.None);
        }

        /*private void OnParticleTrigger()
        {
            ParticleSystem ps = GetComponent<ParticleSystem>();

            // particles
            List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();

            // get
            int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);

            // iterate
            for (int i = 0; i < numEnter; i++)
            {
                player.TakeDamage(damage, effectAttack, this.transform.position, 0, 0,
                    false, 0, WeaponTypes.None, KeyStones.None);
                ParticleSystem.Particle p = enter[i];
                enter[i] = p;

            }

            // set
            ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
        }*/
    }
}