using System.Collections.Generic;
using Kamaria.Item;
using Kamaria.Player.Data;
using Kamaria.Player.Data.Quest;
using Kamaria.UI.NotifiedGetItem;
using Kamaria.Utilities.PoolingPattern;
using Kamaria.VFX_ALL;
using UnityEngine;

namespace Kamaria.Manager
{
    public sealed class UIManager : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private List<GameObject> uisLinkEsc;
        [SerializeField] private GameObject parentNotifiedGetItem;
        [Header("Use With Lobby Only")]
        [SerializeField] private List<VfxShowWeapon> vfxShowWeapons = new List<VfxShowWeapon>();
        private VfxShowWeapon currentVfxShowWeapon;
        
        private void Awake()
        {
            playerData.UIsLinkEsc = uisLinkEsc;
        }
        
        public void NotifiedGetItem(ItemsName itemsName)
        {
            PoolManager.PoolObjectType poolObj = PoolManager.PoolObjectType.NotifiedGetItem;
            
            var notified = PoolManager.Instance.GetPooledObject(poolObj);
            
            if (notified)
            {
                notified.transform.SetParent(parentNotifiedGetItem.transform, false);
                notified.GetComponent<UINotifiedGetItem>().Init(itemsName);
                notified.SetActive(true);
            }
        }

        public void NotifiedGetItem(ItemsQuest itemsName)
        {
            PoolManager.PoolObjectType poolObj = PoolManager.PoolObjectType.NotifiedGetItem;
            
            var notified = PoolManager.Instance.GetPooledObject(poolObj);
            
            if (notified)
            {
                notified.transform.SetParent(parentNotifiedGetItem.transform, false);
                notified.GetComponent<UINotifiedGetItem>().Init(itemsName);
                notified.SetActive(true);
            }
        }

        public void NotifiedInventoryFull()
        {
            PoolManager.PoolObjectType poolObj = PoolManager.PoolObjectType.NotifiedInventoryFull;
            
            var notified = PoolManager.Instance.GetPooledObject(poolObj);
            
            if (notified)
            {
                notified.GetComponent<UINotifiedInventoryFull>().Init();
                notified.SetActive(true);
            }
        }
        
        public void SetVfxShowWeapon(BaseItem item)
        {
            StopVfxShowWeapon();
            
            if (item.WeaponType == WeaponTypes.Sword)
            {
                currentVfxShowWeapon = item.Level switch
                {
                    1 => vfxShowWeapons.Find(x => x.VfxWeapon == VfxWeapon.SwordLv1),
                    2 => vfxShowWeapons.Find(x => x.VfxWeapon == VfxWeapon.SwordLv2),
                    3 => vfxShowWeapons.Find(x => x.VfxWeapon == VfxWeapon.SwordLv3),
                    _ => currentVfxShowWeapon
                };
            }
            else if (item.WeaponType == WeaponTypes.Gun)
            {
                currentVfxShowWeapon = item.Level switch
                {
                    1 => vfxShowWeapons.Find(x => x.VfxWeapon == VfxWeapon.GunLv1),
                    2 => vfxShowWeapons.Find(x => x.VfxWeapon == VfxWeapon.GunLv2),
                    3 => vfxShowWeapons.Find(x => x.VfxWeapon == VfxWeapon.GunLv3),
                    _ => currentVfxShowWeapon
                };
            }
            else if (item.WeaponType == WeaponTypes.Knuckle)
            {
                currentVfxShowWeapon = item.Level switch
                {
                    1 => vfxShowWeapons.Find(x => x.VfxWeapon == VfxWeapon.KnuckleLv1),
                    2 => vfxShowWeapons.Find(x => x.VfxWeapon == VfxWeapon.KnuckleLv2),
                    3 => vfxShowWeapons.Find(x => x.VfxWeapon == VfxWeapon.KnuckleLv3),
                    _ => currentVfxShowWeapon
                };
            }

            currentVfxShowWeapon.gameObject.SetActive(true);
        }

        public void StopVfxShowWeapon()
        {
            for (int i = 0; i < vfxShowWeapons.Count; i++)
            {
                vfxShowWeapons[i].gameObject.SetActive(false);
            }
        }
    }
}