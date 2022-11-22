using System;
using Kamaria.Item;
using UnityEngine;

namespace Kamaria.Player.Animation
{
    [Serializable]
    public sealed class SpeedAnimation
    {
        [SerializeField] private WeaponTypes weaponTypes;
        [SerializeField] private float defaultSpeedAnimCombo1;
        [SerializeField] private float defaultSpeedAnimCombo2;
        [SerializeField] private float defaultSpeedAnimCombo3;
        [SerializeField] private float defaultSpeedAnimCombo4;
        [Space]
        [SerializeField] private float speedAnimCombo1;
        [SerializeField] private float speedAnimCombo2;
        [SerializeField] private float speedAnimCombo3;
        [SerializeField] private float speedAnimCombo4;
        
        public WeaponTypes WeaponTypes => weaponTypes;
        public float SpeedAnimCombo1 => speedAnimCombo1;
        public float SpeedAnimCombo2 => speedAnimCombo2;
        public float SpeedAnimCombo3 => speedAnimCombo3;
        public float SpeedAnimCombo4 => speedAnimCombo4;
        public float DefaultSpeedAnimCombo1 => defaultSpeedAnimCombo1;
        public float DefaultSpeedAnimCombo2 => defaultSpeedAnimCombo2;
        public float DefaultSpeedAnimCombo3 => defaultSpeedAnimCombo3;
        public float DefaultSpeedAnimCombo4 => defaultSpeedAnimCombo4;
    }
}