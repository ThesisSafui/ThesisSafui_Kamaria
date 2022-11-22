using Kamaria.Player.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI.Map
{
    public sealed class UIMap : MonoBehaviour
    {
        [SerializeField] private GameObject cameraMap;
        [SerializeField] private GameObject player;
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private Button exit;
        [SerializeField] private GameObject iconMark;
        
        private void OnEnable()
        {
            cameraMap.SetActive(true);
            exit.onClick.AddListener(Exit);
            
            MarkPositionPlayer();
            Time.timeScale = 0;
            playerData.IsUsingUI = true;
        }

        private void OnDisable()
        {
            cameraMap.SetActive(false);
            iconMark.SetActive(false);
            Time.timeScale = 1;
            playerData.IsUsingUI = false;
        }

        private void MarkPositionPlayer()
        {
            iconMark.transform.position = player.transform.position;
            iconMark.SetActive(true);
        }
        
        private void Exit()
        {
            this.gameObject.SetActive(false);
        }
    }
}