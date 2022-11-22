using Kamaria.Player.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI.OptionMenu
{
    public sealed class UIOptionMenu : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private Button optionMenu;
        [SerializeField] private Button island;
        [SerializeField] private Button exitGame;
        [SerializeField] private Button exitPanel;
        [Space] 
        [SerializeField] private RectTransform uiSetting;
        [SerializeField] private UILoadingScene uiLoadingScene;
        [SerializeField] private string lobbyScene;
        [SerializeField] private RectTransform confirmGotoLobby;
       
        private void OnEnable()
        {
            confirmGotoLobby.gameObject.SetActive(false);
            uiSetting.gameObject.SetActive(false);
            
            optionMenu.onClick.AddListener(OptionOpen);
            island.onClick.AddListener(GotoLobby);
            exitPanel.onClick.AddListener(Exit);
            exitGame.onClick.AddListener(ExitGame);
            
            Time.timeScale = 0;
            playerData.IsUsingUI = true;
        }

        private void OnDisable()
        {
            optionMenu.onClick.RemoveListener(OptionOpen);
            island.onClick.RemoveListener(GotoLobby);
            exitPanel.onClick.RemoveListener(Exit);
            exitGame.onClick.RemoveListener(ExitGame);
            
            Time.timeScale = 1;
            playerData.IsUsingUI = false;
        }
        
        private void GotoLobby()
        {
            confirmGotoLobby.gameObject.SetActive(true);
        }
        
        private void OptionOpen()
        {
            uiSetting.gameObject.SetActive(true);
        }
        
        private void Exit()
        {
            this.gameObject.SetActive(false);
        }

        private void ExitGame()
        {
            Application.Quit();
        }
    }
}