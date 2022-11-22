using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Kamaria.UI
{
    public sealed class UIGameMenu : MonoBehaviour
    {
        #region REFERENCE_VARIABLE

        [SerializeField] private Button play;
        [SerializeField] private Button setting;
        [SerializeField] private Button quit;
        [Space] 
        [SerializeField] private RectTransform loadingScenePanel;
        [SerializeField] private RectTransform settingPanel;
        [Space] 
        [SerializeField] private Button skip;
        [SerializeField] private VideoPlayer videoPlayer;
        [SerializeField] private RectTransform videoPanel;

        #endregion

        private bool isSkip = false;
        
        private void OnEnable()
        {
            videoPanel.gameObject.SetActive(false);
            videoPlayer.gameObject.SetActive(false);
            isSkip = false;
            play.onClick.AddListener(PlayGame);
            setting.onClick.AddListener(SettingGame);
            quit.onClick.AddListener(QuitGame);
            skip.onClick.AddListener((() => isSkip = true));
        }

        private void OnDisable()
        {
            play.onClick.RemoveListener(PlayGame);
            setting.onClick.RemoveListener(SettingGame);
            quit.onClick.RemoveListener(QuitGame);
        }
        
        private void PlayGame()
        {
            StartCoroutine(WaitVideo());
        }

        private IEnumerator WaitVideo()
        {
            videoPlayer.Play();
            videoPanel.gameObject.SetActive(true);
            videoPlayer.gameObject.SetActive(true);
            StartCoroutine(WaitVideoTime());
            yield return new WaitUntil((() => isSkip));
            videoPanel.gameObject.SetActive(false);
            loadingScenePanel.gameObject.SetActive(true);
            videoPlayer.gameObject.SetActive(false);
            StopAllCoroutines();
        }

        private IEnumerator WaitVideoTime()
        {
            var time = videoPlayer.length;
            yield return new WaitForSecondsRealtime((float)(time + 1));
            videoPanel.gameObject.SetActive(false);
            loadingScenePanel.gameObject.SetActive(true);
            videoPlayer.gameObject.SetActive(false);
            StopAllCoroutines();
        }
        
        private void SettingGame()
        {
            settingPanel.gameObject.SetActive(true);
            //this.gameObject.SetActive(false);
        }
        
        private void QuitGame()
        {
            Application.Quit();
        }
    }
}