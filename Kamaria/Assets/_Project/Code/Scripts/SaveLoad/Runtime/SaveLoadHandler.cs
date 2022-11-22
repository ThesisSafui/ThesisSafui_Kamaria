using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using Kamaria.Player.Data;
using LootLocker.Requests;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Networking;

namespace Kamaria.SaveLoad
{
    public static class SaveLoadHandler
    {
        #region EVENTS

        /// <summary>
        /// Event load data.
        /// </summary>
        public static event Action OnLoadData;

        /// <summary>
        /// Event load data failed.
        /// Callback error.
        /// </summary>
        public static event Action<string> OnLoadDataFailed; 

        /// <summary>
        /// Event saving failed.
        /// Callback error.
        /// </summary>
        public static event Action<string> OnSavingFailed;
        
        /// <summary>
        /// Event saving success.
        /// Callback success.
        /// </summary>
        public static event Action<string> OnSavingSuccess;
        
        /// <summary>
        /// Event reset data success.
        /// Callback success.
        /// </summary>
        public static event Action<string> OnResetDataSuccess;
        
        /// <summary>
        /// Event reset data failed.
        /// Callback failed.
        /// </summary>
        public static event Action<string> OnResetDataFailed;

        #endregion

        #region PUBLIC_VARIABLE

        /// <summary>
        /// Save data finish or not. Return bool.
        /// </summary>
        public static bool SaveDataFinish = false;

        /// <summary>
        /// Load data finish or not. Return bool.
        /// </summary>
        public static bool LoadDataFinish = false;
        
        /// <summary>
        /// Reset data finish or not. Return bool.
        /// </summary>
        public static bool ResetDataFinish = false;

        #endregion

        #region PRIVATE_VARIABLE
        
        private static string savePath = String.Empty;
        private static string directoryPath = String.Empty;
        private static LootLockerPlayerFile saveFile;
        
        private const string SAVE_FILE = "SAVE_GAME.sav";               // File name.
        private const string SAVE_DIRECTOYR = "/SaveData/";             // Directory name.
        private const string PURPOSE = "SAVE_GAME";                     // Purpose name.
        private const string SAVING_SUCCESS = "Saving success";         // Callback saving success.
        private const string SAVING_FAILED = "Saving failed";           // Callback saving failed.
        private const string LOAD_DATA_FAILED = "Loading failed";       // Callback loading failed.
        private const string RESET_DATA_SUCCESS = "Reset data success"; // Callback reset success.
        private const string RESET_DATA_FAILED = "Reset data failed";   // Callback reset failed.

        #endregion

        private static void SetPath()
        {
            directoryPath = Application.persistentDataPath + SAVE_DIRECTOYR;
            savePath = directoryPath + SAVE_FILE;

            // If an existing directory on disk is not obtained create a new directory.
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            // If an existing file on disk is not obtained create a new file.
            if (!File.Exists(savePath)) File.WriteAllText(savePath, "New File", Encoding.UTF8);

            // Get path to look file.
            GUIUtility.systemCopyBuffer = directoryPath;
        }

        /// <summary>
        /// Save player data.
        /// </summary>
        public static void SaveData(PlayerData playerData)
        {
            SetPath();
            
            string json = JsonUtility.ToJson(playerData, true);
            
            File.WriteAllText(savePath, json, Encoding.UTF8);

            SetData(false);
        }
        
        /// <summary>
        /// Load player data.
        /// </summary>
        public static void LoadData(PlayerData playerData)
        {
            SetPath();
            
            LootLockerSDKManager.GetAllPlayerFiles(playerFilesResponse =>
            {
                if (playerFilesResponse.success)
                {
                    if (playerFilesResponse.items.Length == 0)
                    {
                        SetFirstFilePlayer(playerData);
                    }
                    else
                    {
                        var files = playerFilesResponse.items.Where(x => x.name == SAVE_FILE);

                        foreach (LootLockerPlayerFile lootLockerPlayerFile in files)
                        {
                            saveFile = lootLockerPlayerFile;
                        }

                        Debug.Log("Load data");
                        OnLoadData?.Invoke();
                    }
                    
                    Debug.Log("Get player file success");
                }
                else
                {
                    Debug.Log("Error get player file");
                    OnLoadDataFailed?.Invoke($"{LOAD_DATA_FAILED}\nError:{playerFilesResponse.Error}");
                }
            });
        }
        
        /// <summary>
        /// New game.
        /// </summary>
        /// <param name="playerData"> Data player </param>
        public static void ResetGame(PlayerData playerData)
        { 
            SetPath();
            
            playerData.ResetData();
           
            string json = JsonUtility.ToJson(playerData, true);
            
            File.WriteAllText(savePath, json, Encoding.UTF8);
            
            SetData(true);
        }

        /// <summary>
        /// Set first player data to cloud.
        /// </summary>
        /// <param name="playerData"> Data player </param>
        private static void SetFirstFilePlayer(PlayerData playerData)
        {
            string json = JsonUtility.ToJson(playerData, true);
            
            File.WriteAllText(savePath, json, Encoding.UTF8);
            
            LootLockerSDKManager.UploadPlayerFile(savePath, PURPOSE, true, response =>
            {
                if (response.success)
                {
                    LootLockerSDKManager.GetAllPlayerFiles(playerFilesResponse =>
                    {
                        if (playerFilesResponse.success)
                        {
                            if (playerFilesResponse.items.Length == 0) return;
                            
                            var files = playerFilesResponse.items.Where(x => x.name == SAVE_FILE);

                            foreach (LootLockerPlayerFile lootLockerPlayerFile in files)
                            {
                                saveFile = lootLockerPlayerFile;
                            }
                            
                            Debug.Log("Get player file success");
                            OnLoadData?.Invoke();
                        }
                        else
                        {
                            Debug.Log("Error get player file");
                            OnLoadDataFailed?.Invoke($"{LOAD_DATA_FAILED}\nError:{response.Error}");
                        }
                    });
                    
                    Debug.Log("Successfully uploaded First player file, url: " + response.url);
                }
                else
                {
                    Debug.Log("Error uploading First player file");
                    OnLoadDataFailed?.Invoke($"{LOAD_DATA_FAILED}\nError:{response.Error}");
                }
            });
        }

        /// <summary>
        /// Get player data from cloud.
        /// </summary>
        /// <param name="playerData"> Data player </param>
        /// <returns></returns>
        public static IEnumerator GetData(PlayerData playerData)
        {
            UnityWebRequest www = UnityWebRequest.Get(saveFile.url);
            yield return www.SendWebRequest();
            
            File.WriteAllText(savePath, www.downloadHandler.text);

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif

            string json = File.ReadAllText(savePath, Encoding.UTF8);
            PlayerData tempPlayerData = JsonUtility.FromJson<PlayerData>(json);
            playerData.GetData(tempPlayerData);

            LoadDataFinish = true;
        }

        /// <summary>
        /// Set player data to cloud.
        /// </summary>
        /// <param name="isResetData"> True = Reset data / False = Not Reset data </param>
        private static void SetData(bool isResetData)
        {
            LootLockerSDKManager.GetAllPlayerFiles(playerFilesResponse =>
            {
                if (playerFilesResponse.success)
                {
                    var files = playerFilesResponse.items.Where(x => x.name == SAVE_FILE);

                    foreach (LootLockerPlayerFile lootLockerPlayerFile in files)
                    {
                        LootLockerSDKManager.DeletePlayerFile(lootLockerPlayerFile.id, response => 
                        {
                            if (response.success)
                            {
                                Debug.Log("Successfully deleted player file with id: " + lootLockerPlayerFile.id);
                            }
                            else
                            {
                                Debug.Log("Error deleting player file");
                            }
                        });
                    }
                    
                    LootLockerSDKManager.UploadPlayerFile(savePath, PURPOSE, true, response =>
                    {
                        if (response.success)
                        {
                            Debug.Log("Successfully uploaded player file, url: " + response.url);
                            if (!isResetData)
                            {
                                OnSavingSuccess?.Invoke(SAVING_SUCCESS);
                            }
                            else
                            {
                                OnResetDataSuccess?.Invoke(RESET_DATA_SUCCESS);
                            }
                        }
                        else
                        {
                            Debug.Log("Error uploading player file");
                            if (!isResetData)
                            {
                                OnSavingFailed?.Invoke($"{SAVING_FAILED}\nError:{response.Error}");
                            }
                            else
                            {
                                OnResetDataFailed?.Invoke($"{RESET_DATA_FAILED}\nError:{response.Error}");
                            }
                        }
                    });

                    Debug.Log("Get player file success");
                }
                else
                {
                    Debug.Log("Error retrieving player storage");

                    if (!isResetData)
                    {
                        OnSavingFailed?.Invoke($"{SAVING_FAILED}\nError:{playerFilesResponse.Error}");
                    }
                    else
                    {
                        OnResetDataFailed?.Invoke($"{RESET_DATA_FAILED}\nError:{playerFilesResponse.Error}");
                    }
                }
            });
        }
    }
}