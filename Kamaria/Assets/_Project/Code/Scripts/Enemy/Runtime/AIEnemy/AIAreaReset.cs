using System.Collections;
using UnityEngine;

namespace Kamaria.Enemy.AIEnemy
{
    public sealed class AIAreaReset : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!other.TryGetComponent(out BaseAI enemy)) return;

            if (!enemy.IsResetState)
            {
                enemy.IsResetState = true;
                enemy.IsFirstDefinitelyAttack = true;
                StartCoroutine(CanAlertDuration(enemy, enemy.CanAlertDuration));
            }
        }

        private IEnumerator CanAlertDuration(BaseAI enemy, float durationTime)
        {
            enemy.CanAlert = false;
            yield return new WaitForSeconds(durationTime);
            enemy.CanAlert = true;
        }
    }
}