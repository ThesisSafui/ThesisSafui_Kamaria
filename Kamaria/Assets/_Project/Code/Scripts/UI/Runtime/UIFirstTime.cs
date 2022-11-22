using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.UI
{
    public sealed class UIFirstTime : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;

        private void OnEnable()
        {
            Time.timeScale = 0;
            playerData.IsUsingUI = true;
        }
    }
}