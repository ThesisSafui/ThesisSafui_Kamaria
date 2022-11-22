using System;
using System.Collections.Generic;
using Kamaria.Item;
using UnityEngine;

namespace Kamaria.Player.Data
{
    [Serializable]
    public sealed class WeaponObject
    {
        [SerializeField] private WeaponTypes weaponType;
        [SerializeField] private List<GameObject> weaponsLv = new List<GameObject>();
        [SerializeField] private List<GameObject> weaponsPower = new List<GameObject>();

        public WeaponTypes WeaponType => weaponType;
        public List<GameObject> WeaponsLv => weaponsLv;
        public List<GameObject> WeaponsPower => weaponsPower;
    }
}