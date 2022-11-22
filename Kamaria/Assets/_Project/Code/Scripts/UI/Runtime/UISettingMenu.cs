using Kamaria.Utilities;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI
{
    public sealed class UISettingMenu : MonoBehaviour
    {
        #region REFERENCE_VARIABLE
        
        [SerializeField] private Button backToMenu;
        [SerializeField] private VolumeSetting bgmVolume;
        [SerializeField] private VolumeSetting sfxVolume;
        [SerializeField] private VolumeSetting uiVolume;

        #endregion

        private SoundHandler soundHandler;
        
        private void OnEnable()
        {
            soundHandler = SoundHandler.Instance;
            
            Initialized();
            
            backToMenu.onClick.AddListener(BackToMenu);

            bgmVolume.Volume.onValueChanged.AddListener(BGMChangeVolume);
            bgmVolume.Mute.onValueChanged.AddListener(BGMMute);
            
            sfxVolume.Volume.onValueChanged.AddListener(SFXChangeVolume);
            sfxVolume.Mute.onValueChanged.AddListener(SFXMute);
            
            uiVolume.Volume.onValueChanged.AddListener(UIChangeVolume);
            uiVolume.Mute.onValueChanged.AddListener(UIMute);
        }

        private void OnDisable()
        {
            backToMenu.onClick.RemoveListener(BackToMenu);
            
            bgmVolume.Volume.onValueChanged.RemoveListener(BGMChangeVolume);
            bgmVolume.Mute.onValueChanged.RemoveListener(BGMMute);
            
            sfxVolume.Volume.onValueChanged.RemoveListener(SFXChangeVolume);
            sfxVolume.Mute.onValueChanged.RemoveListener(SFXMute);
            
            uiVolume.Volume.onValueChanged.RemoveListener(UIChangeVolume);
            uiVolume.Mute.onValueChanged.RemoveListener(UIMute);
        }

        private void Initialized()
        {
            bgmVolume.Mute.isOn = soundHandler.InitializedValueMute(SoundTypes.BGM);
            sfxVolume.Mute.isOn = soundHandler.InitializedValueMute(SoundTypes.SFX);
            uiVolume.Mute.isOn = soundHandler.InitializedValueMute(SoundTypes.UI);
            
            bgmVolume.Volume.value = soundHandler.InitializedValueVolume(SoundTypes.BGM);
            sfxVolume.Volume.value = soundHandler.InitializedValueVolume(SoundTypes.SFX);
            uiVolume.Volume.value = soundHandler.InitializedValueVolume(SoundTypes.UI);
        }

        private void BackToMenu()
        {
            this.gameObject.SetActive(false);
        }

        #region BGM

        private void BGMChangeVolume(float volume)
        {
            soundHandler.ChangeVolume(SoundTypes.BGM, volume);
        }
        
        private void BGMMute(bool callback)
        {
            soundHandler.Mute(SoundTypes.BGM, !callback);
        }

        #endregion
        
        #region SFX

        private void SFXChangeVolume(float volume)
        {
            soundHandler.ChangeVolume(SoundTypes.SFX, volume);
        }
        
        private void SFXMute(bool callback)
        {
            soundHandler.Mute(SoundTypes.SFX, !callback);
        }

        #endregion
        
        #region UI

        private void UIChangeVolume(float volume)
        {
            soundHandler.ChangeVolume(SoundTypes.UI, volume);
        }
        
        private void UIMute(bool callback)
        {
            soundHandler.Mute(SoundTypes.UI, !callback);
        }

        #endregion
    }
}