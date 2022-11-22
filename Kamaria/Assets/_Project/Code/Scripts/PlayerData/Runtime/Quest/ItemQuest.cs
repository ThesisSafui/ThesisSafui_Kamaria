using UnityEngine;

namespace Kamaria.Player.Data.Quest
{
    public enum ItemsQuest
    {
        WindStone, Map, ShipGPU, Artifact
    }
    
    public sealed class ItemQuest : MonoBehaviour
    {
        [SerializeField] private ItemsQuest item;
        [SerializeField] private Sprite image;

        public ItemsQuest Item => item;
        public Sprite Image => image;
    }
}