using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Kamaria.UI
{
    public sealed class UILoadingScene : MonoBehaviour
    {
        #region REFERENCE_VARIABLE

        [SerializeField] private string scene;
        [SerializeField] private float waitingTimeBeforeLoading;
        [SerializeField] private TextMeshProUGUI progressText;
        [SerializeField] private Image loadingFill;

        #endregion

        private void OnEnable()
        {
            StartCoroutine(nameof(LoadSceneAsync));
        }
        
        private void OnDisable()
        {
            StopCoroutine(nameof(LoadSceneAsync));
        }

        private IEnumerator LoadSceneAsync()
        {
            yield return new WaitForSecondsRealtime(waitingTimeBeforeLoading);
            
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
            
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / 0.9f);

                loadingFill.fillAmount = progress;
                progressText.text = $"{progress * 100}%";
                
                yield return null;
            }
        }

        public void LoadScene(string scene)
        {
            this.scene = scene;
        }

        public void StartLoadScene()
        {
            this.gameObject.SetActive(true);
        }
    }
}