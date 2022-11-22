using System.Collections;
using Kamaria.Player.Data;
using Kamaria.Player.Data.Quest;
using Kamaria.SaveLoad;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace Kamaria.UI.Portal
{
    public sealed class UIPortal : MonoBehaviour
    {
        [SerializeField] private SaveLoadManager saveLoadManager;
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private QuestManagerSO questManagerSO;
        [SerializeField] private Button farming;
        [SerializeField] private Button skeletonBoss;
        [SerializeField] private Button sharkBoss;
        [SerializeField] private Button exit;
        [SerializeField] private UILoadingScene loadingScenePanel;

        [SerializeField] private string[] farmingScenes;
        [SerializeField] private string skeletonBossScenes;
        [SerializeField] private string sharkBossScenes;

        public string SkeletonBossScenes => skeletonBossScenes;
        public string SharkBossScenes => sharkBossScenes;
        public UILoadingScene LoadingScenePanel => loadingScenePanel;
        public SaveLoadManager SaveLoadManager => saveLoadManager;

        private Random random = new Random();

        public string loadScene { get; set; }
        
        private void OnEnable()
        {
            farming.onClick.AddListener(delegate { LoadScene(farmingScenes[random.Next(farmingScenes.Length)]); });
            skeletonBoss.onClick.AddListener((() => LoadScene(skeletonBossScenes)));
            sharkBoss.onClick.AddListener((() => LoadScene(sharkBossScenes)));
            exit.onClick.AddListener(Exit);
            SetUI();
            playerData.IsUsingUI = true;
            Time.timeScale = 0;
        }

        private void OnDisable()
        {
            farming.onClick.RemoveAllListeners();
            skeletonBoss.onClick.RemoveAllListeners();
            sharkBoss.onClick.RemoveAllListeners();
            exit.onClick.RemoveListener(Exit);
            
            playerData.IsUsingUI = false;
            Time.timeScale = 1;
        }

        private void LoadScene(string scene)
        {
            saveLoadManager.SaveData(true);
            loadScene = scene;
        }

        public void LoadSceneNext()
        {
            loadingScenePanel.LoadScene(loadScene);
            loadingScenePanel.gameObject.SetActive(true);
        }
        
        private void Exit()
        {
            this.gameObject.SetActive(false);
        }

        private void SetUI()
        {
            BaseQuest questDefeatTheBossSharkPirate =
                questManagerSO.MainQuests.Find(x => x.NameQuest == NameQuest.DefeatTheBossSharkPirate);
            BaseQuest questDefeatTheBossSkeletonPirate =
                questManagerSO.MainQuests.Find(x => x.NameQuest == NameQuest.KingOfTheSea);

            sharkBoss.interactable = questDefeatTheBossSharkPirate.IsSucceed || questDefeatTheBossSharkPirate.IsDoing;
            skeletonBoss.interactable = questDefeatTheBossSkeletonPirate.IsSucceed || questDefeatTheBossSkeletonPirate.IsDoing;
        }
    }
}