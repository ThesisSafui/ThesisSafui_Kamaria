using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kamaria.Item;
using Kamaria.Player.Data.Quest;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI.NotifiedGetItem
{
    public sealed class UINotifiedGetItem : MonoBehaviour
    {
        [SerializeField] private List<BaseItemSO> itemsData;
        [SerializeField] private List<ItemQuest> itemsQuest;
        [SerializeField] private Image imageItem;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private float animTime;
        
        public void Init(ItemsName itemName)
        {
            var item = itemsData.Find(x => x.Info.Name[Index.Start] == itemName);
            
            imageItem.sprite = item.Info.Image[Index.Start];
            this.itemName.text = itemName.ToString();
        }
        
        public void Init(ItemsQuest itemQuest)
        {
            var item = itemsQuest.Find(x => x.Item == itemQuest);

            imageItem.sprite = item.Image;
            string itemNameText = item.Item.ToString();

            if (item.Item == ItemsQuest.WindStone)
            {
                itemNameText = "Wind Stone";
            }
            
            this.itemName.text = itemNameText;
        }

        private void OnEnable()
        {
            StartCoroutine(Close());
        }
        
        private void OnDisable()
        {
            StopAllCoroutines();
        }

        private IEnumerator Close()
        {
            yield return new WaitForSecondsRealtime(animTime);
            gameObject.SetActive(false);
        }
    }
}