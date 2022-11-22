using System;
using System.Collections.Generic;
using Kamaria.Item;
using UnityEngine;

namespace Kamaria.DropItem
{
    [Serializable]
    public sealed class ItemDrop
    {
        [SerializeField] private TiersItem tier;
        [SerializeField] private List<BaseItemSO> items = new List<BaseItemSO>();

        public TiersItem Tier => tier;
        public List<BaseItemSO> Items => items;
    }
}