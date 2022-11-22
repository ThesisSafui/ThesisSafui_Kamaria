using System.Collections;
using Kamaria.Item;
using Kamaria.Player.Controller;
using Kamaria.Player.Data;
using Kamaria.Utilities.GameEvent;
using Kamaria.Utilities.SaveLoad;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.SaveLoad
{
    public sealed class SaveLoadManager : MonoBehaviour
    {
        [SerializeField] private SaveLoadDataSO saveLoadDataSO;
        [SerializeField] private CharacterSkill characterSkill;
        [SerializeField] private GameEventSO eventLoadNextScene;
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private RectTransform savingDataPanel;
        [SerializeField] private RectTransform loadingDataPanel;
        [SerializeField] private TextMeshProUGUI savingLog;
        [SerializeField] private TextMeshProUGUI loadingLog;
        [SerializeField] private RectTransform loadingErrorPanel;
        [SerializeField] private Button loadingQuit;
        [SerializeField] private Button tryAgain;
        [SerializeField] private RectTransform uiConfirm;

        #region PRIVATE_VARIABLE
        
        private bool isSavingFailed = false;
        private bool isLoadNextScene = false;
        private float waitingTimeAfterShowSavingLog = 1f;

        #endregion

        private void Awake()
        {
            isLoadNextScene = false;
            LoadData();
        }

        private void OnEnable()
        {
            tryAgain.onClick.AddListener(TryAgainLoadData);
            loadingQuit.onClick.AddListener(Quit);
            
            SaveLoadHandler.OnLoadData += EventOnLoadData;
            SaveLoadHandler.OnLoadDataFailed += EventOnLoadFailed;
            SaveLoadHandler.OnSavingSuccess += EventOnSavingSuccess;
            SaveLoadHandler.OnSavingFailed += EventOnSavingFailed;
        }

        private void OnDisable()
        {
            tryAgain.onClick.RemoveListener(TryAgainLoadData);
            loadingQuit.onClick.RemoveListener(Quit);
            
            SaveLoadHandler.OnLoadData -= EventOnLoadData;
            SaveLoadHandler.OnLoadDataFailed -= EventOnLoadFailed;
            SaveLoadHandler.OnSavingSuccess -= EventOnSavingSuccess;
            SaveLoadHandler.OnSavingFailed -= EventOnSavingFailed;
        }

        public void SaveData(bool isLoadNextScene)
        {
            this.isLoadNextScene = isLoadNextScene;
            StartCoroutine(nameof(SavePlayerData));
        }
        
        public void LoadData()
        {
            saveLoadDataSO.LoadDataFinish = false;
            StartCoroutine(nameof(LoadPlayerData));
        }
        
        private IEnumerator SavePlayerData()
        {
            var weapon = playerData.Info.DeviceCompartment.SlotWeapon.Info;
            playerData.Info.DeviceCompartment.SlotWeapon.Remove(weapon, playerData);
            SaveLoadHandler.SaveData(playerData.Info);
            SaveLoadHandler.SaveDataFinish = false;
            ShowPanel(savingDataPanel, true);
            yield return new WaitUntil(() => SaveLoadHandler.SaveDataFinish);
            yield return new WaitForSecondsRealtime(waitingTimeAfterShowSavingLog);
            playerData.Info.DeviceCompartment.SlotWeapon.Add(weapon, playerData);
            ShowPanel(savingDataPanel, false);
            savingLog.gameObject.SetActive(false);

            if (uiConfirm != null)
            {
                uiConfirm.gameObject.SetActive(false);
            }
            
            if (isSavingFailed) yield break;

            if (isLoadNextScene)
            {
                playerData.Info.DeviceCompartment.SlotWeapon.Remove(weapon, playerData);
                eventLoadNextScene.TriggerEvent();
            }
        }
        
        private IEnumerator LoadPlayerData()
        {
            SaveLoadHandler.LoadData(playerData.Info);
            SaveLoadHandler.LoadDataFinish = false;
            ShowPanel(loadingDataPanel, true);
            yield return new WaitUntil(() => SaveLoadHandler.LoadDataFinish);

            var currentWeapon = playerData.Info.Inventory.InventoryWeapon.BaseWeaponSO.Find(x => x.Info.IsUsedEquip);
            
            if (currentWeapon == null)
            {
                playerData.Info.DeviceCompartment.SlotWeapon.Add(
                    playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Sword),
                    playerData);
            }
            else
            {
                currentWeapon.Info.IsUsedEquip = false;
                var weapon =
                    playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x =>
                        x.WeaponType == currentWeapon.Info.WeaponType);
                playerData.Info.DeviceCompartment.SlotWeapon.Add(weapon, playerData);
                /*var weaponBefore = playerData.Info.DeviceCompartment.SlotWeapon.Info;
                weaponBefore.IsUsedEquip = false;
                playerData.Info.DeviceCompartment.SlotWeapon.Info = currentWeapon;*/
                /*playerData.Info.DeviceCompartment.SlotWeapon.Remove(playerData.Info.DeviceCompartment.SlotWeapon.Info,
                    playerData);
                
                playerData.Info.DeviceCompartment.SlotWeapon.Add(currentWeapon, playerData);*/
            }
            
            Debug.Log("LoadDataFinish1");
            saveLoadDataSO.LoadDataFinish = SaveLoadHandler.LoadDataFinish;
            ShowPanel(loadingDataPanel, false);
            playerData.DebugLog();
        }

        private void EventOnSavingSuccess(string callback)
        {
            isSavingFailed = false;
            SavingLog(callback);
        }

        private void EventOnSavingFailed(string callback)
        {
            isSavingFailed = true;
            SavingLog(callback);
        }
        
        private void SavingLog(string callback)
        {
            savingLog.text = callback;
            savingLog.gameObject.SetActive(true);
            SaveLoadHandler.SaveDataFinish = true;
        }

        private void EventOnLoadFailed(string callback)
        { 
            loadingLog.text = callback;
            loadingErrorPanel.gameObject.SetActive(true);
            loadingLog.gameObject.SetActive(true);
        }

        private void EventOnLoadData()
        {
            StartCoroutine(SaveLoadHandler.GetData(playerData.Info));
        }
        
        private void TryAgainLoadData()
        {
            StopCoroutine(nameof(LoadPlayerData));
            ShowPanel(loadingDataPanel, false);
            loadingErrorPanel.gameObject.SetActive(false);
            loadingLog.gameObject.SetActive(false);
            LoadData();
        }
        
        /// <summary>
        /// Show panel or not.
        /// </summary>
        /// <param name="panel"> Panel </param>
        /// <param name="isShow"> True = show / False = not show </param>
        private void ShowPanel(RectTransform panel, bool isShow)
        {
            panel.gameObject.SetActive(isShow);
        }
        
        private void Quit()
        {
            Application.Quit();
        }
    }
}