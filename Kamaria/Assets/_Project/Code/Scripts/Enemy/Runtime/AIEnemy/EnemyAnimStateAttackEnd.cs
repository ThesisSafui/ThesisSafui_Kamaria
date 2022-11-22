using UnityEngine;

namespace Kamaria.Enemy.AIEnemy
{
    public sealed class EnemyAnimStateAttackEnd : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            var enemy = animator.gameObject.GetComponent<BaseAI>();
            enemy.SetAttackFinish(true);
            enemy.ResetGetDamage();
            enemy.ResetColliderAttackAndVFX();
        }

        // public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo,
        //     int layerIndex)
        // {
        // }

        // public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo,
        //     int layerIndex)
        // {
        // }
        //
        // public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo,
        //     int layerIndex)
        // {
        // }
    }
}