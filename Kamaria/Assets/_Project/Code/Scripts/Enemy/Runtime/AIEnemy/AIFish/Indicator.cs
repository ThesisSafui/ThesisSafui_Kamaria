using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.VFX.Utility;

namespace Kamaria.Enemy.AIEnemy.Fish
{
    public sealed class Indicator : MonoBehaviour
    {
        [SerializeField] private VisualEffect vfxIndicator;
        [SerializeField] private float showTime;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float sizeSmoothTime;
        
        private ExposedProperty propertySize = "Size";
        private float size;

        public void SetSize(float size)
        {
            this.size = size;
            StopCoroutine(nameof(SetSizeTime));
            StartCoroutine(nameof(SetSizeTime));
        }

        private IEnumerator SetSizeTime()
        {
            while (Math.Abs(vfxIndicator.GetFloat(propertySize) - size) > 0.1f)
            {
                vfxIndicator.SetFloat(propertySize,
                    Mathf.Lerp(vfxIndicator.GetFloat(propertySize), size, Time.deltaTime * sizeSmoothTime));
                yield return null;
            }
        }

        public void Init(Vector3 target)
        {
            this.transform.position = target;
            this.transform.position += offset;

            Invoke(nameof(Close), showTime);
        }
        
        private void Close()
        {
            this.gameObject.SetActive(false);
        }
    }
}