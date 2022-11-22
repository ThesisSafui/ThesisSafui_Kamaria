using System;
using Kamaria.Player.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI.Choose
{
    public sealed class UIChoose : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private GameObject ui1;
        [SerializeField] private GameObject ui2;
        [SerializeField] private Button buttonChoose1;
        [SerializeField] private Button buttonChoose2;
        [SerializeField] private Button exit;

        private void OnEnable()
        {
            buttonChoose1.onClick.AddListener(delegate { OpenUI(ui1); });
            buttonChoose2.onClick.AddListener(delegate { OpenUI(ui2); });
            exit.onClick.AddListener(Exit);
            
            playerData.IsUsingUI = true;
            Time.timeScale = 0;
        }

        private void OnDisable()
        {
            buttonChoose1.onClick.RemoveAllListeners();
            buttonChoose2.onClick.RemoveAllListeners();
            exit.onClick.RemoveListener(Exit);
            
            playerData.IsUsingUI = false;
            Time.timeScale = 1;
        }

        private void OpenUI(GameObject ui)
        {
            this.gameObject.SetActive(false);
            ui.SetActive(true);
        }
        
        private void Exit()
        {
            this.gameObject.SetActive(false);
        }
    }
}