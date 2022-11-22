using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Kamaria.Utilities
{
    public sealed class HideShadow : MonoBehaviour
    {
        [SerializeField] private List<Renderer> renderers = new List<Renderer>();
        
        private void Awake()
        {
            if (renderers.Count == 0)
            {
                renderers.AddRange(GetComponentsInChildren<Renderer>());
            }
        }

        public void Show()
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].shadowCastingMode = ShadowCastingMode.On;
            }
        }
        
        public void Hide()
        {
            for (int i = 0; i < renderers.Count; i++)
            {
                renderers[i].shadowCastingMode = ShadowCastingMode.Off;
            }
        }
    }
}