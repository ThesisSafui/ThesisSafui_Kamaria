using System.Collections;
using System.Collections.Generic;
using Kamaria.Item;
using Kamaria.Player.Data;
using Kamaria.UIDamage;
using Kamaria.Utilities;
using UnityEngine;
using Random = System.Random;

namespace Kamaria.ObjInteract
{
    public sealed class Gate : MonoBehaviour, IDamageable
    {
        [SerializeField] private IconWeakWeapon[] iconWeakWeapons;
        [SerializeField] private GameObject mainObj;
        [SerializeField] private BlinkEffect blinkEffect;
        [SerializeField] private float fadeSpeed;
        [SerializeField] private float resetTime = 10;
        
        private List<Renderer> renderers = new List<Renderer>();
        private List<float> tempAlpha = new List<float>();
        private bool isDead;
        private Random random = new Random();
        private int currentIndextypesDamage;
        private bool cangetDamage = true;

        private readonly int opacity = Shader.PropertyToID("_Opacity");
        private const string RT_SHADER = "Universal Render Pipeline/RealToon/Version 5/Default/Default";

        private void Awake()
        {
            if (renderers.Count == 0)
            {
                renderers.AddRange( mainObj.GetComponentsInChildren<Renderer>());
            }

            for (int i = 0; i < renderers.Count; i++)
            {
                tempAlpha.Add(renderers[i].material.shader.name == RT_SHADER
                    ? renderers[i].material.GetFloat(opacity)
                    : renderers[i].material.color.a);
            }
        }

        private void OnEnable()
        {
            Initialized();
        }
        
        private void Update()
        {
            if (!isDead) return;
            
            FadeObj();
        }

        private void Initialized()
        {
            cangetDamage = true;
            isDead = false;
            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i].material.shader.name == RT_SHADER)
                {
                    renderers[i].material.SetFloat(opacity, tempAlpha[i]);
                }
                else
                {
                    renderers[i].material.color = new Color(
                        renderers[i].material.color.r,
                        renderers[i].material.color.g,
                        renderers[i].material.color.b,
                        tempAlpha[i]
                    );
                }
            }

            SetGetDamage();
        }

        private void SetGetDamage()
        {
            currentIndextypesDamage = 0;
            for (int i = 0; i < iconWeakWeapons.Length; i++)
            {
                iconWeakWeapons[i].Init(RandomTypeGetDamage(out WeaponTypes weaponTypes));
                iconWeakWeapons[i].gameObject.SetActive(true);
            }
        }

        private WeaponTypes RandomTypeGetDamage(out WeaponTypes weakWeapon)
        {
            WeaponTypes weakForWeapon = WeaponTypes.None;
            
            weakForWeapon = random.Next(3) switch
            {
                0 => WeaponTypes.Gun,
                1 => WeaponTypes.Sword,
                2 => WeaponTypes.Knuckle,
                _ => weakForWeapon
            };

            weakWeapon = weakForWeapon;
            return weakWeapon;
        }

        private void FadeObj()
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                if (renderers[i].material.shader.name == RT_SHADER)
                {
                    renderers[i].material.SetFloat(opacity, Mathf.Lerp(renderers[i].material.GetFloat(opacity),
                        0, fadeSpeed * Time.deltaTime));
                }
                else
                {
                    renderers[i].material.color = new Color(
                        renderers[i].material.color.r,
                        renderers[i].material.color.g,
                        renderers[i].material.color.b,
                        Mathf.Lerp(renderers[i].material.color.a, 0, fadeSpeed * Time.deltaTime)
                    );
                }
            }
        }

        private void Dead()
        {
            isDead = true;
            StopCoroutine(nameof(ResetGetDamage));
            StartCoroutine(FadeTime());
        }
        
        public void TakeDamage(int damage, EffectAttack effectAttack, Vector3 explosionPos, float powerKnockback,
            float radiusKnockback, bool isAOE, float stunTime, WeaponTypes weaponTypes, KeyStones keyStones)
        {
            if (isDead) return;
            if (!cangetDamage) return;
            StartCoroutine(nameof(WaitNextDamage));
            blinkEffect.Blink();
            
            Debug.Log($"Gate index{currentIndextypesDamage}");
            Debug.Log($"Gate attack ={weaponTypes}");
            Debug.Log($"Gate get {iconWeakWeapons[currentIndextypesDamage].WeaponTypes}");
            if (weaponTypes == iconWeakWeapons[currentIndextypesDamage].WeaponTypes)
            {
                iconWeakWeapons[currentIndextypesDamage].gameObject.SetActive(false);
                currentIndextypesDamage++;
            }
            else
            {
                SetGetDamage();
            }

            if (currentIndextypesDamage == iconWeakWeapons.Length)
            {
                Dead();
            }

            if (currentIndextypesDamage <= 0) return;
            StopCoroutine(nameof(ResetGetDamage));
            StartCoroutine(nameof(ResetGetDamage));
        }

        private IEnumerator WaitNextDamage()
        {
            cangetDamage = false;
            yield return new WaitForSeconds(0.25f);
            cangetDamage = true;
        }

        private IEnumerator ResetGetDamage()
        {
            yield return new WaitForSeconds(resetTime);
            SetGetDamage();
        }
        
        private IEnumerator FadeTime()
        {
            yield return new WaitForSeconds(fadeSpeed + 0.5f);
            mainObj.SetActive(false);
        }
    }
}