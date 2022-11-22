using System;
using UnityEngine;

namespace Kamaria.Enemy.AIEnemy
{
    public sealed class AIAreaAlert : MonoBehaviour
    {
        [SerializeField] private BaseAI ai;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            
            if (!ai.IsCanAlertState && ai.CanAlert)
            {
                ai.IsCanAlertState = true;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;

            switch (ai.IsCanAlertState)
            {
                case true:
                    return;
                case false when ai.CanAlert:
                    ai.IsCanAlertState = true;
                    break;
            }
        }
    }
}