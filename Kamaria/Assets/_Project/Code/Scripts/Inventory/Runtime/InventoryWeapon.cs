using System;
using System.Collections.Generic;
using Kamaria.Item;
using UnityEngine;

namespace Kamaria.Player.Data.Inventory
{
    [Serializable]
    public sealed class InventoryWeapon : IPlayerData
    {
        [SerializeField] private List<BaseItemSO> baseWeapon = new List<BaseItemSO>();

        public List<BaseItemSO> BaseWeaponSO => baseWeapon;
        
        public List<BaseItem> Weapon = new List<BaseItem>();

        public void Initialized()
        {
            /*Weapon.Clear();

            for (int i = 0; i < baseWeapon.Count; i++)
            {
                Weapon.Add(baseWeapon[i].Info);
                Weapon[i].GetInfo(baseWeapon[i].Info);
            }*/
            
            Weapon.Clear();
            
            for (int i = 0; i < BaseWeaponSO.Count; i++)
            {
                //Items.Add(baseItemsGeneral[i].Info);
                Weapon.Add(new BaseItem());
                Weapon[i].TempGetInfo(BaseWeaponSO[i].Info);
            }
        }

        public void GetData(PlayerData playerData)
        {
            for (int i = 0; i < Weapon.Count; i++)
            {
                Weapon[i].GetInfo(playerData.Inventory.InventoryWeapon.Weapon[i]);
            }
        }
    }
}