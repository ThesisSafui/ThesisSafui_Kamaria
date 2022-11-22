using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kamaria.Utilities.SingletonPattern;
using UnityEngine;
using UnityEngine.Audio;

namespace Kamaria.Utilities
{
    public class SoundHandler : SingletonPersistent<SoundHandler>
    {
        [SerializeField] private AudioMixerGroup mixerGroupBGM;
        [SerializeField] private AudioMixerGroup mixerGroupSFX;
        [SerializeField] private AudioMixerGroup mixerGroupUI;
        [SerializeField] private List<SoundData> soundsData = new List<SoundData>();
        [SerializeField] private GameObject parentBGM;
        [SerializeField] private GameObject parentSFX;
        [SerializeField] private GameObject parentUI;

        private List<SoundClip> tempPause = new List<SoundClip>();

        public override void Awake()
        {
            base.Awake(); 
            InitializedSound();
            PlayBGM(SoundClip.Sound.BGMLobby);
        }

        private void InitializedSound()
        {
            var soundBGM = soundsData.Find(x => x.SoundTypes == SoundTypes.BGM);
            var soundUI = soundsData.Find(x => x.SoundTypes == SoundTypes.UI);
            var soundSFX = soundsData.Find(x => x.SoundTypes == SoundTypes.SFX);

            SetDataSound(soundBGM.SoundClips, mixerGroupBGM, parentBGM);
            SetDataSound(soundUI.SoundClips, mixerGroupUI, parentUI);
            SetDataSound(soundSFX.SoundClips, mixerGroupSFX, parentSFX);
        }

        private void SetDataSound(List<SoundClip> soundBGMPlayer, AudioMixerGroup audioMixerGroup, GameObject parent)
        {
            foreach (var soundClip in soundBGMPlayer)
            {
                var soundSource = parent.AddComponent<AudioSource>();
                soundSource.clip = soundClip.audioClip;
                soundSource.loop = soundClip.loop;
                soundSource.volume = soundClip.soundVolume;
                soundSource.playOnAwake = false;
                soundSource.mute = soundClip.mute;
                soundSource.outputAudioMixerGroup = audioMixerGroup;
                
                if (soundClip.use3D)
                {
                    soundSource.spatialBlend = soundClip.spatialBlend;
                    soundSource.minDistance = soundClip.min3D;
                    soundSource.maxDistance = soundClip.max3D;
                    soundSource.rolloffMode = soundClip.rolloffMode;
                }
                
                soundClip.audioSource = soundSource;
            }
        }

        public void PlayBGM(SoundClip.Sound sound)
        {
            var soundTypes = soundsData.Find(x => x.SoundTypes == SoundTypes.BGM);
            var soundPlay = soundTypes.SoundClips.Find(x => x.sound == sound);
            
            if (soundPlay.audioSource.isPlaying) return;
            
            Play(soundPlay);
        }
        
        public void PlaySFX(SoundClip.Sound sound)
        {
            var soundTypes = soundsData.Find(x => x.SoundTypes == SoundTypes.SFX);
            var soundPlay = soundTypes.SoundClips.Find(x => x.sound == sound);
            Play(soundPlay);
        }

        public SoundClip GetAudioSourceSFX(SoundClip.Sound sound)
        {
            var soundTypes = soundsData.Find(x => x.SoundTypes == SoundTypes.SFX);
            return soundTypes.SoundClips.Find(x => x.sound == sound);
        }
        
        public void PlaySFX3D(SoundClip.Sound sound, Vector3 pos)
        {
            var soundTypes = soundsData.Find(x => x.SoundTypes == SoundTypes.SFX);
            var soundPlay = soundTypes.SoundClips.Find(x => x.sound == sound);
            AudioSource.PlayClipAtPoint(soundPlay.audioSource.clip, pos, soundPlay.audioSource.volume + 0.5f);
        }

        public void PlaySFXPlayer(SoundClip.Sound sound)
        {
            var soundTypes = soundsData.Find(x => x.SoundTypes == SoundTypes.SFX);
            var soundPlay = soundTypes.SoundClips.Find(x => x.sound == sound);
            var soundPlayer = soundTypes.SoundClips.FindAll(x => x.soundCharacter == SoundCharacter.Player);
            
            foreach (var soundClip in soundPlayer)
            {
                if (soundClip.sound == sound || soundClip.untilTheEnd) continue;
                soundClip.audioSource.Stop();
            }
            
            Play(soundPlay);
        }

        private void Play(SoundClip soundPlay)
        {
            if (!soundPlay.audioSource.isPlaying)
            {
                soundPlay.audioSource.volume = soundPlay.soundVolume;
            }
            soundPlay.audioSource.mute = soundPlay.mute;
            soundPlay.audioSource.Play();
        }

        public void AllSoundPause()
        {
            var soundSFX = soundsData.Find(x => x.SoundTypes == SoundTypes.SFX);
            tempPause.Clear();
            foreach (var soundClip in soundSFX.SoundClips)
            {
                if (soundClip.audioSource.isPlaying)
                {
                    tempPause.Add(soundClip);
                }
                soundClip.audioSource.Pause();
            }
        }

        public void AllSoundUnPause()
        {
            foreach (var soundClip in tempPause)
            {
                Play(soundClip);
            }

            tempPause.Clear();
        }

        public void PlayUI(SoundClip.Sound sound)
        {
            var soundTypes = soundsData.Find(x => x.SoundTypes == SoundTypes.UI);
            var soundPlay = soundTypes.SoundClips.Find(x => x.sound == sound);
            Play(soundPlay);
        }

        public void StopBGM(SoundClip.Sound sound)
        {
            var soundTypes = soundsData.Find(x => x.SoundTypes == SoundTypes.BGM);
            var soundPlay = soundTypes.SoundClips.Find(x => x.sound == sound);
            soundPlay.audioSource.Stop();
        }

        public void StopSFX(SoundClip.Sound sound)
        {
            var soundTypes = soundsData.Find(x => x.SoundTypes == SoundTypes.SFX);
            var soundPlay = soundTypes.SoundClips.Find(x => x.sound == sound);
            soundPlay.audioSource.Stop();
        }
        
        public void PlaySFXWalk()
        {
            var soundTypes = soundsData.Find(x => x.SoundTypes == SoundTypes.SFX);
            var soundPlay = soundTypes.SoundClips.Find(x => x.sound == SoundClip.Sound.PlayerRun);

            if (soundPlay.audioSource.isPlaying) return;
            
            Play(soundPlay);
        }

        public void PlaySFXDead()
        {
            var soundTypes = soundsData.Find(x => x.SoundTypes == SoundTypes.SFX);
            var soundPlay = soundTypes.SoundClips.Find(x => x.sound == SoundClip.Sound.PlayerDead);

            if (soundPlay.audioSource.isPlaying) return;
            var soundPlayer = soundTypes.SoundClips.FindAll(x => x.soundCharacter == SoundCharacter.Player);
            
            foreach (var soundClip in soundPlayer)
            {
                if (soundClip.sound == SoundClip.Sound.PlayerDead || soundClip.untilTheEnd) continue;
                soundClip.audioSource.Stop();
            }
            
            Play(soundPlay);
        }

        public void StopUI(SoundClip.Sound sound)
        {
            var soundTypes = soundsData.Find(x => x.SoundTypes == SoundTypes.UI);
            var soundPlay = soundTypes.SoundClips.Find(x => x.sound == sound);
            soundPlay.audioSource.Stop();
        }
        
        public bool InitializedValueMute(SoundTypes soundType)
        {
            var soundTypesData = soundsData.Find(x => x.SoundTypes == soundType);
            return soundTypesData.SoundClips.Count == 0 || !soundTypesData.SoundClips[Index.Start].audioSource.mute;
        }
        
        public float InitializedValueVolume(SoundTypes soundType)
        {
            var soundTypesData = soundsData.Find(x => x.SoundTypes == soundType);
            return soundTypesData.SoundClips.Count == 0 ? 0 : soundTypesData.SoundClips[Index.Start].audioSource.volume;
        }
        
        public void ChangeVolume(SoundTypes soundTypes, float volume)
        {
            var soundTypesData = soundsData.Find(x => x.SoundTypes == soundTypes);
            
            foreach (var soundClip in soundTypesData.SoundClips)
            {
                soundClip.soundVolume = volume;

                if (soundClip.audioSource.isPlaying)
                {
                    soundClip.audioSource.volume = soundClip.soundVolume;
                }
            }
        }
        
        public void Mute(SoundTypes soundTypes, bool isMute)
        {
            var soundTypesData = soundsData.Find(x => x.SoundTypes == soundTypes);
            
            foreach (var soundClip in soundTypesData.SoundClips)
            {
                soundClip.mute = isMute;

                soundClip.audioSource.mute = soundClip.mute;
            }
        }
    }
}