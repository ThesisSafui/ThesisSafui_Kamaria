using UnityEngine;

namespace Kamaria.Utilities.SaveLoad
{
    [CreateAssetMenu(fileName = "New SaveLoadData", menuName = "ThesisSafui/Data/SaveLoadData")]
    public sealed class SaveLoadDataSO : ScriptableObject
    {
        public bool LoadDataFinish;

        private void OnEnable()
        {
            LoadDataFinish = false;
        }
    }
}