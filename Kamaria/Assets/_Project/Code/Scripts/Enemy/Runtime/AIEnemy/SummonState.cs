using System.Collections;
using System.Collections.Generic;
using Kamaria.Enemy.AIEnemy.SharkBoss;
using Kamaria.Enemy.AIEnemy.SkeletonBoss;
using UnityEngine;

namespace Kamaria.Enemy.AIEnemy
{
    public sealed class SummonState : BaseAIFiniteStateMachine
    {
        private IEnumerator coroutineWait;
        private IEnumerator coroutineChargeAttack;
        private bool isFinish;
        
        #region ENEMIES

        #region SHARKBOSS

        private int animIsJump;

        #endregion
        
        #region SHARKBOSS

        #endregion

        private int animSummon;
        private int animIsChargeAttack;
        private AISharkBoss aiSharkBoss;
        private AISkeletonBoss aiSkeletonBoss;

        #endregion
        
        public SummonState(BaseAI ai, List<BaseAIFiniteStateMachine> statesData)
        {
            State = AIState.Summon;
            base.ai = ai;
            base.statesData = statesData;
           
            if (ai.EnemyType.Equals(Enemies.SharkBoss))
            {
                aiSharkBoss = ai.GetComponent<AISharkBoss>();
                animIsJump = aiSharkBoss.AnimIsJump;
                animSummon = aiSharkBoss.AnimSummon;
                animIsChargeAttack = aiSharkBoss.AnimIsChargeAttack;
            }
            else if (ai.EnemyType.Equals(Enemies.SkeletonBoss))
            {
                aiSkeletonBoss = ai.GetComponent<AISkeletonBoss>();
                animSummon = aiSkeletonBoss.AnimSummon;
            }
        }
        
        public override void EnterState()
        {
            ai.Agent.ResetPath();
            ai.Agent.isStopped = true;
            isFinish = false;
            if (ai.EnemyType == Enemies.SharkBoss)
            {
                ai.Agent.ResetPath();
                ai.Agent.isStopped = true;

                ai.Agent.enabled = false;
                
                aiSharkBoss.Animator.SetTrigger(animSummon);
                aiSharkBoss.Animator.SetBool(animIsChargeAttack, true);
                aiSharkBoss.Animator.SetBool(animIsJump, true);
                
                aiSharkBoss.EnemyClere = false;
                
                coroutineWait = SharkWaitTime();
                coroutineChargeAttack = SharkChargeAttack();

                aiSharkBoss.StartCoroutine(coroutineChargeAttack);
            }
            else
            {
                ai.Agent.ResetPath();
                ai.Agent.isStopped = true;
                aiSkeletonBoss.Animator.SetTrigger(animSummon);
                aiSkeletonBoss.Summon();
            }
        }

        public override void UpdateState()
        {
            if (ai.IsDead)
            {
                ai.NextState(statesData.Find(x => x.State == AIState.Dead));
            }
            
            if (ai.EnemyType == Enemies.SharkBoss)
            {
                aiSharkBoss.CheckEnemySummonClere();
            }

            ChangeState();
        }

        public override void ExitState()
        {
            if (ai.EnemyType == Enemies.SharkBoss)
            {
                ai.Agent.enabled = true;
                ai.Agent.ResetPath();
                ai.Agent.isStopped = true;
                aiSharkBoss.StopCoroutine(coroutineChargeAttack);
                aiSharkBoss.StopCoroutine(coroutineWait);
                aiSharkBoss.Animator.ResetTrigger(animSummon);
                aiSharkBoss.Animator.SetBool(animIsChargeAttack, true);
                aiSharkBoss.Animator.SetBool(animIsJump, false);
                isFinish = false;
            }
            else
            {
                isFinish = false;
                aiSkeletonBoss.SummonFinish = false;
            }
        }

        private void ChangeState()
        {
            if (ai.EnemyType == Enemies.SharkBoss)
            {
                if (isFinish)
                {
                    aiSharkBoss.CoolDownRush();
                    aiSharkBoss.CoolDownSlam();
                    ai.NextState(statesData.Find(x => x.State == AIState.Chase));
                }
            }
            else
            {
                if (aiSkeletonBoss.SummonFinish)
                {
                    ai.NextState(statesData.Find(x => x.State == AIState.Chase));
                }
            }
        }

        private IEnumerator SharkWaitTime()
        {
            yield return new WaitUntil(() => aiSharkBoss.EnemyClere);
            aiSharkBoss.MoveDown();
            isFinish = true;
        }
        
        private IEnumerator SharkChargeAttack()
        {
            aiSharkBoss.BlinkEffect.ContinuousBlinking();
            yield return new WaitForSeconds(aiSharkBoss.MaxChargeAttackDuration);
            
            ai.Animator.SetBool(animIsChargeAttack, false);

            aiSharkBoss.StartCoroutine(coroutineWait);
            aiSharkBoss.Summon();
        }
    }
}