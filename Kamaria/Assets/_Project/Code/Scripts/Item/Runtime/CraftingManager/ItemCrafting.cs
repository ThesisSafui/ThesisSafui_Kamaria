using System;
using System.Collections.Generic;
using Kamaria.Item.Weapon;
using UnityEngine;

namespace Kamaria.Item.CraftingManager
{
    [Serializable]
    public sealed class ItemCrafting
    {
        [SerializeField] private ItemsName itemsName;
        [SerializeField] private List<ItemRequirementData> itemsRequirement;

        public ItemsName ItemsName => itemsName;
        public List<ItemRequirementData> ItemsRequirement => itemsRequirement;
    }
}