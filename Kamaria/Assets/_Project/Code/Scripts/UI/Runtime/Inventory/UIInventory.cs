using System;
using System.Collections.Generic;
using Kamaria.Player.Data;
using Kamaria.Player.Data.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI.Inventory
{
    public sealed class UIInventory : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private UIInventoryAndVaultSO uiInventoryAndVaultData;
        [SerializeField] private UIItemInventoryAndVaultItem prefabItem;

        [SerializeField] private UIInventoryDelete deleteCountPanel;
        [SerializeField] private Button deleteButton;
        [SerializeField] private Button refreshButton;
        [SerializeField] private Button exitButton;

        [SerializeField] private List<RectTransform> slotInventory = new List<RectTransform>();

        private int stackCount;
        private int fraction;
        private List<UIItemInventoryAndVaultItem> itemsInventoryTemp = new List<UIItemInventoryAndVaultItem>();

        private void OnEnable()
        {
            Initialized();
            
            exitButton.onClick.AddListener(Exit);
            deleteButton.onClick.AddListener(MoveItemToInventory);
            refreshButton.onClick.AddListener(Refresh);

            uiInventoryAndVaultData.Clicked += UiInventoryAndVaultDataOnClicked;
            uiInventoryAndVaultData.Confirm += UiInventoryAndVaultDataOnConfirm;
            
            deleteButton.interactable = false;
            deleteCountPanel.gameObject.SetActive(false);
            
            playerData.IsUsingUI = true;
            Time.timeScale = 0;
        }

        private void OnDisable()
        {
            exitButton.onClick.RemoveListener(Exit);
            deleteButton.onClick.RemoveListener(MoveItemToInventory);
            refreshButton.onClick.RemoveListener(Refresh);

            uiInventoryAndVaultData.Clicked -= UiInventoryAndVaultDataOnClicked;
            uiInventoryAndVaultData.Confirm -= UiInventoryAndVaultDataOnConfirm;
            
            playerData.IsUsingUI = false;
            Time.timeScale = 1;
        }
        
        private void UiInventoryAndVaultDataOnClicked(UIItemInventoryAndVaultItem callback)
        {
            deleteButton.interactable = true;
            callback.OutLine.enabled = true;
        }
        
        private void UiInventoryAndVaultDataOnConfirm()
        {
            Refresh();
        }

        private void MoveItemToInventory()
        {
            deleteCountPanel.gameObject.SetActive(true);
            deleteCountPanel.Init(uiInventoryAndVaultData.ItemSelect.BaseItem);
        }

        private void Refresh()
        {
            Initialized();
            uiInventoryAndVaultData.ItemSelect = null;
            deleteButton.interactable = false;
        }

        private void Initialized()
        {
            Init(slotInventory, playerData.Info.Inventory.InventoryGeneral, itemsInventoryTemp, SelectTypes.Inventory);
        }

        private void Init(List<RectTransform> slot, InventoryGeneral inventory, List<UIItemInventoryAndVaultItem> temps, SelectTypes selectTypes)
        {
            int countItem = 0;
            for (int i = 0; i < inventory.Items.Count; i++)
            {
                countItem += inventory.Items[i].Count;
            }
            if (countItem > inventory.MaxInventory) return;
            
            temps.Clear();

            for (int i = 0; i < slot.Count; i++)
            {
                if (slot[i].transform.childCount > 0)
                {
                    for (int j = 0; j < slot[i].transform.childCount; j++)
                    {
                        Destroy(slot[i].GetComponentsInChildren<UIItemInventoryAndVaultItem>()[j].gameObject);
                    }
                }
            }
            
            for (int i = 0; i < inventory.Items.Count; i++)
            {
                bool haveFraction = false;
                
                if (inventory.Items[i].Count <= 0) continue;

                inventory.Items[i].ResultStack(inventory.Stack, out stackCount, out fraction);

                if (fraction > 0)
                {
                    haveFraction = true;
                    stackCount++;
                }
                
                for (int j = 0; j < stackCount; j++)
                {
                    if (j == stackCount - 1 && haveFraction)
                    {
                        var item = Instantiate(prefabItem);
                        item.Init(inventory.Items[i].Image[inventory.Items[i].LevelIndex], fraction, selectTypes,
                            inventory.Items[i]);
                        temps.Add(item);
                    }
                    else
                    {
                        var item = Instantiate(prefabItem);
                        item.Init(inventory.Items[i].Image[inventory.Items[i].LevelIndex], inventory.Stack, selectTypes,
                            inventory.Items[i]);
                        temps.Add(item);
                    }
                }
            }
            
            for (int i = 0; i < temps.Count; i++)
            {
               temps[i].transform.SetParent(slot[i], false);
            }
        }
        
        private void Exit()
        {
            gameObject.SetActive(false);
        }
    }
}