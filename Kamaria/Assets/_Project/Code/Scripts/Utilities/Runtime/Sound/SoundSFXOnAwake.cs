using System;
using System.Collections;
using UnityEngine;

namespace Kamaria.Utilities
{
    public sealed class SoundSFXOnAwake : MonoBehaviour
    {
        [SerializeField] private SoundClip.Sound sound;
        
        private void OnEnable()
        {
            if (sound == SoundClip.Sound.SkeletonMeteor)
            {
                StartCoroutine(WaitMeteor());
            }
            else
            {
                SoundHandler.Instance.PlaySFX3D(sound, this.transform.position);
            }
        }

        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator WaitMeteor()
        {
            var soundClip = SoundHandler.Instance.GetAudioSourceSFX(sound);
            var clip = soundClip.audioClip;
            yield return new WaitForSeconds(3);
            while (true)
            {
                AudioSource.PlayClipAtPoint(clip, this.gameObject.transform.position, soundClip.soundVolume + 0.5f);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}