using System;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI
{
    [Serializable]
    public sealed class VolumeSetting
    {
        [SerializeField] private Slider volume;
        [SerializeField] private Toggle mute;
        
        public Slider Volume => volume;
        public Toggle Mute => mute;
    }
}