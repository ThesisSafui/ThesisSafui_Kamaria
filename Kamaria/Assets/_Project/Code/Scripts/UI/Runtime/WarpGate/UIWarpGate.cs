using Kamaria.Manager;
using Kamaria.Player.Data;
using Kamaria.SaveLoad;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Kamaria.UI.WarpGate
{
    public sealed class UIWarpGate : MonoBehaviour
    {
        [SerializeField] private FarmingManagerSO farmingManager;
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private SaveLoadManager saveLoadManager;
        [SerializeField] private UILoadingScene uiLoadingScene;
        [SerializeField] private Button gotoLobby;
        [SerializeField] private Button nextMap;
        [SerializeField] private Button exit;
        [SerializeField] private string[] farmingScenes;
        [SerializeField] private string lobbyScenes;
        
        private Random random = new Random();
        
        private void OnEnable()
        {
            gotoLobby.onClick.AddListener(delegate
            {
                farmingManager.ResetFloor();
                SetLoadScene(lobbyScenes);
            });
            nextMap.onClick.AddListener(delegate
            {
                farmingManager.CurrentFloor += 1;
                SetLoadScene(farmingScenes[random.Next(farmingScenes.Length)]);
            });
            exit.onClick.AddListener(Exit);
            
            playerData.IsUsingUI = true;
            Time.timeScale = 0;
        }

        private void OnDisable()
        {
            gotoLobby.onClick.RemoveAllListeners();
            nextMap.onClick.RemoveAllListeners();
            exit.onClick.RemoveListener(Exit);
            
            playerData.IsUsingUI = false;
            Time.timeScale = 1;
        }

        private void SetLoadScene(string loadScene)
        {
            saveLoadManager.SaveData(true);
            uiLoadingScene.LoadScene(loadScene);
        }        
        
        private void Exit()
        {
            this.gameObject.SetActive(false);
        }
        
        public void NextScene()
        {
            uiLoadingScene.gameObject.SetActive(true);
        }
    }
}