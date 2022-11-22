using UnityEngine;

namespace Kamaria.Enemy.AIEnemy.SkeletonBoss
{
    public class AnimHealStateSkeleton : StateMachineBehaviour
    {
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            if (!animator.TryGetComponent(out AISkeletonBoss target)) return;
            target.HealFinish = true;
        }

        /*public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
        }

        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
        }

        public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
        }*/
    }
}