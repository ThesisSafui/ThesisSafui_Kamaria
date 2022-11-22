using System;
using System.Collections;
using Kamaria.Player.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI.UIMainGame
{
    public sealed class UIQuestPassword : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private Button confirm;
        [SerializeField] private Button exit;
        [SerializeField] private RectTransform notificationFailed;
        [SerializeField] private TMP_InputField password;
        [SerializeField] private string answer;
        
        private void OnEnable()
        {
            Time.timeScale = 0;
            playerData.IsUsingUI = true;
            notificationFailed.gameObject.SetActive(false);
            
            exit.onClick.AddListener(Close);
            confirm.onClick.AddListener(CheckPassword);
        }

        private void OnDisable()
        {
            Time.timeScale = 1;
            playerData.IsUsingUI = false;
            notificationFailed.gameObject.SetActive(false);
            
            exit.onClick.RemoveListener(Close);
            confirm.onClick.RemoveListener(CheckPassword);
            StopCoroutine(ShowNotificationFailed());
        }

        private void CheckPassword()
        {
            if (String.Equals(password.text, answer, StringComparison.CurrentCultureIgnoreCase))
            {
                playerData.QuestPasswordSucceed = true;
                Close();
            }
            else
            {
                notificationFailed.gameObject.SetActive(true);
                StartCoroutine(ShowNotificationFailed());
            }
        }

        private IEnumerator ShowNotificationFailed()
        {
            yield return new WaitForSecondsRealtime(2);
            notificationFailed.gameObject.SetActive(false);
        }
        
        private void Close()
        {
            this.gameObject.SetActive(false);
        }
    }
}