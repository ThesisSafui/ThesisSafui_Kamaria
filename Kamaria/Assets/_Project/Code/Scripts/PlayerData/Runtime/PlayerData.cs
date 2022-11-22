using System;
using Kamaria.Item;
using Kamaria.Player.Data.DeviceCompartmen;
using Kamaria.Player.Data.Inventory;
using Kamaria.Player.Data.Quest;
using UnityEngine;

namespace Kamaria.Player.Data
{
    public enum PlayerGenders
    {
        None,
        Male,
        Female,
        Lgbt
    }

    [Serializable]
    public sealed class PlayerData : IPlayerData
    {
        public AuthenticationData Authentication = new AuthenticationData();
        public StatusData Status = new StatusData();
        public DeviceCompartmentData DeviceCompartment = new DeviceCompartmentData();
        public InventoryData Inventory;
        public InventoryGeneral VaultGeneral = new InventoryGeneral();
        public PlayerQuest Quest;
        public bool IsFirstTime = true;
        
        public void GetData(PlayerData playerData)
        {
            Authentication.GetData(playerData);
            Status.GetData(playerData);
            DeviceCompartment.GetData(playerData);
            Inventory.GetData(playerData);
            VaultGeneral.GetData(playerData);
            Quest.GetData(playerData);
            IsFirstTime = playerData.IsFirstTime;
        }

        public void Initialized()
        {
            Authentication.Initialized();
            Status.Initialized();
            DeviceCompartment.Initialized();
            Inventory.Initialized();
            VaultGeneral.Initialized();
            Quest.Initialized();
            IsFirstTime = true;
        }
        
        public void ResetData()
        {
            Status.Initialized();
            DeviceCompartment.Initialized();
            Inventory.Initialized();
            VaultGeneral.Initialized();
            Quest.Initialized();
            IsFirstTime = true;
        }
        
        /// <summary>
        /// Increase status player from equipping items.
        /// </summary>
        /// <param name="item"> Item equipping. </param>
        /// <param name="playerData"> Player info </param>
        public void IncreaseStatus(BaseItem item, PlayerDataSO playerData)
        {
            if (item.Types != ItemTypes.Weapon) return;
            
            Debug.Log($"ADD ATK{item.Atk[item.LevelIndex]}");
            playerData.Info.Status.MaxHealth += item.MaxHealth[item.LevelIndex];
            playerData.Info.Status.Atk += item.Atk[item.LevelIndex];
            playerData.Info.Status.CritRate += item.CritRate[item.LevelIndex];
            playerData.Info.Status.ReductionDamage += item.ReductionDamage[item.LevelIndex];
            playerData.Info.Status.SpeedAttack += item.SpeedAttack[item.LevelIndex];
            playerData.Info.Status.AttackRange += item.AttackRange[item.LevelIndex];
            playerData.Info.Status.MaxCombo = item.MaxCombo[item.LevelIndex];

            playerData.CharacterControllerData.DashAcceleration += item.IncreaseDashAcceleration[item.LevelIndex];
        }

        /// <summary>
        /// Decrease status player from removed items.
        /// </summary>
        /// <param name="item"> Item removed </param>
        /// <param name="playerData"> Player info </param>
        public void DecreaseStatus(BaseItem item, PlayerDataSO playerData)
        {
            if (item.Types != ItemTypes.Weapon) return;
            
            Debug.Log($"REMOVE ATK{item.Atk[item.LevelIndex]}");
            /*playerData.Info.Status.MaxHealth -= item.MaxHealth[item.LevelIndex];
            playerData.Info.Status.Atk -= item.Atk[item.LevelIndex];
            playerData.Info.Status.CritRate -= item.CritRate[item.LevelIndex];
            playerData.Info.Status.ReductionDamage -= item.ReductionDamage[item.LevelIndex];
            playerData.Info.Status.SpeedAttack -= item.SpeedAttack[item.LevelIndex];
            playerData.Info.Status.AttackRange -= item.AttackRange[item.LevelIndex];
            playerData.Info.Status.MaxCombo -= item.MaxCombo[item.LevelIndex];

            playerData.CharacterControllerData.DashAcceleration -= item.IncreaseDashAcceleration[item.LevelIndex];*/

            playerData.Info.Status.MaxHealth =
                ClampStatus(playerData.Info.Status.MaxHealth, item.MaxHealth[item.LevelIndex]);
            
            playerData.Info.Status.Atk =
                ClampStatus(playerData.Info.Status.Atk, item.Atk[item.LevelIndex]);

            playerData.Info.Status.CritRate =
                ClampStatus(playerData.Info.Status.CritRate, item.CritRate[item.LevelIndex]);

            playerData.Info.Status.ReductionDamage =
                ClampStatus(playerData.Info.Status.ReductionDamage, item.ReductionDamage[item.LevelIndex]);

            playerData.Info.Status.SpeedAttack =
                ClampStatus(playerData.Info.Status.SpeedAttack, item.SpeedAttack[item.LevelIndex]);

            playerData.Info.Status.AttackRange =
                ClampStatus(playerData.Info.Status.AttackRange, item.AttackRange[item.LevelIndex]);

            playerData.Info.Status.MaxCombo =
                ClampStatus(playerData.Info.Status.MaxCombo, item.MaxCombo[item.LevelIndex]);

            playerData.CharacterControllerData.DashAcceleration =
                ClampStatus(playerData.CharacterControllerData.DashAcceleration,
                    item.IncreaseDashAcceleration[item.LevelIndex]);
        }
        
        private int ClampStatus(int main,int secondary)
        {
            return Mathf.Clamp(main - secondary, 0, Int32.MaxValue);
        }
        
        private float ClampStatus(float main,float secondary)
        {
            return Mathf.Clamp(main - secondary, 0, Int32.MaxValue);
        }
    }
}