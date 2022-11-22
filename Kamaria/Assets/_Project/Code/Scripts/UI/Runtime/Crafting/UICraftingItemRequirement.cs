using TMPro;
using UnityEngine;

namespace Kamaria.UI.Crafting
{
    public sealed class UICraftingItemRequirement : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI count;
        [SerializeField] private Color colorCanCrafting;
        [SerializeField] private Color colorCanNotCrafting;
        
        public TextMeshProUGUI ItemName => itemName;
        public TextMeshProUGUI Count => count;

        public void Init(string itemName, string count, bool isCrafting)
        {
            this.itemName.text = itemName;
            this.count.text = count;

            this.count.color = isCrafting ? colorCanCrafting : colorCanNotCrafting;
        }
    }
}