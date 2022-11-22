using System;
using System.Collections.Generic;
using Kamaria.Item;

namespace Kamaria.Player.Data.Inventory
{
    [Serializable]
    public sealed class InventoryKeyStone : IPlayerData
    {
        public List<BaseItem> KeyStones = new List<BaseItem>();
        
        public void Initialized()
        {
            KeyStones.Clear();
        }

        public void GetData(PlayerData playerData)
        {
            KeyStones = playerData.Inventory.InventoryKeyStone.KeyStones;
        }

        public void GetKeyStone(BaseItem keyStone)
        {
            if (keyStone.Types == ItemTypes.KeyStone)
            {
                KeyStones.Add(keyStone);
            }
        }
    }
}