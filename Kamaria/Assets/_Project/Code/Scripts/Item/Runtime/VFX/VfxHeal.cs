using System;
using UnityEngine;

namespace Kamaria.VFX_ALL
{
    public sealed class VfxHeal : MonoBehaviour
    {
        [SerializeField] private float lifeTime;
        
        private void OnEnable()
        {
            Invoke(nameof(Close), lifeTime);
        }

        private void Close()
        {
            this.gameObject.SetActive(false);
        }
    }
}