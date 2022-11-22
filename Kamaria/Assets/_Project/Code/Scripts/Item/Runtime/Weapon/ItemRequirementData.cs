using System;
using UnityEngine;

namespace Kamaria.Item.Weapon
{
    [Serializable]
    public sealed class ItemRequirementData
    {
        [SerializeField] private ItemsName itemsName;
        [SerializeField] private int count;

        public ItemsName ItemsName => itemsName;
        public int Count => count;
    }
}