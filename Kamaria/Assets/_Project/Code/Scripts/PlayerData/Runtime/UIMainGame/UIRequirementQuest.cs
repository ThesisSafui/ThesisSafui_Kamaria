using System;
using TMPro;
using UnityEngine;

namespace Kamaria.UI.UIMainGame
{
    [Serializable]
    public sealed class UIRequirementQuest
    {
        [SerializeField] private RectTransform parent;
        [SerializeField] private TextMeshProUGUI requirement;
        [SerializeField] private TextMeshProUGUI countProgress;

        public RectTransform Parent => parent;
        public TextMeshProUGUI Requirement => requirement;
        public TextMeshProUGUI CountProgress => countProgress;
    }
}