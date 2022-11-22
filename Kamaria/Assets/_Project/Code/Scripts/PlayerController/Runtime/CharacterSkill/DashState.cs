using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Player.Controller
{
    public sealed class DashState : StateMachineBehaviour
    {
        [SerializeField] private PlayerDataSO playerData;
        
        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
            playerData.CloseAllColliderDamage();
            animator.gameObject.GetComponent<PlayerEvent>().CloseVfx();
            animator.GetComponent<PlayerEvent>().CloseSpine();
            animator.gameObject.GetComponent<CharacterSkill>().ResetComboAttack();
        }

        /*public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo,
            int layerIndex)
        {
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo,
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