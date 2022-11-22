using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Player.Controller
{
    public class AnimChangeWeaponState : StateMachineBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            playerData.CharacterSkillData.ChangeWeaponFinish = false;
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            playerData.CharacterSkillData.ChangeWeaponFinish = true;
            animator.SetBool(playerData.PlayerAnimation.AnimIsChangeWeapon, false);
            playerData.PlayerAnimation.IsAnimationAttackNotMove = false;
            playerData.PlayerAnimation.IsAttacking = false;
            
            playerData.CharacterSkillData.PunchFinish = true;
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            playerData.PlayerAnimation.IsAnimationAttackNotMove = true;
            playerData.PlayerAnimation.IsAttacking = true;
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