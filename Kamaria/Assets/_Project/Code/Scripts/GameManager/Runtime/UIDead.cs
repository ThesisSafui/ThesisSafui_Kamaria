using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI.UIMainGame
{
    public sealed class UIDead : MonoBehaviour
    {
        [SerializeField] private Button tryAgain;
        [SerializeField] private Button gotoLobby;

        public bool IsTryAgain { get; set; }
        public bool IsChoseFinish { get; set; }

        private void OnEnable()
        {
            IsChoseFinish = false;
            
            tryAgain.onClick.AddListener((() =>
            {
                IsTryAgain = true;
                IsChoseFinish = true;
                this.gameObject.SetActive(false);
            }));
            gotoLobby.onClick.AddListener((() =>
            {
                IsTryAgain = false;
                IsChoseFinish = true;
                this.gameObject.SetActive(false);
            }));
        }

        private void OnDisable()
        {
            tryAgain.onClick.RemoveAllListeners();
            gotoLobby.onClick.RemoveAllListeners();
        }
    }
}