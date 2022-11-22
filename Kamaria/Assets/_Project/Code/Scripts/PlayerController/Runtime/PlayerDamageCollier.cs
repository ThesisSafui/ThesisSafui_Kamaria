using System;
using System.Collections;
using Kamaria.Enemy.AIEnemy.Mouse;
using Kamaria.Item;
using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Player.Controller
{
    public enum CollidersDamage
    {
        BladeColliderCombo1, BladeColliderCombo2, BladeColliderCombo3,
        BladeColliderCombo4LV1, BladeColliderCombo4LV2,
        BladeColliderSpinLV1, BladeColliderSpinLV2,
        BladeColliderRedStoneLV1, BladeColliderRedStoneLV2,
        BladeColliderCombo1GreenStoneLV1, BladeColliderCombo2GreenStoneLV1, BladeColliderCombo3GreenStoneLV1,
        BladeColliderCombo4LV1GreenStoneLV1, BladeColliderCombo4LV2GreenStoneLV1,
        BladeColliderSpinLV1GreenStoneLV1, BladeColliderSpinLV2GreenStoneLV1, 
        BladeColliderCombo1GreenStoneLV2, BladeColliderCombo2GreenStoneLV2, BladeColliderCombo3GreenStoneLV2,
        BladeColliderCombo4LV1GreenStoneLV2, BladeColliderCombo4LV2GreenStoneLV2,
        BladeColliderSpinLV1GreenStoneLV2, BladeColliderSpinLV2GreenStoneLV2, 
        ArrowCollider,
        KnuckleComboLeft, KnuckleComboRight, KnuckleComboHeavyPunchNormal, KnuckleComboHeavyPunchStun,
        KnuckleGuardCounter, KnuckleAoeLV1, KnuckleAoeLV2, 
        KnuckleSkillRedStoneLV1, KnuckleSkillRedStoneLV2, KnuckleSkillGreenStoneLV1, KnuckleSkillGreenStoneLV2,
        CannonCollider,
        BulletGun, Grenade, BeamLv1, BeamLv2, BulletGunGreenStone, Spear
    }
    
    public sealed class PlayerDamageCollier : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO player;
        [SerializeField] private PlayerDamage playerDamage;
        [SerializeField] private bool isOneHit;
        [SerializeField] private bool isAOE;
        [SerializeField] private float powerKnockback;
        [SerializeField] private float radiusKnockback;
        [SerializeField] private float stunTime;
        [SerializeField] private bool isRange;
        [SerializeField] private bool canCharge;
        [Header("MiniAoe")]
        [SerializeField] private bool isMiniAoeKnuckle;
        [SerializeField] private float radiusDamageKnuckle;
        [SerializeField] private LayerMask layerDamage;
        [Header("Gun Bullet")]
        [SerializeField] private bool isGunBullet;
        [SerializeField] private int countTarget;
        [SerializeField] private float timeDamage;
        [SerializeField] private bool isLaser;
        [SerializeField] private float laserLifeTime;
        [SerializeField] private float longLaser;

        private int currentHitTarget;
        private delegate void Hit();
        private AIMouse aiMouse;
        
        public PlayerDamage PlayerDamage => playerDamage;
        [HideInInspector] public int damage;
        [HideInInspector] public EffectAttack effectAttack;
        public bool CanCharge => canCharge;
        [HideInInspector] public WeaponTypes weaponTypes;
        [HideInInspector] public KeyStones keyStones;

        private void OnEnable()
        {
            currentHitTarget = 0;
            if (isRange)
            {
                player.SetDataAttack(true, this, out damage, out effectAttack, out weaponTypes, out keyStones);
            }
            
            if (isMiniAoeKnuckle)
            {
                Collider[] hitColliders = new Collider[15];
                int numColliders =
                    Physics.OverlapSphereNonAlloc(this.transform.position, radiusDamageKnuckle, hitColliders, layerDamage);

                for (int i = 0; i < numColliders; i++)
                {
                    if (!hitColliders[i].TryGetComponent(out IDamageable target)) continue;
                    
                    target.TakeDamage(damage, effectAttack, this.transform.position, powerKnockback, radiusKnockback,
                        isAOE, stunTime, weaponTypes, keyStones);
                }
            }

            if (isLaser)
            {
                StartCoroutine(nameof(DamageLaser));
                Invoke(nameof(Close), laserLifeTime);
            }
        }

        private void OnDisable()
        {
            StopCoroutine(nameof(DamageLaser));
        }

        private IEnumerator DamageLaser()
        {
            while (true)
            { 
                RaycastHit[] laserHit = new RaycastHit[15];

                int countHit = Physics.RaycastNonAlloc(transform.position, transform.forward, laserHit, longLaser,
                    layerDamage);
                for (int i = 0; i < countHit; i++)
                {

                    if (!laserHit[i].collider.TryGetComponent(out IDamageable target)) continue;

                    target.TakeDamage(damage, effectAttack, this.transform.position, powerKnockback, radiusKnockback,
                        isAOE, stunTime, weaponTypes, keyStones);
                }
                yield return new WaitForSeconds(timeDamage);
            }
        }

        private void Close()
        {
            gameObject.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isMiniAoeKnuckle) return;
            if (isLaser) return;
            
            if (!other.TryGetComponent(out IDamageable target)) return;

            if (isGunBullet)
            {
                currentHitTarget++;
                target.TakeDamage(damage, effectAttack, this.transform.position, powerKnockback, radiusKnockback,
                    isAOE, stunTime, weaponTypes, keyStones);
                
                if (currentHitTarget == countTarget)
                {
                    gameObject.SetActive(false);
                }
                
                return;
            }
            
            target.TakeDamage(damage, effectAttack, this.transform.position, powerKnockback, radiusKnockback,
                isAOE, stunTime, weaponTypes, keyStones);

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

            if (isMiniAoeKnuckle)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(this.transform.position, radiusDamageKnuckle);
            }

            if (isLaser)
            {
                Gizmos.color = Color.magenta;
                Vector3 direction = transform.TransformDirection(Vector3.forward) * longLaser;
                Gizmos.DrawRay(this.transform.position, direction);
            }
        }
#endif
    }
}