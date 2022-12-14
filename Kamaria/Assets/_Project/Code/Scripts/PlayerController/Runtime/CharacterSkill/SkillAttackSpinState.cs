using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Player.Controller
{
    public sealed class SkillAttackSpinState : StateMachineBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            playerData.PlayerAnimation.IsAttacking = true;
            playerData.PlayerAnimation.IsAnimationAttackNotMove = true;
            playerData.CharacterSkillData.CurrentCombo = 0;
            
            animator.SetBool(playerData.PlayerAnimation.AnimIsAttacking, false);
            
            animator.gameObject.GetComponent<CharacterSkill>().ResetComboAttack();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            playerData.CharacterSkillData.IsUseActiveSkill = false;
            playerData.PlayerAnimation.IsAttacking = false;
            playerData.PlayerAnimation.IsAnimationAttackNotMove = false;
            playerData.CharacterSkillData.IsUseSkillCanNotAttack = false;
            animator.SetBool(playerData.PlayerAnimation.AnimIsAttackSpin, false);
            animator.SetBool(playerData.PlayerAnimation.AnimIsCanMove, true);
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            playerData.PlayerAnimation.IsAttacking = true;
            playerData.PlayerAnimation.IsAnimationAttackNotMove = true;
        }
        
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