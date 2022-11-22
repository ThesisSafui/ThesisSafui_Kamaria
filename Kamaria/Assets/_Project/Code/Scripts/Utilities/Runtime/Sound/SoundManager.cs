using System;
using UnityEngine;

namespace Kamaria.Utilities
{
    public sealed class SoundManager : MonoBehaviour
    {
        private SoundHandler soundHandler;

        private void Awake()
        {
            soundHandler = SoundHandler.Instance;
        }
        
        public void PlaySoundClick()
        {
            soundHandler.PlayUI(SoundClip.Sound.Click);
        }
    }
}