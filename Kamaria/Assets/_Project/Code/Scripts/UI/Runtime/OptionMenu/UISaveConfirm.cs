using Kamaria.Player.Data;
using Kamaria.SaveLoad;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI.OptionMenu
{
    public sealed class UISaveConfirm : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private Button yesButton;
        [SerializeField] private Button noButton;
        [SerializeField] private SaveLoadManager saveLoadManager;

        private void OnEnable()
        {
            yesButton.onClick.AddListener(Save);
            noButton.onClick.AddListener((() => gameObject.SetActive(false)));
            
            Time.timeScale = 0;
            playerData.IsUsingUI = true;
        }

        private void OnDisable()
        {
            yesButton.onClick.RemoveListener(Save);
            noButton.onClick.RemoveAllListeners();
            
            Time.timeScale = 1;
            playerData.IsUsingUI = false;
        }
        
        private void Save()
        {
            saveLoadManager.SaveData(false);
        }
    }
}