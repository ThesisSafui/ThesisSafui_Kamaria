using System;
using UnityEngine;

namespace Project.Scripts
{
    public sealed class ShieldRipplesVFX : MonoBehaviour
    {
        [SerializeField] private float showTime;

        public void Init(Vector3 scale)
        {
            transform.localScale = scale;
        }
        
        private void OnEnable()
        {
            Invoke(nameof(Close), showTime);
        }

        private void Close()
        {
            gameObject.SetActive(false);
        }
    }
}