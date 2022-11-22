using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI.Armory
{
    public sealed class UIItemRequirement : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private TextMeshProUGUI itemName;
        [SerializeField] private TextMeshProUGUI itemCount;
        [SerializeField] private Color colorCanUpgrade;
        [SerializeField] private Color colorNotUpgrade;

        public Image Image => image;
        public TextMeshProUGUI ItemName => itemName;
        public TextMeshProUGUI ItemCount => itemCount;
        public Color ColorCanUpgrade => colorCanUpgrade;
        public Color ColorNotUpgrade => colorNotUpgrade;

        public void Init(Sprite image,string itemName,string itemCount, bool isCanUpgrade)
        {
            this.image.sprite = image;
            this.itemName.text = itemName;
            this.itemCount.text = itemCount;

            this.itemCount.color = isCanUpgrade ? colorCanUpgrade : colorNotUpgrade;
        }
    }
}