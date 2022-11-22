using System;
using Kamaria.Item;
using Kamaria.Player.Data;
using Kamaria.Utilities.PoolingPattern;
using Kamaria.VFX_ALL;
using UnityEngine;

namespace Kamaria.Player.Controller
{
    public sealed class KnuckleMiniAoe : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;

        private BaseItem weaponKnuckle = new BaseItem();

        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out IDamageable target)) return;

            weaponKnuckle =
                playerData.Info.Inventory.InventoryWeapon.Weapon.Find(x => x.WeaponType == WeaponTypes.Knuckle);

            if (!weaponKnuckle.WeaponComponentLevel[1].UpgradeComponent3) return;

            PoolAoe(other.gameObject.transform.position);
        }
        
        private void PoolAoe(Vector3 spawnKnuckleAoe)
        {
            PoolManager.PoolObjectType poolObj = weaponKnuckle.WeaponComponentLevel[2].UpgradeComponent3 
                ? PoolManager.PoolObjectType.KnuckleAoeLV2 : PoolManager.PoolObjectType.KnuckleAoeLV1;
            
            var knuckleAoe = PoolManager.Instance.GetPooledObject(poolObj);
            
            if (knuckleAoe)
            {
                knuckleAoe.transform.position = spawnKnuckleAoe;
                knuckleAoe.transform.rotation = Quaternion.identity;
                knuckleAoe.SetActive(true);
                knuckleAoe.GetComponent<VfxShockWave>().Init(spawnKnuckleAoe);
            }
        }
    }
}