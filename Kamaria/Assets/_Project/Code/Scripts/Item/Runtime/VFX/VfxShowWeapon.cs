using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

namespace Kamaria.VFX_ALL
{
    public enum VfxWeapon
    {
        SwordLv1, SwordLv2, SwordLv3,
        GunLv1, GunLv2, GunLv3,
        KnuckleLv1, KnuckleLv2, KnuckleLv3
    }
    
    public sealed class VfxShowWeapon : MonoBehaviour
    {
        [SerializeField] private VfxWeapon vfxWeapon;
        [SerializeField] private VisualEffect visualEffect;
        [SerializeField] private float stopTime;

        public VfxWeapon VfxWeapon => vfxWeapon;
        
        private void OnEnable()
        {
            Time.timeScale = 1;
            StartCoroutine(nameof(StopPlay));
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            Time.timeScale = 0;
            visualEffect.pause = false;
        }

        private IEnumerator StopPlay()
        {
            yield return new WaitForSecondsRealtime(stopTime);
            Time.timeScale = 0;
            visualEffect.pause = true;
        }
    }
}