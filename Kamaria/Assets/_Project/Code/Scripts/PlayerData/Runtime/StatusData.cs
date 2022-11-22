using System;
using UnityEngine;

namespace Kamaria.Player.Data
{
    [Serializable]
    public sealed class StatusData : IPlayerData
    {
        public int BaseMaxHealth;
        public int BaseAtk;
        
        public int MaxHealth;
        public int CurrentHealth;
        public int Atk;
        public int CritRate;
        public float ReductionDamage;
        public float SpeedAttack;
        public float AttackRange;
        public int MaxCombo;
        
        public void Initialized()
        {
            MaxHealth = BaseMaxHealth;
            CurrentHealth = MaxHealth;
            Atk = BaseAtk;
            CritRate = 0;
            ReductionDamage = 0;
            SpeedAttack = 0;
            AttackRange = 0;
            MaxCombo = 0;
        }

        public void GetData(PlayerData playerData)
        {
            MaxHealth = playerData.Status.MaxHealth;
            CurrentHealth = playerData.Status.CurrentHealth;
            Atk = playerData.Status.Atk;
            CritRate = playerData.Status.CritRate;
            ReductionDamage = playerData.Status.ReductionDamage;
            SpeedAttack = playerData.Status.SpeedAttack;
            AttackRange = playerData.Status.AttackRange;
            MaxCombo = playerData.Status.MaxCombo;
        }
    }
}