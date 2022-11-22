using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kamaria.Item.Weapon
{
    [Serializable]
    public sealed class ItemWeaponRequirementLevel
    {
        [SerializeField] private List<ItemRequirementData> itemsRequirementComponent1;
        [SerializeField] private List<ItemRequirementData> itemsRequirementComponent2;
        [SerializeField] private List<ItemRequirementData> itemsRequirementComponent3;

        public List<ItemRequirementData> ItemsRequirementComponent1 => itemsRequirementComponent1;
        public List<ItemRequirementData> ItemsRequirementComponent2 => itemsRequirementComponent2;
        public List<ItemRequirementData> ItemsRequirementComponent3 => itemsRequirementComponent3;
    }
}