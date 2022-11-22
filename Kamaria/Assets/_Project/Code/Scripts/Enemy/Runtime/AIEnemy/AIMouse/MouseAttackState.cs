using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using static System.Decimal;
using RandomUnity = UnityEngine.Random;
using RandomSystem = System.Random;

namespace Kamaria.Enemy.AIEnemy.Mouse
{
    public sealed class MouseAttackState : BaseAIFiniteStateMachine
    {
        private AIMouse aiMouse;
        private int walk = 1;
        private int idle = 0;
        private RandomSystem random = new RandomSystem();
        private EnemyAttackData[] attacksFullNear;
        private EnemyAttackData[] attacksFullFar;
        private EnemyAttackData[] attacksNearLevel1;
        private EnemyAttackData[] attacksFarLevel1;
        private EnemyAttackData[] attacksNearLevel2;
        private EnemyAttackData[] attacksFarLevel2;
        private List<EnemyAttackData[]> attacksNear;
        private List<EnemyAttackData[]> attacksFar;
        private bool isChargeAttack = false;
        private float chargeAttackDuration;
        private IEnumerator coroutineChargeAttack;
        private IEnumerator coroutineDurationAttack;
        private Sequence tweenSequence;

        #region ANIMATION
        
        private int animIsChargeAttack;
        private EnemyAttackData animNormalAttack;
        private EnemyAttackData animSpineAttack;
        private EnemyAttackData animContinuousAttack;
        private EnemyAttackData animJumpAttack1;
        private EnemyAttackData animJumpAttack2;
        private EnemyAttackData animCurrent;

        #endregion

        public MouseAttackState(BaseAI ai, List<BaseAIFiniteStateMachine> statesData)
        {
            #region GET_DATA
            
            State = AIState.Attack;
            base.ai = ai;
            base.statesData = statesData;
            
            aiMouse = ai.GetComponent<AIMouse>();
            animIsChargeAttack = aiMouse.AnimIsChargeAttack;
            animNormalAttack = new EnemyAttackData(aiMouse.AnimNormalAttack, EnemyAttackTypes.Normal,
                aiMouse.MoveNormalAttack);
            animSpineAttack = new EnemyAttackData(aiMouse.AnimSpinAttack, EnemyAttackTypes.CanCharge,
                aiMouse.MoveSpineAttack);
            animContinuousAttack = new EnemyAttackData(aiMouse.AnimContinuousAttack, EnemyAttackTypes.Normal,
                aiMouse.MoveContinuousAttack);
            animJumpAttack1 = new EnemyAttackData(aiMouse.AnimJumpAttack1, EnemyAttackTypes.CanCharge,
                aiMouse.MoveJumpAttack1);
            animJumpAttack2 = new EnemyAttackData(aiMouse.AnimJumpAttack2, EnemyAttackTypes.CanCharge,
                aiMouse.MoveJumpAttack2);
            
            GetAttackLevel();

            #endregion
        }
        
        public override void EnterState()
        {
            ai.Agent.ResetPath();
            ai.Agent.isStopped = true;

            coroutineChargeAttack = ChargeAttack();
            coroutineDurationAttack = DurationAttack();
            RotationAttack();

            ai.IsAttackFinish = false;
            ai.IsFirstDefinitelyAttack = false;
            ai.CanAttack = false;

            RandomAttackType();
            
            ai.Animator.SetBool(animIsChargeAttack, isChargeAttack);
            ai.Animator.SetTrigger(animCurrent.Anim);
        }
        
        public override void UpdateState()
        {
            if (ai.IsDead)
            {
                tweenSequence.Pause();
                ai.NextState(statesData.Find(x => x.State == AIState.Dead));
            }
            
            if (ai.IsAttackFinish)
            {
                ChangeState();
            }
        }

        public override void ExitState()
        {
            ai.StartCoroutine(coroutineDurationAttack);
            ai.IsAttackState = false;
            ai.IsAttackNear = false;
            ai.IsDefinitelyAttack = false;
            ai.Animator.SetBool(animIsChargeAttack, false);
            ai.DefinitelyAttack();
            ai.SpAmIcon.SetActive(false);
        }
        
        private void RandomAttackType()
        {
            isChargeAttack = random.Next(2) switch
            {
                0 => true,
                1 => false,
                _ => isChargeAttack
            };
            
            animCurrent = ai.IsAttackNear
                ? attacksNear[ai.IndexLevel(ai.EnemyLevel)][random.Next(attacksNear[ai.IndexLevel(ai.EnemyLevel)].Length)]
                : attacksFar[ai.IndexLevel(ai.EnemyLevel)][random.Next(attacksFar[ai.IndexLevel(ai.EnemyLevel)].Length)];

            ai.GetMoveAttackData(animCurrent.MoveAttackData);
            
            if (animCurrent.AttackTypes != EnemyAttackTypes.CanCharge) return;
            
            decimal tempChargeAttackDuration =
                (decimal)RandomUnity.Range(ai.MinChargeAttackDuration, ai.MaxChargeAttackDuration);
            chargeAttackDuration = (float)Round(tempChargeAttackDuration, 2);
            ai.StartCoroutine(coroutineChargeAttack);
        }
        
        private void ChangeState()
        {
            switch (random.Next(2))
            {
                case 0:
                    ai.NextState(statesData.Find(x => x.State == AIState.Chase));
                    break;
                case 1:
                    ai.NextState(statesData.Find(x => x.State == AIState.Escape));
                    break;
            }
        }

        private void GetAttackLevel()
        {
            attacksFullNear = new[] { animNormalAttack, animSpineAttack, animContinuousAttack, animJumpAttack1, animJumpAttack2 };
            attacksFullFar = new[] { animSpineAttack, animContinuousAttack, animJumpAttack1, animJumpAttack2 };
            
            attacksNearLevel1 = new[] { animNormalAttack, animJumpAttack1};
            attacksFarLevel1 = new[] { animJumpAttack1 };

            attacksNearLevel2 = new[] { animNormalAttack, animJumpAttack1, animSpineAttack };
            attacksFarLevel2 = new[] { animJumpAttack1, animSpineAttack };
            
            attacksFar = new List<EnemyAttackData[]> { attacksFarLevel1, attacksFarLevel2, attacksFullFar };
            attacksNear = new List<EnemyAttackData[]> { attacksNearLevel1, attacksNearLevel2, attacksFullNear };
        }

        private IEnumerator ChargeAttack()
        {
            yield return new WaitForSeconds(chargeAttackDuration);
            ai.Animator.SetBool(animIsChargeAttack, false);
        }

        private IEnumerator DurationAttack()
        {
            yield return new WaitForSeconds(ai.CanAttackDuration);
            ai.CanAttack = true;
        }
        
        private void RotationAttack()
        {
            Quaternion rotation =
                Quaternion.LookRotation(new Vector3(ai.Target.transform.position.x, 0,
                    ai.Target.transform.position.z) - new Vector3(ai.gameObject.transform.position.x, 0,
                    ai.gameObject.transform.position.z), Vector3.up);

            tweenSequence = DOTween.Sequence();
            tweenSequence.Append(ai.gameObject.transform.DORotateQuaternion(rotation, ai.RotationAttackDuration));
        }
    }
}