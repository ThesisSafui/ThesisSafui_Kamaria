using System;

namespace Kamaria.Player.Data.Inventory
{
    [Serializable]
    public sealed class InventoryData : IPlayerData
    {
        public InventoryWeapon InventoryWeapon = new InventoryWeapon();
        public InventoryGeneral InventoryGeneral = new InventoryGeneral();
        public InventoryKeyStone InventoryKeyStone = new InventoryKeyStone();

        public void Initialized()
        {
            InventoryWeapon.Initialized();
            InventoryGeneral.Initialized();
            InventoryKeyStone.Initialized();
        }

        public void GetData(PlayerData playerData)
        {
            InventoryWeapon.GetData(playerData);
            InventoryGeneral.GetData(playerData);
            InventoryKeyStone.GetData(playerData);
        }
    }
}