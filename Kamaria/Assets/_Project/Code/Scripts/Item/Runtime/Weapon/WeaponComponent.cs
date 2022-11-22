using System;
using UnityEngine;

namespace Kamaria.Item.Weapon
{
    [Serializable]
    public sealed class WeaponComponent
    {
        [Tooltip("Component1 upgrade yet? / True = upgraded : False = not upgraded.")]
        public bool UpgradeComponent1 = false;
        
        [Tooltip("Component2 upgrade yet? / True = upgraded : False = not upgraded.")]
        public bool UpgradeComponent2 = false; 
        
        [Tooltip("Component3 upgrade yet? / True = upgraded : False = not upgraded.")]
        public bool UpgradeComponent3 = false;
        
        [Tooltip("Weapon evolution yet? / True = evolution : False = not evolution.")]
        public bool IsEvolution = false;

        /// <summary>
        /// Item current level full upgraded or not.
        /// Return True if full upgraded : False if not full upgrade.
        /// </summary>
        public bool FullUpgrade => UpgradeComponent1 && UpgradeComponent2 && UpgradeComponent3;
    }
}