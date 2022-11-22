using UnityEngine;

namespace Kamaria.Manager
{
    [CreateAssetMenu(fileName = "New FarmingManager", menuName = "ThesisSafui/Data/FarmingManager")]
    public sealed class FarmingManagerSO : ScriptableObject
    {
        public Dungeons Dungeons;
        public int CurrentFloor;
        [SerializeField] private int maxFloorStack;
        [SerializeField] private int floorIncrease;
        [SerializeField] private bool useReset;
        [Space]
        [SerializeField] private int maxPotion;
        [SerializeField] private int percentHeal;

        public int CountIncrease => ((CurrentFloor / maxFloorStack) - (floorIncrease / maxFloorStack)) + 1;
        public int CountIncreaseDropRate => (CurrentFloor / maxFloorStack);
        public int MaxPotion => maxPotion;
        public int PercentHeal => percentHeal;
        public int CurrentPotion { get; set; }

        private void OnEnable()
        {
            if (useReset)
            {
                ResetFloor();
            }
        }
        
        public void ResetFloor()
        {
            CurrentFloor = 1;
        }
        
        public bool CanIncrease()
        {
            return CurrentFloor >= floorIncrease;
        }
    }
}