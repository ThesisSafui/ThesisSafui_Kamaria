using System;

namespace Kamaria.DropItem
{
    [Serializable]
    public sealed class ItemRate
    {
        public TiersItem Tier;
        public int Rate;
        
        public void GetData(ItemRate itemRate)
        {
            Tier = itemRate.Tier;
            Rate = itemRate.Rate;
        }
    }
}