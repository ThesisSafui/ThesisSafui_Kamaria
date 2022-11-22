using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Item.Support
{
    public sealed class ItemSupportBangleManager : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        
        public void UpLevel()
        {
            playerData.Info.DeviceCompartment.SlotBangleTech.Info.UpLevel();
        }
    }
}