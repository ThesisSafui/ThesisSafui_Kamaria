using UnityEngine;

namespace Kamaria.Enemy.AIEnemy
{
    public sealed class AIAreaAttackNear : MonoBehaviour
    {
        [SerializeField] private BaseAI ai;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;

            if (ai.CanAttack)
            {
                ai.IsAttackNear = true;
            }
            /*if (!ai.IsAttackState && ai.CanAttack)
            {
                ai.IsAttackState = true;
                ai.IsAttackNear = true;
            }*/
        }

        private void OnTriggerStay(Collider other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            
            if (ai.CanAttack)
            {
                ai.IsAttackNear = true;
            }
            /*switch (ai.IsAttackState)
            {
                case true:
                    return;
                case false when ai.CanAttack:
                    ai.IsAttackState = true;
                    ai.IsAttackNear = true;
                    break;
            }*/
        }
    }
}