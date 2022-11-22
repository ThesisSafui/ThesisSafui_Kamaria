using System;
using System.Collections;
using System.Collections.Generic;
using Kamaria.UI.UIMainGame;
using Kamaria.Utilities;
using Kamaria.Utilities.GameEvent;
using Kamaria.Utilities.PoolingPattern;
using Kamaria.Utilities.SaveLoad;
using TMPro;
using UnityEngine;
using Random = System.Random;

namespace Kamaria.Manager
{
    public enum Map
    {
        AbandonIsland ,ForbiddenIsland ,ShipOfDeath, Lobby
    }

    public enum Dungeons
    {
        BootyCove, DeadIsle, DeadMansHeaven, None
    }

    [Serializable]
    public sealed class MapUIText
    {
        [SerializeField] private Map map;
        [SerializeField] private TextMeshProUGUI text;

        public Map Map => map;
        public TextMeshProUGUI Text => text;
    }
    
    public sealed class GameManagerFarming : MonoBehaviour
    {
        public event Action RestartGame;
        [SerializeField] private GameEventSO gotoLobbyEvent;
        [SerializeField] private Map map;
        [SerializeField] private Dungeons dungeon;
        [SerializeField] private TextMeshProUGUI textDungeon;
        [SerializeField] private TextMeshProUGUI floorState;
        [SerializeField] private TextMeshProUGUI floor;
        [SerializeField] private List<MapUIText> mapUIText;
        [SerializeField] private SaveLoadDataSO saveLoadDataSO;
        [SerializeField] private GameEventSO eventLoadData;
        [SerializeField] private FarmingManagerSO farmingManagerSO;
        [SerializeField] private GameObject player;
        [SerializeField] private float resetSpawnEnemies;
        [SerializeField] private float resetSpawnPlayer;
        [SerializeField] private List<PatternSpawn> patterns;
        [SerializeField] private List<CampEnemy> campsEnemies;
        [SerializeField] private List<GameObject> camps = new List<GameObject>();
        [SerializeField] private UIDead uiDead;
        [SerializeField] private List<GameObject> gates;
        [SerializeField] private List<GameObject> treasureChests;
        [SerializeField] private int countTreasureChest;
        [SerializeField] private RectTransform uiLoading;

        [Header("NotFarming")] 
        [SerializeField] private Transform spawnPlayerNotFarming;

        [Header("Fix")]
        [SerializeField] private GameObject treasureFix;
        [SerializeField] private GameObject boss;
        [SerializeField] private Transform positionBossSpawn;
        [SerializeField] private List<GameObject> enemiesSummon;

        private Random random = new Random();
        private PatternSpawn currentPattern;
        private List<GameObject> treasureChestsActive = new List<GameObject>();
        public Dungeons Dungeon => dungeon;
        public Map Map => map;

        private void Awake()
        {
            farmingManagerSO.CurrentPotion = farmingManagerSO.MaxPotion;
            farmingManagerSO.Dungeons = dungeon;
            saveLoadDataSO.LoadDataFinish = false;
            player.SetActive(false);

            SpawnBoss(false);
            //Initialize();
            StartCoroutine(WaitData());
            //SoundBgm();
            SoundBgm();
        }

        private void SoundBgm()
        {
            if (map == Map.Lobby)
            {
                SoundHandler.Instance.PlayBGM(SoundClip.Sound.BGMLobby);
                SoundHandler.Instance.StopBGM(SoundClip.Sound.BGMBattle);
            }
            else
            {
                SoundHandler.Instance.PlayBGM(SoundClip.Sound.BGMBattle);
                SoundHandler.Instance.StopBGM(SoundClip.Sound.BGMLobby);
            }
        }
        
        public void ResetFloor()
        {
            farmingManagerSO.ResetFloor();
        }

        private void Initialize()
        {
            if (map == Map.Lobby)
            {
                SetTextMap();
                SpawnPlayerNotFarming();
            }
            else if (map == Map.AbandonIsland)
            {
                uiDead.gameObject.SetActive(false);
                currentPattern = patterns[random.Next(patterns.Count)];
                //currentPattern = patterns[3]; //TODO: Change.

                SetTextMap();
                SetWarpGate();
                ActiveTreasureChest();
                SpawnEnemy();
                SpawnPlayer();
            }
            else if (map is Map.ForbiddenIsland or Map.ShipOfDeath)
            {
                SpawnBoss(true);
                SetTextMap();
                SpawnPlayerNotFarming();
            }
        }

        private IEnumerator WaitData()
        {
            yield return new WaitUntil(() => saveLoadDataSO.LoadDataFinish);
            Debug.Log("LoadDataFinish2");
            Initialize();
        }

        private void SpawnPlayerNotFarming()
        {
            player.transform.position = spawnPlayerNotFarming.position;
            player.SetActive(true);
            uiLoading.gameObject.SetActive(false);
        }

        private void SpawnBoss(bool isSpawn)
        {
            if (boss == null) return;
            
            boss.transform.position = positionBossSpawn.position;
            boss.SetActive(isSpawn);
            
            if (!isSpawn)
            {
                for (int i = 0; i < enemiesSummon.Count; i++)
                {
                    enemiesSummon[i].SetActive(false);
                }
            }
        }

        private void SetTextMap()
        {
            textDungeon.gameObject.SetActive(false);
            for (int i = 0; i < mapUIText.Count; i++)
            {
                mapUIText[i].Text.gameObject.SetActive(false);
            }

            switch (map)
            {
                case Map.ShipOfDeath or Map.ForbiddenIsland or Map.Lobby:
                    floorState.gameObject.SetActive(false);
                    floor.gameObject.SetActive(false);
                    mapUIText.Find(x => x.Map == map).Text.gameObject.SetActive(true);
                    break;
                case Map.AbandonIsland:
                    floorState.gameObject.SetActive(true);
                    floor.gameObject.SetActive(true);
                    floorState.text = farmingManagerSO.CurrentFloor.ToString();
                    textDungeon.gameObject.SetActive(true);
                    break;
            }
        }

        private void SpawnPlayer()
        {
            ActivePlayer(true);
            uiLoading.gameObject.SetActive(false);
        }

        private void SpawnEnemy()
        {
            ActiveEnemy(true);
        }
        
        private void ActivePlayer(bool isActive)
        {
            farmingManagerSO.CurrentPotion = farmingManagerSO.MaxPotion;
            
            if (isActive)
            {
                player.transform.position = currentPattern.SpawnPlayerPos.position;
            }
            
            player.SetActive(isActive);
        }

        private void ActiveEnemy(bool isActive)
        {
            for (int i = 0; i < campsEnemies.Count; i++)
            {
                for (int j = 0; j < campsEnemies[i].Enemies.Count; j++)
                {
                    if (isActive)
                    {
                        campsEnemies[i].Enemies[j].transform.position = campsEnemies[i].SpawnsEnemyPos.position;
                    }
                    
                    campsEnemies[i].Enemies[j].SetActive(isActive);
                }
            }
            
            CloseCamps();
        }

        private void CloseCamps()
        {
            for (int i = 0; i < camps.Count; i++)
            {
                camps[i].SetActive(false);
            }
        }

        private void SetWarpGate()
        {
            for (int i = 0; i < patterns.Count; i++)
            {
                patterns[i].WarpGate.SetActive(false);
            }
            
            currentPattern.WarpGate.SetActive(true);
        }

        public void ResetGame()
        {
            farmingManagerSO.CurrentPotion = farmingManagerSO.MaxPotion;
            uiDead.gameObject.SetActive(true);
            StartCoroutine(nameof(UIDeadTime));
        }

        private IEnumerator UIDeadTime()
        {
            yield return new WaitUntil((() => uiDead.IsChoseFinish));
            
            if (uiDead.IsTryAgain)
            {
                RestartGame?.Invoke();
                ResetGameObject();
                StartCoroutine(ResetGameTime());
            }
            else
            {
                gotoLobbyEvent.TriggerEvent();
            }
            
        }
        
        private void ResetGameObject()
        {
            foreach (GameObject gate in gates)
            {
                gate.SetActive(false);
                gate.SetActive(true);
            }
            
            foreach (GameObject treasureChestActive in treasureChestsActive)
            {
                treasureChestActive.SetActive(false);
                treasureChestActive.SetActive(true);
            }
            
            if (treasureFix != null)
            {
                treasureFix.gameObject.SetActive(false);
                treasureFix.gameObject.SetActive(true);
            }

            var item = PoolManager.Instance.poolObjectSets.Find(x =>
                x.poolObjectType == PoolManager.PoolObjectType.ItemDrop);

            for (int i = 0; i < item.pooledObjects.Count; i++)
            {
                item.pooledObjects[i].SetActive(false);
            }
            
            var meteor = PoolManager.Instance.poolObjectSets.Find(x =>
                x.poolObjectType == PoolManager.PoolObjectType.Meteor);
            
            for (int i = 0; i < meteor.pooledObjects.Count; i++)
            {
                meteor.pooledObjects[i].SetActive(false);
            }
        }

        private void ActiveTreasureChest()
        {
            int lastIndex;
            int indexTreasureChestsActive = 0;
            
            foreach (var treasureChest in treasureChests)
            {
                treasureChest.SetActive(false);
            }

            for (int i = 0; i < countTreasureChest; i++)
            {
                lastIndex = indexTreasureChestsActive;
                indexTreasureChestsActive = random.Next(treasureChests.Count);

                if (treasureChestsActive.Count != 0)
                {
                    while (indexTreasureChestsActive == lastIndex)
                    {
                        indexTreasureChestsActive = random.Next(treasureChests.Count);
                    }
                }
                
                treasureChestsActive.Add(treasureChests[indexTreasureChestsActive]);
            }
            
            foreach (var treasureChestActive in treasureChestsActive)
            {
                treasureChestActive.SetActive(true);
            }

            if (treasureFix != null)
            {
                treasureFix.gameObject.SetActive(true);
            }
        }

        public void ResetDebugMode()
        {
            /*player.SetActive(false);

            foreach (GameObject enemy in enemies)
            {
                enemy.SetActive(false);
            }
            
            player.transform.position =
                patterns.Find(x => x.PatternsSpawn == PatternsSpawn.Pattern1).SpawnPlayerPos.position;
            
            player.SetActive(true);

            foreach (GameObject enemy in enemies)
            {
                enemy.transform.position = spawnsEnemyPos[0].position;
                enemy.SetActive(true);
            }*/
        }

        private IEnumerator ResetGameTime()
        {
            if (map == Map.AbandonIsland)
            {
                uiLoading.gameObject.SetActive(true);
                ActivePlayer(false);
                ActiveEnemy(false);
                eventLoadData.TriggerEvent(); //TODO:OPEN
                saveLoadDataSO.LoadDataFinish = false;
                yield return new WaitForSecondsRealtime(resetSpawnEnemies);
            
                SpawnEnemy();
            
                yield return new WaitForSecondsRealtime(resetSpawnPlayer);
                yield return new WaitUntil((() => saveLoadDataSO.LoadDataFinish)); //TODO:OPEN
                SpawnPlayer();
            
                Time.timeScale = 1;
            }
            else
            {
                uiLoading.gameObject.SetActive(true);
                player.SetActive(false);
                SpawnBoss(false);
                eventLoadData.TriggerEvent(); //TODO:OPEN
                saveLoadDataSO.LoadDataFinish = false;
                yield return new WaitForSecondsRealtime(resetSpawnEnemies);

                SpawnBoss(true);
            
                yield return new WaitForSecondsRealtime(resetSpawnPlayer);
                yield return new WaitUntil((() => saveLoadDataSO.LoadDataFinish)); //TODO:OPEN
                SpawnPlayerNotFarming();
            
                Time.timeScale = 1;
            }
        }
    }

    public enum PatternsSpawn
    {
        Pattern1, Pattern2,
        Pattern3, Pattern4
    }
}