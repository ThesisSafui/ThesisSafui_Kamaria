using UnityEngine;

namespace Kamaria.Item.Weapon
{
    [CreateAssetMenu(fileName = "New ItemWeaponManagerData", menuName = "ThesisSafui/Data/ItemWeaponManager")]
    public sealed class ItemWeaponManagerSO : ScriptableObject
    {
        [SerializeField] private ItemWeaponData swordData;
        
        [Space]
        [SerializeField] private ItemWeaponData knuckleData;
        
        [Space]
        [SerializeField] private ItemWeaponData gunData;

        public ItemWeaponData SwordData => swordData;
        public ItemWeaponData KnuckleData => knuckleData;
        public ItemWeaponData GunData => gunData;
    }
}