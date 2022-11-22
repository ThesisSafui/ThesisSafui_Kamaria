using System;
using System.Collections;
using UnityEngine;

namespace Kamaria.VFX_ALL
{
    public sealed class VfxPortal : MonoBehaviour
    {
        [SerializeField] private GameObject vfxPortal2;
        [SerializeField] private float timeShow;
        [SerializeField] private float lifeTime;

        private void OnEnable()
        {
            StartCoroutine(nameof(LoopOpen));
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator LoopOpen()
        {
            while (true)
            {
                vfxPortal2.SetActive(false);
                yield return new WaitForSeconds(timeShow);
                vfxPortal2.SetActive(true);
                yield return new WaitForSeconds(lifeTime);
            }
        }
    }
}