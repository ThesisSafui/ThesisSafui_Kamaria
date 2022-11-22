using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Player.Controller
{
    public sealed class AttackStartState : StateMachineBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            animator.SetBool(playerData.PlayerAnimation.AnimIsAttacking, true);
            playerData.PlayerAnimation.IsAttacking = true;
            playerData.PlayerAnimation.IsAnimationAttackNotMove = true;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
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