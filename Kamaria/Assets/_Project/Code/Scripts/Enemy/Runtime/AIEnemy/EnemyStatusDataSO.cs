using UnityEngine;

namespace Kamaria.Enemy.AIEnemy
{
    [CreateAssetMenu(fileName = "New EnemyStatusData", menuName = "ThesisSafui/Data/EnemyStatus")]
    public class EnemyStatusDataSO : ScriptableObject
    {
        [SerializeField] private EnemyStatusData[] statusLevel = new EnemyStatusData[3];

        public EnemyStatusData[] StatusLevel => statusLevel;
    }
}