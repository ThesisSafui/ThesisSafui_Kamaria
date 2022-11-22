using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Player.Controller
{
    public class SkillRedStoneStartState : StateMachineBehaviour
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
            playerData.PlayerAnimation.IsAnimationAttackNotMove = false;
            animator.gameObject.GetComponent<CharacterSkill>().ResetComboAttack();
            animator.gameObject.GetComponent<CharacterSkill>().ResetColliderDamage();
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