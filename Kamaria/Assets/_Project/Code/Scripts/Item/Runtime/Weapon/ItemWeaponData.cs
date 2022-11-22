using System;
using UnityEngine;

namespace Kamaria.Item.Weapon
{
    [Serializable]
    public sealed class ItemWeaponData
    {
        [SerializeField] private float[] unlockSpeedAttack = new float[3];
        [SerializeField] private int[] unlockPowerAttack = new int[3];
        [SerializeField] private float[] unlockDashAcceleration = new float[3];
        [SerializeField] private int[] unlockMaxHealth = new int[3];
        [SerializeField] private int[] unlockReductionDamage = new int[3];
        [SerializeField] private int[] unlockAttackRange = new int[3];
        [SerializeField] private int unlockMaxCombo;

        public int UnlockMaxCombo => unlockMaxCombo;
        public float[] UnlockSpeedAttack => unlockSpeedAttack;
        public int[] UnlockPowerAttack => unlockPowerAttack;
        public float[] UnlockDashAcceleration => unlockDashAcceleration;
        public int[] UnlockMaxHealth => unlockMaxHealth;
        public int[] UnlockReductionDamage => unlockReductionDamage;
        public int[] UnlockAttackRange => unlockAttackRange;
    }
}