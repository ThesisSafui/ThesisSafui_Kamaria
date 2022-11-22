using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Player.Controller
{
    public sealed class AnimGunAttackState : StateMachineBehaviour
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
            animator.SetBool(playerData.PlayerAnimation.AnimIsAttacking, false);
            playerData.PlayerAnimation.IsAttacking = false;
            playerData.PlayerAnimation.IsAnimationAttackNotMove = false;

            /*if (playerData.CharacterSkillData.IsUseSkillPowerStone) return;
            
            animator.gameObject.GetComponent<PlayerControllerInput>().WaitNextComboKnuckle();*/
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