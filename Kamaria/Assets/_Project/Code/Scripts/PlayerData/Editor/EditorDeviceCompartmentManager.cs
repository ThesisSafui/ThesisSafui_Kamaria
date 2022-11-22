using Kamaria.Item;
using UnityEngine;

namespace Kamaria.Player.Data.DeviceCompartmen
{
    public sealed class EditorDeviceCompartmentManager : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;

        public PlayerDataSO PlayerData => playerData;

        public void Equip(BaseItem item)
        {
            if (playerData.Info.DeviceCompartment.SlotWeapon.IsEmpty)
            {
                playerData.Info.DeviceCompartment.SlotWeapon.Add(item, playerData);
            }
        }

        public void Remove(BaseItem item)
        {
            if (!playerData.Info.DeviceCompartment.SlotWeapon.IsEmpty)
            {
                playerData.Info.DeviceCompartment.SlotWeapon.Remove(item, playerData);
            }
        }
    }
}