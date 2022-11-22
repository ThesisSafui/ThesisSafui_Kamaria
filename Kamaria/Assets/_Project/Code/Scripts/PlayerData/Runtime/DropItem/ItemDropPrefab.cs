using System.Collections.Generic;
using Kamaria.Item;
using Kamaria.ObjInteract;
using TMPro;
using UnityEngine;

namespace Kamaria.DropItem
{
    public sealed class ItemDropPrefab : MonoBehaviour
    {
        [SerializeField] private List<InteractItemDrop> itemsDropPrefabData;
        [SerializeField] private TextMeshProUGUI itemName;
        
        private Camera camera;
        
        private void OnEnable()
        {
            camera = Camera.main;
        }
        
        private void Update()
        {
            itemName.gameObject.transform.LookAt(
                itemName.gameObject.transform.position + camera.transform.rotation * Vector3.forward,
                camera.transform.rotation * Vector3.up);
        }

        public void Init(ItemsName itemsName)
        {
            itemName.text = itemsName.ToString();
            
            for (int i = 0; i < itemsDropPrefabData.Count; i++)
            {
                itemsDropPrefabData[i].gameObject.SetActive(false);
                
                if (itemsDropPrefabData[i].ItemsName == itemsName)
                {
                    itemsDropPrefabData[i].gameObject.SetActive(true);
                }
            }
        }
    }
}