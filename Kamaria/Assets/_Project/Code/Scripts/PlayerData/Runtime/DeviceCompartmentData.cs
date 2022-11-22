using System;
using Kamaria.Item;
using Kamaria.Player.Data.Slot;
using UnityEngine;

namespace Kamaria.Player.Data.DeviceCompartmen
{
    [Serializable]
    public sealed class DeviceCompartmentData : IPlayerData
    {
        public SlotData SlotWeapon = new SlotData(ItemTypes.Weapon);
        public SlotData SlotGlassTech = new SlotData(ItemTypes.Support);
        public SlotData SlotBangleTech = new SlotData(ItemTypes.Support);
        
        // Use when character have equipment
        /*public SlotData Equipment1= new SlotData(ItemTypes.Equipment1);
        public SlotData Equipment2= new SlotData(ItemTypes.Equipment2);
        public SlotData Equipment3= new SlotData(ItemTypes.Equipment3);*/

        public void Initialized()
        {
            SlotWeapon.Initialized();
            SlotGlassTech.Initialized();
            SlotBangleTech.Initialized();
            
            // Use when character have equipment
            /*Equipment1.Initialized();
            Equipment2.Initialized();
            Equipment3.Initialized();*/
        }

        public void GetData(PlayerData playerData)
        {
            SlotWeapon.GetData(playerData);
            SlotGlassTech.GetData(playerData);
            SlotBangleTech.GetData(playerData);
            
            // Use when character have equipment
            /*Equipment1.GetData(playerData);
            Equipment2.GetData(playerData);
            Equipment3.GetData(playerData);*/
        }
    }
}