using System;
using System.Collections.Generic;
using Kamaria.Item;
using UnityEngine;

namespace Kamaria.Player.Data.Inventory
{
    public enum InventoryTypes
    {
        Vault,General
    }
    
    [Serializable]
    public sealed class InventoryGeneral : IPlayerData
    {
        [SerializeField] private InventoryTypes inventoryTypes;
        [SerializeField] private int stack;
        [SerializeField] private int slotItem;
        [SerializeField] private List<BaseItemSO> baseItemsGeneral = new List<BaseItemSO>();

        public List<BaseItem> Items = new List<BaseItem>();
        
        private int countStack;
        private int countFraction;

        public int Stack => stack;
        public int SlotItem => slotItem;
        public int MaxInventory => stack * slotItem;

        public void Initialized()
        {
            Items.Clear();
            
            for (int i = 0; i < baseItemsGeneral.Count; i++)
            {
                //Items.Add(baseItemsGeneral[i].Info);
                Items.Add(new BaseItem());
                Items[i].TempGetInfo(baseItemsGeneral[i].Info);
            }
        }

        public void GetData(PlayerData playerData)
        {
            switch (inventoryTypes)
            {
                case InventoryTypes.General:
                {
                    for (int i = 0; i < Items.Count; i++)
                    {
                        Items[i].GetInfo(playerData.Inventory.InventoryGeneral.Items[i]);
                    }

                    break;
                }
                case InventoryTypes.Vault:
                    
                    for (int i = 0; i < Items.Count; i++)
                    {
                        Items[i].GetInfo(playerData.VaultGeneral.Items[i]);
                    }
                    
                    break;
            }
        }

        /// <summary>
        /// Add count item.
        /// </summary>
        /// <param name="item"> Add item </param>
        /// <param name="count"> Count? </param>
        /// <param name="isAdd"> Is add? </param>
        /// <param name="playerData"> playerData </param>
        public void Add(BaseItem item ,int count, out bool isAdd, PlayerData playerData)
        {
            if (IsUseSlotItemExceedLimit(playerData, item.Name[Index.Start], count))
            {
                isAdd = false;
                return;
            }
            
            if (item.Count + count <= (int)MaxInventory)
            {
                item.Count += count;
                isAdd = true;
            }
            else
            {
                isAdd = false;
            }
        }
        
        public bool IsUseSlotItemExceedLimit(PlayerData playerData, ItemsName itemsName,int countAdd)
        {
            int result = 0;
            
            var inventory = playerData.Inventory.InventoryGeneral;
            
            for (int i = 0; i < inventory.Items.Count; i++)
            {
                if (inventory.Items[i].Name[Index.Start] == itemsName)
                {
                    inventory.Items[i].ResultStack(inventory.stack, out countStack, out countFraction, countAdd);
                }
                else
                {
                    inventory.Items[i].ResultStack(inventory.stack, out countStack, out countFraction);
                }
                
                if (countFraction != 0)
                {
                    countFraction = 1;
                }

                result += countStack + countFraction;
            }

            return result > slotItem;
        }

        /// <summary>
        /// Remove count item.
        /// </summary>
        /// <param name="item"> Remove item </param>
        /// <param name="count"> Count? </param>
        public void Remove(BaseItem item ,int count)
        {
            if (item.Count - count >= 0)
            {
                item.Count -= count;
            }
        }
    }
}