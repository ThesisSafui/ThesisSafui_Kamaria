using System;
using UnityEngine;

namespace Kamaria.Enemy.AIEnemy
{
    [Serializable]
    public sealed class EnemyStatusData
    {
        [Range(1, 3)] public int Level;
        public int MaxHealth;
        public int CurrentHealth;
        public int MaxGuard;
        public int CurrentGuard;
        public int Atk;
        public float ReductionDamage;
        public float IncreasePercent;

        public void GetStatus(EnemyStatusData statusData)
        {
            Level = statusData.Level;
            MaxHealth = statusData.MaxHealth;
            CurrentHealth = MaxHealth;
            Atk = statusData.Atk;
            ReductionDamage = statusData.ReductionDamage;
            IncreasePercent = statusData.IncreasePercent;
            MaxGuard = statusData.MaxGuard;
            CurrentGuard = MaxGuard;
        }
        
        public void Increase()
        {
            MaxHealth += (int)(MaxHealth * IncreasePercent) / 100;
            CurrentHealth = MaxHealth;
            Atk += (int)(Atk * IncreasePercent) / 100;
            ReductionDamage += (float)(ReductionDamage * IncreasePercent) / 100;
        }
    }
}