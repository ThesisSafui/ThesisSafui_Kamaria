using System.Collections;
using Kamaria.Player.Data;
using Kamaria.Utilities.SaveLoad;
using UnityEngine;

namespace Kamaria.UI
{
    public sealed class FirstTime : MonoBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        [SerializeField] private RectTransform uiFirstTime;
        [SerializeField] private SaveLoadDataSO saveLoadDataSO;

        private void Awake()
        {
            if (playerData.Info.IsFirstTime)
            {
                StartCoroutine(WaitShow());
            }
        }

        private IEnumerator WaitShow()
        {
            yield return new WaitUntil((() => saveLoadDataSO.LoadDataFinish));
            yield return new WaitForSeconds(2);
            uiFirstTime.gameObject.SetActive(true);

        }
    }
}