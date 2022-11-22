using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Player.Controller
{
    public sealed class SkillAttackGuardCounterState : StateMachineBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            playerData.PlayerAnimation.IsAttacking = true;
            playerData.PlayerAnimation.IsAnimationAttackNotMove = true;
            playerData.CharacterSkillData.CurrentCombo = 0;
            
            animator.SetBool(playerData.PlayerAnimation.AnimIsAttacking, false);
            animator.SetBool(playerData.PlayerAnimation.AnimIsCanMove, false);
            
            animator.gameObject.GetComponent<CharacterSkill>().ResetComboAttack();
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            playerData.CharacterSkillData.UsingGuard = false;
            playerData.CharacterSkillData.IsUseActiveSkill = false;
            playerData.PlayerAnimation.IsAttacking = false;
            playerData.PlayerAnimation.IsAnimationAttackNotMove = false;
            playerData.CharacterSkillData.IsUseSkillCanNotAttack = false;
            animator.SetBool(playerData.PlayerAnimation.AnimIsCanMove, true);
            animator.SetBool(playerData.PlayerAnimation.AnimIsGuardCounter, false);
            animator.gameObject.GetComponent<CharacterSkill>().ResetComboAttack();
            animator.gameObject.GetComponent<CharacterSkill>().ResetColliderDamage();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            animator.SetBool(playerData.PlayerAnimation.AnimIsCanMove, false);
            playerData.PlayerAnimation.IsAttacking = true;
            playerData.PlayerAnimation.IsAnimationAttackNotMove = true;
        }

        /*public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
        }

        public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
        }*/
    }
}