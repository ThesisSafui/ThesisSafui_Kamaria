using System.Collections.Generic;
using System.Linq;
using Kamaria.Player.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI.Tips
{
    public sealed class UITips : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private Button rightButton;
        [SerializeField] private Button leftButton;
        [SerializeField] private List<RectTransform> tutorialList = new List<RectTransform>();

        private int currentIndexTutorial;
        
        private void OnEnable()
        {
            Time.timeScale = 0;
            playerData.IsUsingUI = true; 
            Initialized();
            
            rightButton.onClick.AddListener(IncreaseIndex);
            leftButton.onClick.AddListener(DecreaseIndex);
        }

        private void OnDisable()
        {
            Time.timeScale = 1;
            playerData.IsUsingUI = false;
            
            rightButton.onClick.RemoveListener(IncreaseIndex);
            leftButton.onClick.RemoveListener(DecreaseIndex);
        }

        private void Initialized()
        {
            currentIndexTutorial = 0;
            for (int i = 0; i < tutorialList.Count; i++)
            {
                tutorialList[i].gameObject.SetActive(false);
            }

            tutorialList[currentIndexTutorial].gameObject.SetActive(true);
        }
        
        private void IncreaseIndex()
        {
            tutorialList[currentIndexTutorial].gameObject.SetActive(false);
            currentIndexTutorial++;
            if (currentIndexTutorial >= tutorialList.Count)
            {
                currentIndexTutorial = 0;
            }
            tutorialList[currentIndexTutorial].gameObject.SetActive(true);
        }
        
        private void DecreaseIndex()
        {
            tutorialList[currentIndexTutorial].gameObject.SetActive(false);
            currentIndexTutorial--;
            if (currentIndexTutorial < 0)
            {
                currentIndexTutorial = tutorialList.Count - 1;
            }
            tutorialList[currentIndexTutorial].gameObject.SetActive(true);
        }
        
        private void Exit()
        {
            this.gameObject.SetActive(false);
        }
    }
}