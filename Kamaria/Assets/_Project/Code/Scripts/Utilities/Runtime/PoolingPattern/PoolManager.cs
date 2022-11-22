using System;
using System.Collections.Generic;
using Kamaria.Utilities.SingletonPattern;
using UnityEngine;

namespace Kamaria.Utilities.PoolingPattern
{
   public class PoolManager : Singleton<PoolManager>
    { 
        public enum PoolObjectType
        {
            GroundSlashLv1, GroundSlashLv2,
            Arrow, TextDamage,
            ItemDrop, NotifiedGetItem, NotifiedInventoryFull, Bomb, IndicatorBomb, ShieldRipplesVFX,
            Meteor, Pistol, Explosion, ShockWaveEnemyMouse, ShockWaveEnemyGolem, 
            ShockWavePlayer, ShockWavePlayerGreenStoneLV1, ShockWavePlayerGreenStoneLV2, ShockWavePlayerRedStone,
            KnuckleAoeLV1, KnuckleAoeLV2, KnuckleSkillRedStone, ExplosionCannon, BulletCannon,
            GunBulletLv1, GunBulletLv2, GunBulletLv3, GrenadeLv1, GrenadeLv2, ExplosionPlayerLv1, ExplosionPlayerLv2,
            BulletGunGreenStoneLv1, BulletGunGreenStoneLv2, ExplosionBulletGunGreenStoneLv1, ExplosionBulletGunGreenStoneLv2
        }

        [Serializable]
        public class PoolObjectSet
        {
            public PoolObjectType poolObjectType;
            public int amountToPool;
            public GameObject objectToPool;
            public bool shouldExpand;
            public List<GameObject> pooledObjects;
        }

        public List<PoolObjectSet> poolObjectSets;
        private Dictionary<PoolObjectType, PoolObjectSet> poolObjectSetDictionary;

        private void Start()
        {
            poolObjectSetDictionary = new Dictionary<PoolObjectType, PoolObjectSet>();
            foreach (PoolObjectSet poolObjectSet in poolObjectSets)
            {
                poolObjectSet.pooledObjects = new List<GameObject>();
                for (int i = 0; i < poolObjectSet.amountToPool; i++)
                {
                    var obj = Instantiate(poolObjectSet.objectToPool);
                    obj.SetActive(false);
                    poolObjectSet.pooledObjects.Add(obj);
                }

                poolObjectSetDictionary.Add(poolObjectSet.poolObjectType, poolObjectSet);
            }
        }

        public GameObject GetPooledObject(PoolObjectType pooledObjectType)
        {
            var poolObjectSet = poolObjectSetDictionary[pooledObjectType];
            var pooledObjects = poolObjectSet.pooledObjects;

            foreach (var pooledObject in pooledObjects)
            {
                if (!pooledObject.activeInHierarchy)
                {
                    return pooledObject;
                }
            }

            if (poolObjectSet.shouldExpand)
            {
                GameObject obj = (GameObject) Instantiate(poolObjectSet.objectToPool);
                obj.SetActive(false);
                pooledObjects.Add(obj);

                return obj;
            }

            return null;
        }
    }
}