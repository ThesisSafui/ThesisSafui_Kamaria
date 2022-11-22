using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Kamaria.Item.CraftingManager;
using Kamaria.Player.Data;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI.Crafting
{
    public sealed class UICrafting : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private CraftingManagerSO craftingManager;
        [SerializeField] private Button exitButton;
        [SerializeField] private RectTransform panelSpawnItem;
        [SerializeField] private UIItemCrafting prefabItemCrafting;
        [SerializeField] private RectTransform resultPanel;
        [SerializeField] private RectTransform resultSucceed;
        [SerializeField] private RectTransform resultFail;
        [SerializeField] private RectTransform resultInventoryFull;
        [SerializeField] private float showResultPanelTime;
        
        private void OnEnable()
        {
            exitButton.onClick.AddListener(Exit);
            SetUI();
            CloseResultPanel();
            
            playerData.IsUsingUI = true;
            Time.timeScale = 0;
        }

        private void OnDisable()
        {
            exitButton.onClick.RemoveListener(Exit);
            playerData.IsUsingUI = false;
            Time.timeScale = 1;
        }

        private void Exit()
        {
            gameObject.SetActive(false);
        }

        public void SetUI()
        {
            List<UIItemCrafting> itemsCrafting = new List<UIItemCrafting>();
            
            if (panelSpawnItem.transform.childCount > 0)
            {
                for (int i = 0; i < panelSpawnItem.transform.childCount; i++)
                {
                    Destroy(panelSpawnItem.GetComponentsInChildren<UIItemCrafting>()[i].gameObject);
                }
            }

            for (int i = 0; i < craftingManager.ItemsCrafting.Count; i++)
            {
                List<bool> canCrafting = new List<bool>();

                var itemPlayer = playerData.Info.Inventory.InventoryGeneral.Items.Find(x =>
                    x.Name[Index.Start] == craftingManager.ItemsCrafting[i].ItemsName);

                var itemCrafting = Instantiate(prefabItemCrafting);
                itemCrafting.Init(itemPlayer.Image[Index.Start], itemPlayer.Name[Index.Start]);
                
                for (int j = 0; j < craftingManager.ItemsCrafting[i].ItemsRequirement.Count; j++)
                {
                    var itemPlayerRequirement = playerData.Info.Inventory.InventoryGeneral.Items.Find(x =>
                        x.Name[Index.Start] == craftingManager.ItemsCrafting[i].ItemsRequirement[j].ItemsName);
                    
                    var itemRequirement = Instantiate(itemCrafting.PrefabUICraftingItemRequirement, itemCrafting.PanelSpawnItemRequirement);
                    
                    string itemName = $"{craftingManager.ItemsCrafting[i].ItemsRequirement[j].ItemsName.ToString()}";
                    itemName = string.Concat(itemName.Select(x => Char.IsUpper(x) ? " " + x : x.ToString()))
                        .TrimStart(' ');
                    
                    string itemCount =
                        $"{itemPlayerRequirement.Count}/{craftingManager.ItemsCrafting[i].ItemsRequirement[j].Count}";
                    itemRequirement.Init(itemName, itemCount,
                        itemPlayerRequirement.Count >= craftingManager.ItemsCrafting[i].ItemsRequirement[j].Count);
                    
                    if (itemPlayerRequirement.Count >= craftingManager.ItemsCrafting[i].ItemsRequirement[j].Count)
                    {
                        canCrafting.Add(true);
                    }
                }

                itemCrafting.CanCrafting(craftingManager.ItemsCrafting[i].ItemsRequirement.Count == canCrafting.Count);

                itemsCrafting.Add(itemCrafting);
            }

            itemsCrafting.Sort((x, y) =>
                String.Compare(x.ItemsName.ToString(), y.ItemsName.ToString(), StringComparison.Ordinal));

            itemsCrafting.Sort((x, y) =>
                y.IsCanCrafting.CompareTo(x.IsCanCrafting));
            
            for (int i = 0; i < itemsCrafting.Count; i++)
            {
                itemsCrafting[i].transform.SetParent(panelSpawnItem, false);
            }
        }

        private void CloseResultPanel()
        {
            resultPanel.gameObject.SetActive(false);
            resultSucceed.gameObject.SetActive(false);
            resultFail.gameObject.SetActive(false);
            resultInventoryFull.gameObject.SetActive(false);
        }

        public void ShowResultPanel(bool isSucceed)
        {
            resultPanel.gameObject.SetActive(true);
            
            if (isSucceed)
            {
                resultSucceed.gameObject.SetActive(true);
                resultFail.gameObject.SetActive(false);
            }
            else
            {
                resultSucceed.gameObject.SetActive(false);
                resultFail.gameObject.SetActive(true);
            }

            StartCoroutine(nameof(ResultPanelTime));
        }
        
        public void ShowResultInventoryFull()
        {
            resultPanel.gameObject.SetActive(true);

            resultInventoryFull.gameObject.SetActive(true);
            
            resultSucceed.gameObject.SetActive(false);
            resultFail.gameObject.SetActive(false);
            
            StartCoroutine(nameof(ResultPanelTime));
        }

        private IEnumerator ResultPanelTime()
        {
            yield return new WaitForSecondsRealtime(showResultPanelTime);
            CloseResultPanel();
        }
    }
}