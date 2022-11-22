using System;
using Kamaria.Item;
using Kamaria.Item.CraftingManager;
using Kamaria.Utilities;
using Kamaria.Utilities.GameEvent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI.Crafting
{
    public sealed class UIItemCrafting : MonoBehaviour
    {
        [SerializeField] private GameEventSO eventCrafting;
        [SerializeField] private GameEventSO eventCraftingFail;
        [SerializeField] private GameEventSO eventCraftingInventoryFull;
        [SerializeField] private CraftingManagerSO craftingManager;
        [SerializeField] private Button button;
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private RectTransform panelSpawnItemRequirement;
        [SerializeField] private UICraftingItemRequirement prefabUICraftingItemRequirement;

        public Button Button => button;
        public Image Image => image;
        public TextMeshProUGUI ItemName => itemName;
        public RectTransform PanelSpawnItemRequirement => panelSpawnItemRequirement;
        public UICraftingItemRequirement PrefabUICraftingItemRequirement => prefabUICraftingItemRequirement;
        public ItemsName ItemsName => _itemsName;
        public bool IsCanCrafting => isCanCrafting;

        private ItemsName _itemsName;
        private bool isCanCrafting;
        private bool craftingSucceed;
        private bool isInventoryFull;
        
        public void Init(Sprite image, ItemsName itemName)
        {
            _itemsName = itemName;
            this.image.sprite = image;
            this.itemName.text = itemName.ToString();
        }

        public void CanCrafting(bool canCrafting)
        {
            isCanCrafting = canCrafting;
        }
        
        private void OnEnable()
        {
            button.onClick.AddListener(Clicked);
        }

        private void OnDisable()
        {
            button.onClick.RemoveListener(Clicked);
        }

        private void Clicked()
        {
            SoundHandler.Instance.PlayUI(SoundClip.Sound.Click);
            craftingManager.CraftingItem(_itemsName, out craftingSucceed, out isInventoryFull);

            if (isInventoryFull)
            {
                eventCraftingInventoryFull.TriggerEvent();
                return;
            }
            
            if (craftingSucceed)
            {
                eventCrafting.TriggerEvent();
            }
            else
            {
                eventCraftingFail.TriggerEvent();
            }
        }
    }
}