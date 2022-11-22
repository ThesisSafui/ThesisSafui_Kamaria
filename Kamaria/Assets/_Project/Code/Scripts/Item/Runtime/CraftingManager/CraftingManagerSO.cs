using System;
using System.Collections.Generic;
using Kamaria.Item.Weapon;
using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Item.CraftingManager
{
    [CreateAssetMenu(fileName = "New CraftingManagerSO", menuName = "ThesisSafui/Data/CraftingManager")]
    public sealed class CraftingManagerSO : ScriptableObject
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private List<ItemCrafting> itemsCrafting;

        public List<ItemCrafting> ItemsCrafting => itemsCrafting;

        public void CraftingItem(ItemsName itemsName, out bool succeed, out bool isInventoryFull)
        {
            var itemPlayer =
                playerData.Info.Inventory.InventoryGeneral.Items.Find(x => x.Name[Index.Start] == itemsName);

            var itemCrafting = itemsCrafting.Find(x => x.ItemsName == itemsName);
            
            if (playerData.Info.Inventory.InventoryGeneral.IsUseSlotItemExceedLimit(playerData.Info,itemsName,1))
            {
                isInventoryFull = true;
                succeed = false;
                return;
            }
            
            isInventoryFull = false;
            
            if (IsCanCrafting(itemCrafting.ItemsRequirement))
            {
                itemPlayer.IncreaseCount(1);
                succeed = true;
                return;
            }

            succeed = false;
        }
        
        private bool IsCanCrafting(List<ItemRequirementData> itemCraftingRequirement)
        {
            List<BaseItem> items = new List<BaseItem>();
            List<bool> isCanUpgrade = new List<bool>();

            for (int i = 0; i < itemCraftingRequirement.Count; i++)
            {
                items.Add(playerData.Info.Inventory.InventoryGeneral.Items.Find(x => x.Name[Index.Start] ==
                    itemCraftingRequirement[i].ItemsName));
            }

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].Count >= itemCraftingRequirement[i].Count)
                {
                    isCanUpgrade.Add(true);
                }
            }
            
            if (items.Count == isCanUpgrade.Count)
            {
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].DecreaseCount(itemCraftingRequirement[i].Count);
                }
                
                return true;
            }
            
            return false;
        }
    }
}