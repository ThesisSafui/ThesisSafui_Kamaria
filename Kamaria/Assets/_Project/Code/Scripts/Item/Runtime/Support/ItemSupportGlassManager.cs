using Kamaria.Item.Weapon;
using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Item.Support
{
    public sealed class ItemSupportGlassManager : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        
        public void UpLevel()
        {
            playerData.Info.DeviceCompartment.SlotGlassTech.Info.UpLevel();
        }
    }
}