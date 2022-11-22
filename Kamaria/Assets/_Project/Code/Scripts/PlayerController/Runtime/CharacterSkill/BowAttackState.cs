using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Player.Controller
{
    public class BowAttackState : StateMachineBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            if (playerData.CharacterSkillData.IsBowCharging) return;
            
            playerData.PlayerAnimation.IsAnimationAttackNotMove = false;
            playerData.CharacterSkillData.IsBowAttackFinish = true;
            animator.SetBool(playerData.PlayerAnimation.AnimIsAttacking, false);
        }

        // public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo,
        //     int layerIndex)
        // {
        // }
        //
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