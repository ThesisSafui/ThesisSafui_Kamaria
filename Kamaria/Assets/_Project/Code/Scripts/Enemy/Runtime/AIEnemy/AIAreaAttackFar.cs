using UnityEngine;

namespace Kamaria.Enemy.AIEnemy
{
    public sealed class AIAreaAttackFar : MonoBehaviour
    {
        [SerializeField] private BaseAI ai;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            
            if (!ai.IsAttackState && ai.CanAttack)
            {
                ai.IsAttackState = true;
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;

            switch (ai.IsAttackState)
            {
                case true:
                    return;
                case false when ai.CanAttack:
                    ai.IsAttackState = true;
                    break;
            }
        }
    }
}