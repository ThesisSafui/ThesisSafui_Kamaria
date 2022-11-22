using System;
using Kamaria.Item;
using UnityEngine;

namespace Kamaria.Player.Data.Slot
{
    [Serializable]
    public sealed class SlotData : IPlayerData
    {
        public BaseItem Info = new BaseItem();
        public bool IsEmpty = true;
        
        private ItemTypes itemTypes;
        
        public SlotData(ItemTypes itemTypes)
        {
            this.itemTypes = itemTypes;
        }

        public void Initialized()
        {
            IsEmpty = true;
            Info = new BaseItem();
        }

        public void GetData(PlayerData playerData)
        {
            switch (itemTypes)
            {
                case ItemTypes.Weapon:
                    Info.GetInfo(playerData.DeviceCompartment.SlotWeapon.Info);
                    break;
                // Use when character have equipment
                /*case ItemTypes.Equipment1:
                    Info.GetInfo(playerData.DeviceCompartment.Equipment1.Info);
                    break;
                case ItemTypes.Equipment2:
                    Info.GetInfo(playerData.DeviceCompartment.Equipment2.Info);
                    break;
                case ItemTypes.Equipment3:
                    Info.GetInfo(playerData.DeviceCompartment.Equipment3.Info);
                    break;*/
                /*case ItemTypes.Support:
                    if (Info.Name[Info.LevelIndex] == ItemsName.GlassTech)
                    {
                        Info.GetInfo(playerData.DeviceCompartment.SlotGlassTech.Info);
                    }
                    else if (Info.Name[Info.LevelIndex] == ItemsName.BangleTech)
                    {
                        Info.GetInfo(playerData.DeviceCompartment.SlotBangleTech.Info);
                    }

                    break;*/
                case ItemTypes.None:
                    Info.GetInfo(new BaseItem());
                    break;
            }
        }

        /// <summary>
        /// Add item on slot.
        /// </summary>
        /// <param name="item"> Item to add. </param>
        /// <param name="playerData"> Player info. </param>
        public void Add(BaseItem item, PlayerDataSO playerData)
        {
            if (item.Types != itemTypes || !IsEmpty) return;

            if (item.IsUsedEquip) return;

            Debug.Log("ADD");
            Info = item;
            item.IsUsedEquip = true;
            playerData.Info.IncreaseStatus(item, playerData);
            IsEmpty = false;

            playerData.Info.GetData(playerData.Info);
        }

        /// <summary>
        /// Remove item on slot.
        /// </summary>
        /// <param name="item"> Item to remove. </param>
        /// <param name="playerData"> Player info. </param>
        public void Remove(BaseItem item, PlayerDataSO playerData)
        {
            if (IsEmpty) return;

            Debug.Log("REMOVE");
            item.IsUsedEquip = false;
            playerData.Info.DecreaseStatus(item, playerData);
            Initialized();

            playerData.Info.GetData(playerData.Info);
        }
    }
}