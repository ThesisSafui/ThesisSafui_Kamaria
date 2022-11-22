using Kamaria.Player.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI.OptionMenu
{
    public sealed class UIConfirmGoToLobby : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;
        [SerializeField] private UILoadingScene uiLoadingScene;
        [SerializeField] private string loadScene;

        private void OnEnable()
        {
            yesButton.onClick.AddListener(GotoLobby);
            noButton.onClick.AddListener((() => gameObject.SetActive(false)));
            
            Time.timeScale = 0;
            playerData.IsUsingUI = true;
        }
        
        private void OnDisable()
        {
            yesButton.onClick.RemoveListener(GotoLobby);
            noButton.onClick.RemoveAllListeners();
            
            /*Time.timeScale = 1;
            playerData.IsUsingUI = false;*/
        }
        
        private void GotoLobby()
        {
            uiLoadingScene.LoadScene(loadScene);
            uiLoadingScene.StartLoadScene();
        }
    }
}