using System;
using System.Collections.Generic;
using Kamaria.Player.Data;
using Kamaria.Player.Data.Inventory;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI
{
    public sealed class UIInventoryAndVault : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private UIInventoryAndVaultSO uiInventoryAndVaultData;
        [SerializeField] private UIItemInventoryAndVaultItem prefabItem;

        [SerializeField] private UIMoveCount moveCountPanel;
        [SerializeField] private Button moveToVaultButton;
        [SerializeField] private Button moveToInventoryButton;
        [SerializeField] private Button refreshButton;
        [SerializeField] private Button exitButton;

        [SerializeField] private List<RectTransform> slotVault;
        [SerializeField] private List<RectTransform> slotInventory;

        private int stackCount;
        private int fraction;
        private List<UIItemInventoryAndVaultItem> itemsInventoryTemp = new List<UIItemInventoryAndVaultItem>();
        private List<UIItemInventoryAndVaultItem> itemsVaultTemp = new List<UIItemInventoryAndVaultItem>();
        
        private void OnEnable()
        {
            Initialized();

            exitButton.onClick.AddListener(Exit);
            moveToVaultButton.onClick.AddListener(MoveItemToVault);
            moveToInventoryButton.onClick.AddListener(MoveItemToInventory);
            refreshButton.onClick.AddListener(Refresh);

            uiInventoryAndVaultData.Clicked += UiInventoryAndVaultDataOnClicked;
            uiInventoryAndVaultData.Confirm += UiInventoryAndVaultDataOnConfirm;
            
            moveToVaultButton.interactable = false;
            moveToInventoryButton.interactable = false;
            moveCountPanel.gameObject.SetActive(false);
            
            playerData.IsUsingUI = true;
            Time.timeScale = 0;
        }

        private void OnDisable()
        {
            exitButton.onClick.RemoveListener(Exit);
            moveToVaultButton.onClick.RemoveListener(MoveItemToVault);
            moveToInventoryButton.onClick.RemoveListener(MoveItemToInventory);
            refreshButton.onClick.RemoveListener(Refresh);
            
            uiInventoryAndVaultData.Clicked -= UiInventoryAndVaultDataOnClicked;
            uiInventoryAndVaultData.Confirm -= UiInventoryAndVaultDataOnConfirm;
            
            playerData.IsUsingUI = false;
            Time.timeScale = 1;
        }

        private void UiInventoryAndVaultDataOnClicked(UIItemInventoryAndVaultItem callback)
        {
            moveToVaultButton.interactable = false;
            moveToInventoryButton.interactable = false;
            
            switch (callback.SelectTypes)
            {
                case SelectTypes.Inventory:
                    moveToVaultButton.interactable = true;
                    break;
                case SelectTypes.Vault:
                    moveToInventoryButton.interactable = true;
                    break;
            }

            callback.OutLine.enabled = true;
        }
        
        private void UiInventoryAndVaultDataOnConfirm()
        {
            Refresh();
        }

        private void MoveItemToVault()
        {
            moveCountPanel.gameObject.SetActive(true);
            moveCountPanel.Init(uiInventoryAndVaultData.ItemSelect.BaseItem,
                playerData.Info.VaultGeneral.Items.Find
                    (x => x.Name[Index.Start] == uiInventoryAndVaultData.ItemSelect.BaseItem.Name[Index.Start]));
        }

        private void MoveItemToInventory()
        {
            moveCountPanel.gameObject.SetActive(true);
            moveCountPanel.Init(uiInventoryAndVaultData.ItemSelect.BaseItem,
                playerData.Info.Inventory.InventoryGeneral.Items.Find
                    (x => x.Name[Index.Start] == uiInventoryAndVaultData.ItemSelect.BaseItem.Name[Index.Start]));
        }

        private void Refresh()
        {
            Initialized();
            uiInventoryAndVaultData.ItemSelect = null;
            moveToVaultButton.interactable = false;
            moveToInventoryButton.interactable = false;
        }

        private void Initialized()
        {
            Init(slotInventory, playerData.Info.Inventory.InventoryGeneral, itemsInventoryTemp, SelectTypes.Inventory);
            Init(slotVault, playerData.Info.VaultGeneral, itemsVaultTemp, SelectTypes.Vault);
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