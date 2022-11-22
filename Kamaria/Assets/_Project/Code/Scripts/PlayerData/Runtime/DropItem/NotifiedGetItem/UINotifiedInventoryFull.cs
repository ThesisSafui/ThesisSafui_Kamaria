using System;
using System.Collections;
using UnityEngine;

namespace Kamaria.UI.NotifiedGetItem
{
    public sealed class UINotifiedInventoryFull : MonoBehaviour
    {
        [SerializeField] private float animTime;
        
        public void Init()
        {
        }

        private void OnEnable()
        {
            StartCoroutine(Close());
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator Close()
        {
            yield return new WaitForSecondsRealtime(animTime);
            gameObject.SetActive(false);
        }
    }
}