using System;
using System.Collections.Generic;
using UnityEngine;

namespace Kamaria.DropItem
{
    [Serializable]
    public sealed class ItemDropRate
    {
        [SerializeField] private List<ItemRate> tiersDropRate = new List<ItemRate>();
        [Space]
        [SerializeField] private int increaseLimitCommonDropRate;
        [SerializeField] private int increaseLimitUnCommonDropRate;
        [SerializeField] private int increaseLimitRareDropRate;
        [Space]
        [SerializeField] private int decreaseLimitCommonDropRate;
        [SerializeField] private int decreaseLimitUnCommonDropRate;
        [SerializeField] private int decreaseLimitRareDropRate;

        [HideInInspector] public List<ItemRate> TiersDropRate = new List<ItemRate>();

        public int MaxRate { get; set; }

        public void Initialized()
        {
            MaxRate = 0;
            TiersDropRate.Clear();
            
            for (int i = 0; i < tiersDropRate.Count; i++)
            {
                MaxRate += tiersDropRate[i].Rate;
                TiersDropRate.Add(new ItemRate());
                TiersDropRate[i].GetData(tiersDropRate[i]);
            }
        }
        
        /// <summary>
        /// Decrease common drop rate.
        /// Increase uncommon drop rate and rare drop rate.
        /// </summary>
        /// <param name="count"> Even number </param>
        public void DecreaseCommonDropRate(int count)
        {
            ItemRate itemRateCommonDefault = tiersDropRate.Find(x => x.Tier == TiersItem.Common);
            ItemRate itemRateCommon = TiersDropRate.Find(x => x.Tier == TiersItem.Common);
            
            int limit = itemRateCommonDefault.Rate - decreaseLimitCommonDropRate;
            Debug.Log($"Limit {limit}");
            if (itemRateCommon.Rate <= limit)
            {
                itemRateCommon.Rate = limit;
                return;
            }
            
            ItemRate itemRateUnCommon = TiersDropRate.Find(x => x.Tier == TiersItem.UnCommon);
            ItemRate itemRateRare = TiersDropRate.Find(x => x.Tier == TiersItem.Rare);
            
            if (count % 2 != 0)
            {
                count = 2;
            }
            
            int temp = count / 2;
            
            itemRateCommon.Rate -= count;
            itemRateUnCommon.Rate += temp;
            itemRateRare.Rate += temp;
        }
    }
}