using System.Collections;
using Kamaria.Player.Data;
using Kamaria.SaveLoad;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.Authentication
{
    internal sealed class AuthenticationManager : MonoBehaviour
    {
        #region REFERENCE_VARIABLE

        [SerializeField] private float waitingTimeBeforeLogin;
        [SerializeField] private float waitingTimeAfterShowSavingLog;
        [SerializeField] private float waitingTimeAfterShowResetLog;

        [Space]
        [SerializeField] private PlayerDataSO playerData;
        
        [Header("Objects")]
        [SerializeField] private RectTransform firstLogin;
        [SerializeField] private RectTransform gameMenu;
        [SerializeField] private RectTransform loginFailedPanel;
        [SerializeField] private RectTransform loadingLoginPanel;
        [SerializeField] private RectTransform savingDataPanel;
        [SerializeField] private RectTransform resetDataPanel;

        [Header("Text")]
        [SerializeField] private TextMeshProUGUI steamName;
        [SerializeField] private TextMeshProUGUI errorLoginLog;
        [SerializeField] private TextMeshProUGUI savingLog;
        [SerializeField] private TextMeshProUGUI resetLog;
        
        [Header("Button")]
        [SerializeField] private Button submit;
        [SerializeField] private Button reconnect;
        [SerializeField] private Button newGame;
        [SerializeField] private Button quit;

        #endregion

        #region PRIVATE_VARIABLE

        private bool isSavingFailed = false;

        #endregion
        
        private void Awake()
        {
            StartCoroutine(nameof(Login));
        }

        private void OnEnable()
        {
            submit.onClick.AddListener(FirstLoginSubmit);
            reconnect.onClick.AddListener(Reconnect);
            newGame.onClick.AddListener(NewGame);
            quit.onClick.AddListener(CloseApplication);
            
            AuthenticationHandler.OnLoginSuccess += EventOnLoginSuccess;
            AuthenticationHandler.OnLoginFailed += EventOnLoginFailed;
            
            SaveLoadHandler.OnLoadData += EventOnLoadData;
            SaveLoadHandler.OnLoadDataFailed += EventOnLoginFailed;
            SaveLoadHandler.OnSavingSuccess += EventOnSavingSuccess;
            SaveLoadHandler.OnSavingFailed += EventOnSavingFailed;
            SaveLoadHandler.OnResetDataSuccess += EventOnResetDataSuccess;
            SaveLoadHandler.OnResetDataFailed += EventOnResetDataFailed;
        }

        private void OnDisable()
        {
            submit.onClick.RemoveListener(FirstLoginSubmit);
            reconnect.onClick.RemoveListener(Reconnect);
            newGame.onClick.RemoveListener(NewGame);
            quit.onClick.RemoveListener(CloseApplication);

            AuthenticationHandler.OnLoginSuccess -= EventOnLoginSuccess;
            AuthenticationHandler.OnLoginFailed -= EventOnLoginFailed;
            
            SaveLoadHandler.OnLoadData -= EventOnLoadData;
            SaveLoadHandler.OnLoadDataFailed -= EventOnLoginFailed;
            SaveLoadHandler.OnSavingSuccess -= EventOnSavingSuccess;
            SaveLoadHandler.OnSavingFailed -= EventOnSavingFailed;
            SaveLoadHandler.OnResetDataSuccess -= EventOnResetDataSuccess;
            SaveLoadHandler.OnResetDataFailed -= EventOnResetDataFailed;
        }

        #region EVENTS

        private void EventOnLoginSuccess(string callback)
        {
            ShowPanel(loginFailedPanel, false);
            StartCoroutine(nameof(LoadPlayerData));
            steamName.text = callback;
            steamName.gameObject.SetActive(true);
        }
        
        private void EventOnLoginFailed(string callback)
        {
            steamName.gameObject.SetActive(false);
            ShowPanel(loadingLoginPanel, false);
            ShowPanel(loginFailedPanel, true);

            errorLoginLog.text = $"{callback}";
        }
        
        private void EventOnLoadData()
        {
            StartCoroutine(SaveLoadHandler.GetData(playerData.Info));
        }
        
        private void EventOnSavingSuccess(string callback)
        {
            isSavingFailed = false;
            savingLog.text = callback;
            savingLog.gameObject.SetActive(true);
            SaveLoadHandler.SaveDataFinish = true;
        }
        
        private void EventOnSavingFailed(string callback)
        {
            isSavingFailed = true;
            savingLog.text = callback;
            savingLog.gameObject.SetActive(true);
            SaveLoadHandler.SaveDataFinish = true;
        }
        
        private void EventOnResetDataSuccess(string callback)
        {
            resetLog.text = callback;
            resetLog.gameObject.SetActive(true);
            SaveLoadHandler.ResetDataFinish = true;
        }
        
        private void EventOnResetDataFailed(string callback)
        {
            resetLog.text = callback;
            resetLog.gameObject.SetActive(true);
            SaveLoadHandler.ResetDataFinish = true;
        }
        
        #endregion

        private void FirstLoginSubmit()
        {
            playerData.Info.Authentication.FirstLogin = false;
            SaveLoadHandler.SaveDataFinish = false;
            StartCoroutine(nameof(SavePlayerData));
        }
        
        private void Reconnect()
        {
            StartCoroutine(nameof(Login));
        }
        
        private void NewGame()
        {
            StartCoroutine(nameof(ResetData));
        }

        private void CloseApplication()
        {
            Application.Quit();
        }
        
        private IEnumerator Login()
        {
            ShowPanel(loadingLoginPanel, true);
            yield return new WaitForSeconds(waitingTimeBeforeLogin);
            AuthenticationHandler.LoginSteam();
        }

        private IEnumerator SavePlayerData()
        {
            SaveLoadHandler.SaveData(playerData.Info);
            ShowPanel(savingDataPanel, true);
            yield return new WaitUntil(() => SaveLoadHandler.SaveDataFinish);
            yield return new WaitForSeconds(waitingTimeAfterShowSavingLog);
            ShowPanel(savingDataPanel, false);
            savingLog.gameObject.SetActive(false);

            if (isSavingFailed) yield break;
            
            ShowPanel(firstLogin, false);
            ShowPanel(gameMenu, true);
        }
        
        private IEnumerator LoadPlayerData()
        {
            SaveLoadHandler.LoadData(playerData.Info);
            SaveLoadHandler.LoadDataFinish = false;
            yield return new WaitUntil(() => SaveLoadHandler.LoadDataFinish);
            ShowPanel(loadingLoginPanel, false);
            playerData.DebugLog();

            ShowPanel(playerData.Info.Authentication.FirstLogin ? firstLogin : gameMenu, true);
            
            /*var currentWeapon = playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.IsUsedEquip);
                
            if (currentWeapon == null)
            {
                playerData.Info.DeviceCompartment.SlotWeapon.Add(
                    playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Sword),
                    playerData);
            }
            else
            {
                var weaponBefore = playerData.Info.DeviceCompartment.SlotWeapon.Info;
                weaponBefore.IsUsedEquip = false;
                playerData.Info.DeviceCompartment.SlotWeapon.Info = currentWeapon;
                /*playerData.Info.DeviceCompartment.SlotWeapon.Remove(playerData.Info.DeviceCompartment.SlotWeapon.Info,
                    playerData);
                
                playerData.Info.DeviceCompartment.SlotWeapon.Add(currentWeapon, playerData);#1#
            }*/
            
            /*if (playerData.Info.DeviceCompartment.SlotWeapon.IsEmpty)
            {
                var currentWeapon = playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.IsUsedEquip);
                
                if (currentWeapon == null)
                {
                    playerData.Info.DeviceCompartment.SlotWeapon.Add(
                        playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Sword),
                        playerData);
                }
                else
                {
                    playerData.Info.DeviceCompartment.SlotWeapon.Add(currentWeapon, playerData);
                }
            }*/
        }

        private IEnumerator ResetData()
        {
            ShowPanel(resetDataPanel, true);
            SaveLoadHandler.ResetGame(playerData.Info);
            SaveLoadHandler.ResetDataFinish = false;
            yield return new WaitUntil(() => SaveLoadHandler.ResetDataFinish);
            yield return new WaitForSeconds(waitingTimeAfterShowResetLog);
            ShowPanel(resetDataPanel, false);
            resetLog.gameObject.SetActive(false);
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
    }
}