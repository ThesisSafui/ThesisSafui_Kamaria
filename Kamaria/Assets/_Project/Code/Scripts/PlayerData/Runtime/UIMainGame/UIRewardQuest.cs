using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kamaria.UI.UIMainGame
{
    [Serializable]
    public sealed class UIRewardQuest
    {
        [SerializeField] private RectTransform parent;
        [SerializeField] private Image reward;
        [SerializeField] private TextMeshProUGUI count;

        public RectTransform Parent => parent;
        public Image Reward => reward;
        public TextMeshProUGUI Count => count;
    }
}