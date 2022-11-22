using DG.Tweening;
using Kamaria.DropItem;
using Kamaria.Item;
using Kamaria.Player.Data;
using Kamaria.Utilities.PoolingPattern;
using UnityEngine;

namespace Kamaria.Manager
{
    public sealed class CreateItemDropManager : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private Vector3 fallPointRang;
        [SerializeField] private float durationToFallPoint;

        private Vector3 fallPoint;
        private float fallPointY;
        private RaycastHit hit;
        
        public void CreateItemDrop(ItemsName itemsName,Transform positionDrop)
        {
            PoolManager.PoolObjectType poolObj = PoolManager.PoolObjectType.ItemDrop;
            
            var itemDrop = PoolManager.Instance.GetPooledObject(poolObj);
            
            if (itemDrop)
            {
                SpawnPosition(positionDrop, itemDrop);
                itemDrop.SetActive(true);
                itemDrop.GetComponent<ItemDropPrefab>().Init(itemsName);
            }
        }

        private void SpawnPosition(Transform positionDrop, GameObject itemDrop)
        {
            itemDrop.transform.position = positionDrop.position;
            if (Physics.Raycast(itemDrop.transform.position,
                    Vector3.down,out hit,playerData.CharacterControllerData.GroundLayers))
            {
                fallPointY = hit.point.y;
            }

            fallPoint = itemDrop.transform.position + new Vector3(
                Random.Range(-fallPointRang.x, fallPointRang.x), 0,
                Random.Range(-fallPointRang.z, fallPointRang.z));

            fallPoint.y = fallPointY;
            
            itemDrop.transform.DOMove(fallPoint, durationToFallPoint);
            
            itemDrop.transform.rotation = Quaternion.identity;
        }
    }
}