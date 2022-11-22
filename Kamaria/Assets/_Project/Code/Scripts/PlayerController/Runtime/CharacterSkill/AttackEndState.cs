using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Player.Controller
{
    public class AttackEndState : StateMachineBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            playerData.PlayerAnimation.IsAttacking = true;
            playerData.CharacterSkillData.CurrentCombo = 0;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            playerData.PlayerAnimation.IsAttacking = false;
            playerData.PlayerAnimation.IsAnimationAttackNotMove = false;

            animator.SetBool(playerData.PlayerAnimation.AnimIsAttacking, false);

            animator.gameObject.GetComponent<CharacterSkill>().ResetComboAttack();
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