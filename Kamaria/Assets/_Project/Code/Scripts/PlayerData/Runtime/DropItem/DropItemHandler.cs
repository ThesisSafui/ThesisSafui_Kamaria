using System;
using System.Collections.Generic;
using System.Linq;
using Kamaria.Item;
using Kamaria.Manager;
using UnityEngine;
using Random = System.Random;

namespace Kamaria.DropItem
{
    public enum TiersItem
    {
        Common, UnCommon, Rare
    }
    
    public sealed class DropItemHandler : MonoBehaviour
    {
        [SerializeField] private bool isBoss;
        [SerializeField] private FarmingManagerSO farmingManager;
        [SerializeField] private List<ItemDrop> itemsDrops = new List<ItemDrop>();
        [SerializeField] private ItemDropRate dropRate;
        [Space] 
        [SerializeField] [Range(0, 10)] private int dropRatePercent;
        [SerializeField] private int countDropItem;

        public ItemDropRate DropRate => dropRate;
        public int CountDropItem => countDropItem;
        
        private Random random = new Random();
        private List<ItemRate> rates = new List<ItemRate>();
        private int maxDropRatePercent = 10;

        private void OnEnable()
        {
            dropRate.Initialized();
            
            if (isBoss)
            {
                for (int i = 0; i < farmingManager.CountIncreaseDropRate; i++)
                {
                    dropRate.DecreaseCommonDropRate(2);
                }
            }

            #region DEBUG

            ItemRate itemRateCommon = dropRate.TiersDropRate.Find(x => x.Tier == TiersItem.Common);
            ItemRate itemRateUnCommon = dropRate.TiersDropRate.Find(x => x.Tier == TiersItem.UnCommon);
            ItemRate itemRateRare = dropRate.TiersDropRate.Find(x => x.Tier == TiersItem.Rare);
            Debug.Log($"Common {gameObject.layer}   : Rate {itemRateCommon.Rate}");
            Debug.Log($"UnCommon {gameObject.layer}  : Rate {itemRateUnCommon.Rate}");
            Debug.Log($"RateRare {gameObject.layer} : Rate {itemRateRare.Rate}");

            #endregion
        }

        public bool CanDrop()
        {
            int dropPercent = random.Next(maxDropRatePercent);
            Debug.Log(dropPercent);
            return dropPercent < dropRatePercent;
        }

        public void DropItem(out ItemsName itemsName)
        {
            TiersItem tiersItem = TiersItem.Common;
            rates.Clear();
            int tier = random.Next(dropRate.MaxRate);
            var tiersDropRate = dropRate.TiersDropRate.OrderBy(x => x.Rate);
            
            foreach (ItemRate itemRate in tiersDropRate)
            {
                rates.Add(itemRate);
            }

            ItemRate rateLast = rates.LastOrDefault();
            
            if (tier >= rateLast.Rate)
            {
                tiersItem = rateLast.Tier;
            }
            else
            {
                for (int i = 0; i < rates.Count; i++)
                {
                    if (tier < rates[i].Rate)
                    {
                        tiersItem = rates[i].Tier;
                        break;
                    }
                }
            }

            Debug.Log($"tier {tiersItem}");

            var dropItemTier = itemsDrops.Find(x => x.Tier == tiersItem);
            itemsName = dropItemTier.Items[random.Next(dropItemTier.Items.Count)].Info.Name[Index.Start];
           
            Debug.Log($"itemsName {itemsName}");
        }
    }
}