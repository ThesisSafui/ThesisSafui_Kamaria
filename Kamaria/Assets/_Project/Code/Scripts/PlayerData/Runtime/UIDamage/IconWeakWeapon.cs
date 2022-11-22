using Kamaria.Item;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UIDamage
{
    public sealed class IconWeakWeapon : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private Sprite iconSword;
        [SerializeField] private Sprite iconKnuckle;
        [SerializeField] private Sprite iconGun;


        private WeaponTypes weaponTypes;
        
        public Image Icon => icon;
        public WeaponTypes WeaponTypes => weaponTypes;
        
        public void Init(WeaponTypes weaponTypes)
        {
            icon.sprite = weaponTypes switch
            {
                WeaponTypes.Gun => iconGun,
                WeaponTypes.Knuckle => iconKnuckle,
                WeaponTypes.Sword => iconSword,
                _ => icon.sprite
            };
            
            this.weaponTypes = weaponTypes;
        }
    }
}