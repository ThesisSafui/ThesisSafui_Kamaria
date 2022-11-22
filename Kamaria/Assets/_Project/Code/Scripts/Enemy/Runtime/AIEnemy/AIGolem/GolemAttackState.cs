using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using RandomUnity = UnityEngine.Random;
using RandomSystem = System.Random;

namespace Kamaria.Enemy.AIEnemy.Golem
{
    public sealed class GolemAttackState : BaseAIFiniteStateMachine
    {
        private AIGolem aiGolem;
        private RandomSystem random = new RandomSystem();
        private EnemyAttackData[] attacksFullNear;
        private EnemyAttackData[] attacksNearLevel1;
        private EnemyAttackData[] attacksNearLevel2;
        private List<EnemyAttackData[]> attacksNear;
        private bool isChargeAttack = false;
        private bool isSpinEnd = true;
        private float chargeAttackDuration;
        private IEnumerator coroutineChargeAttack;
        private IEnumerator coroutineDurationAttack;
        private IEnumerator coroutineSpinAttack;
        private Sequence tweenSequence;
        
        #region ANIMATION
        
        private int animIsChargeAttack;
        private int animIsSpinEnd;
        private EnemyAttackData animAttack1;
        private EnemyAttackData animAttack2;
        private EnemyAttackData animSpinAttack;
        private EnemyAttackData animCurrent;

        #endregion
        
        public GolemAttackState(BaseAI ai, List<BaseAIFiniteStateMachine> statesData)
        {
            #region GET_DATA
            
            State = AIState.Attack;
            base.ai = ai;
            base.statesData = statesData;
            
            aiGolem = ai.GetComponent<AIGolem>();
            animIsChargeAttack = aiGolem.AnimIsChargeAttack;
            animIsSpinEnd = aiGolem.AnimIsSpinEnd;
            animAttack1 = new EnemyAttackData(aiGolem.AnimAttack1, EnemyAttackTypes.CanCharge);
            animAttack2 = new EnemyAttackData(aiGolem.AnimAttack2, EnemyAttackTypes.SpinGolem);
            animSpinAttack = new EnemyAttackData(aiGolem.AnimSpinAttack, EnemyAttackTypes.SpinGolem);

            GetAttackLevel();

            #endregion
        }
        
        public override void EnterState()
        {
            ai.Agent.ResetPath();
            ai.Agent.isStopped = true;

            coroutineChargeAttack = ChargeAttack();
            coroutineDurationAttack = DurationAttack();
            coroutineSpinAttack = SpinAttack();
            
            RotationAttack();

            ai.IsAttackFinish = false;
            ai.IsFirstDefinitelyAttack = false;
            ai.CanAttack = false;
            isChargeAttack = false;
            isSpinEnd = true;

            RandomAttackType();
            
            ai.Animator.SetBool(animIsChargeAttack, isChargeAttack);
            ai.Animator.SetBool(animIsSpinEnd, isSpinEnd);
            ai.Animator.SetTrigger(animCurrent.Anim);
        }

        public override void UpdateState()
        {
            if (ai.IsDead)
            {
                tweenSequence.Pause();
                ai.NextState(statesData.Find(x => x.State == AIState.Dead));
            }

            if (!ai.Agent.isStopped)
            {
                ai.Agent.SetDestination(ai.Target.transform.position);
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
            ai.Animator.SetBool(animIsSpinEnd, true);
            ai.DefinitelyAttack();
            aiGolem.CloseAllDamageCollider();
            aiGolem.IndicatorAttack.SetActive(false);
        }
        
        private void GetAttackLevel()
        {
            attacksNearLevel1 = new[] { animAttack1};
            attacksNearLevel2 = new[] { animAttack1, animSpinAttack };
            attacksFullNear = new[] { animAttack1, animSpinAttack, animAttack2 };
            
            attacksNear = new List<EnemyAttackData[]> { attacksNearLevel1, attacksNearLevel2, attacksFullNear };
        }
        
        private void RandomAttackType()
        {
            animCurrent = attacksNear[ai.IndexLevel(ai.EnemyLevel)]
                [random.Next(attacksNear[ai.IndexLevel(ai.EnemyLevel)].Length)];

            if (animCurrent.AttackTypes == EnemyAttackTypes.Normal) return;

            if (animCurrent.AttackTypes == EnemyAttackTypes.CanCharge)
            {
                aiGolem.IndicatorAttack.SetActive(true);
                isChargeAttack = true;
                ai.StartCoroutine(coroutineChargeAttack);
            }
            else if (animCurrent.AttackTypes == EnemyAttackTypes.SpinGolem)
            {
                ai.Agent.isStopped = false;
                isSpinEnd = false;
                ai.StartCoroutine(coroutineSpinAttack);
            }
        }
        
        private void ChangeState()
        {
            ai.NextState(statesData.Find(x => x.State == AIState.Chase));
        }
        
        private IEnumerator ChargeAttack()
        {
            yield return new WaitForSeconds(aiGolem.MaxChargeAttackDuration);
            ai.Animator.SetBool(animIsChargeAttack, false);
            aiGolem.IndicatorAttack.SetActive(false);
        }
        
        private IEnumerator SpinAttack()
        {
            aiGolem.PlaySfxGoloemSpinAttack();
            yield return new WaitForSeconds(aiGolem.SpinTime);
            aiGolem.StopSfxGoloemSpinAttack();
            aiGolem.CloseAllDamageCollider();
            ai.Agent.ResetPath();
            ai.Agent.isStopped = true;
            ai.Animator.SetBool(animIsSpinEnd, true);
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