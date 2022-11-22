using System;
using System.Collections;
using DG.Tweening;
using Kamaria.Item;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UIDamage
{
    public sealed class HealthBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private Slider sliderGuard;
        [SerializeField] private IconWeakWeapon iconWeakWeapon;
        [SerializeField] private float showTime;

        private int maxHealth;
        private int maxGuard;
        private float sliderHpValue;
        private float sliderGuardValue;

        public void SetIconWeakWeapon(WeaponTypes weaponTypes)
        {
            iconWeakWeapon.Init(weaponTypes);
        }
        
        public void Init(int maxHealth, int maxGuard, bool useGuard)
        {
            this.maxHealth = maxHealth;
            slider.value = slider.maxValue;
            
            if (useGuard)
            {
                this.maxGuard = maxGuard;
                sliderGuard.value = sliderGuard.maxValue;
            }
            else
            {
                sliderGuard.gameObject.SetActive(false);
                iconWeakWeapon.gameObject.SetActive(false);
            }
        }
        
        public void ShowBar(int currentHealth)
        {
            sliderHpValue = Mathf.Clamp((float)currentHealth / maxHealth, slider.minValue, slider.maxValue);
            this.gameObject.SetActive(true);
            //StopCoroutine(nameof(ShowTime));
            //StartCoroutine(nameof(ShowTime));
            slider.DOValue(sliderHpValue, showTime);

        }
        
        public void ShowBar(int currentHealth, int currentGuard)
        {
            sliderHpValue = Mathf.Clamp((float)currentHealth / maxHealth, slider.minValue, slider.maxValue);
            sliderGuardValue = Mathf.Clamp((float)currentGuard / maxGuard, sliderGuard.minValue, sliderGuard.maxValue);
            this.gameObject.SetActive(true);
            //StopCoroutine(nameof(ShowTime));
            //StartCoroutine(nameof(ShowTime));
            slider.DOValue(sliderHpValue, showTime);
            sliderGuard.DOValue(sliderGuardValue, showTime);
        }

        private IEnumerator ShowTime()
        {
            yield return new WaitForSeconds(showTime + 0.5f);
            //yield return new WaitUntil((() => Math.Abs(slider.value - sliderHpValue) < 0.001));
            this.gameObject.SetActive(false);
        }
        
        private IEnumerator ShowTimeHaveGuard()
        {
            yield return new WaitUntil((() => Math.Abs(slider.value - sliderHpValue) < 0.001));
            yield return new WaitUntil((() => Math.Abs(sliderGuard.value - sliderGuardValue) < 0.001));
            this.gameObject.SetActive(false);
        }
    }
}