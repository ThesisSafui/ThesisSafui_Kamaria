using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Player.Controller
{
    public class SkillRedStoneEndState : StateMachineBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            playerData.CharacterSkillData.IsUseUltimateSkill = true;
            playerData.PlayerAnimation.IsAttacking = true;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            playerData.CharacterSkillData.IsUseUltimateSkill = false;
            playerData.PlayerAnimation.IsAttacking = false;
            playerData.PlayerAnimation.IsAnimationAttackNotMove = false;
            playerData.CharacterSkillData.IsUseSkillCanNotAttack = false;
            animator.SetBool(playerData.PlayerAnimation.AnimIsCanMove, true);
            animator.gameObject.GetComponent<CharacterSkill>().ResetComboAttack();
            animator.gameObject.GetComponent<CharacterSkill>().ResetColliderDamage();
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