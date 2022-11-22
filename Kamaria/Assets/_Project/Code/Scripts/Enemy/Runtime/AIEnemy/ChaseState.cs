using System.Collections;
using System.Collections.Generic;
using Kamaria.Enemy.AIEnemy.Fish;
using Kamaria.Enemy.AIEnemy.Golem;
using Kamaria.Enemy.AIEnemy.Mouse;
using Kamaria.Enemy.AIEnemy.SharkBoss;
using Kamaria.Enemy.AIEnemy.SkeletonBoss;
using UnityEngine;

namespace Kamaria.Enemy.AIEnemy
{
    public sealed class ChaseState : BaseAIFiniteStateMachine
    {
        private int animMove;
        private int walk = 1;
        private int idle = 0;
        private bool isAnimMove;
        private bool isBossChase;
        private IEnumerator coroutineDelayChase;

        #region ENEMIES

        private AIMouse aiMouse;
        private AIFish aiFish;
        private AIGolem aiGolem;
        private AISharkBoss aiSharkBoss;
        private AISkeletonBoss aiSkeletonBoss;

        #endregion

        public ChaseState(BaseAI ai, List<BaseAIFiniteStateMachine> statesData)
        {
            State = AIState.Chase;
            base.ai = ai;
            base.statesData = statesData;
            
            if (ai.EnemyType.Equals(Enemies.Mouse))
            {
                aiMouse = ai.GetComponent<AIMouse>();
                animMove = aiMouse.AnimMove;
            }
            else if (ai.EnemyType.Equals(Enemies.Fish))
            {
                aiFish = ai.GetComponent<AIFish>();
                animMove = aiFish.AnimMove;
            }
            else if (ai.EnemyType.Equals(Enemies.Golem))
            {
                aiGolem = ai.GetComponent<AIGolem>();
                animMove = aiGolem.AnimMove;
            }
            else if (ai.EnemyType.Equals(Enemies.SharkBoss))
            {
                aiSharkBoss = ai.GetComponent<AISharkBoss>();
                animMove = aiSharkBoss.AnimMove;
            }
            else if (ai.EnemyType.Equals(Enemies.SkeletonBoss))
            {
                aiSkeletonBoss = ai.GetComponent<AISkeletonBoss>();
                animMove = aiSkeletonBoss.AnimMove;
            }
        }
        
        public override void EnterState()
        {
            isBossChase = false;
            
            if (ai.EnemyType is Enemies.SharkBoss or Enemies.SkeletonBoss)
            {
                isAnimMove = false;
                coroutineDelayChase = DelayChase();
                ai.Agent.ResetPath();
                ai.Agent.isStopped = true;
                isBossChase = true;
                ai.StartCoroutine(coroutineDelayChase);
            }
            else
            {
                ai.Agent.ResetPath();
                ai.Agent.isStopped = false;
            }
        }

        public override void UpdateState()
        {
            if (ai.IsDead)
            {
                ai.NextState(statesData.Find(x => x.State == AIState.Dead));
            }
            
            MoveAnimation();
            
            if (!isBossChase)
            {
                CheckRemainingDistance();
                Chase();
            }
            
            ChangeState();
        }

        public override void ExitState()
        {
            ai.Agent.ResetPath();
            ai.IsCanAlertState = false;
        }
        
        private void CheckRemainingDistance()
        {
            isAnimMove = !(ai.Agent.remainingDistance < ai.Agent.stoppingDistance);
        }
        
        private void MoveAnimation()
        {
            if (isAnimMove)
            {
                ai.Animator.SetFloat(animMove, walk, ai.WalkAnimDumpTime, Time.deltaTime);
            }
            else
            {
                ai.Animator.SetFloat(animMove, idle, ai.IdleAnimDumpTime, Time.deltaTime);
            }
        }

        private void Chase()
        {
            ai.Agent.SetDestination(ai.Target.transform.position);
        }

        private IEnumerator DelayChase()
        {
            yield return new WaitForSeconds(0.2f);
            isBossChase = false;
            if (ai.Agent.enabled)
            {
                ai.Agent.isStopped = false;
            }
        }
        
        private void ChangeState()
        {
            if (ai.EnemyType == Enemies.SharkBoss)
            {
                if (ai.IsResetState)
                {
                    ai.IsResetState = false;
                    ai.NextState(statesData.Find(x => x.State == AIState.Patrol));
                }
                else if (ai.IsAttackState)
                {
                    if (aiSharkBoss.IsAttackNear)
                    {
                        ai.NextState(statesData.Find(x => x.State == AIState.Attack));
                    }
                    else
                    {
                        if (!aiSharkBoss.CanRush) return;
                        ai.NextState(statesData.Find(x => x.State == AIState.Attack));
                    }
                }
                else if (aiSharkBoss.CheckSummon())
                {
                    ai.NextState(statesData.Find(x => x.State == AIState.Summon));
                }
            }
            else if (ai.EnemyType == Enemies.SkeletonBoss)
            {
                if (ai.IsResetState)
                {
                    ai.IsResetState = false;
                    ai.NextState(statesData.Find(x => x.State == AIState.Patrol));
                }
                else if (ai.IsAttackState)
                {
                    if (aiSkeletonBoss.IsAttackNear)
                    {
                        ai.NextState(statesData.Find(x => x.State == AIState.Attack));
                    }
                    else
                    {
                        if (!aiSkeletonBoss.CanPistol && !aiSkeletonBoss.CanMeteor) return;
                        ai.NextState(statesData.Find(x => x.State == AIState.Attack));
                    }
                }
                else if (aiSkeletonBoss.CanSummon)
                {
                    aiSkeletonBoss.CanSummon = false;
                    ai.NextState(statesData.Find(x => x.State == AIState.Summon));
                }
                else if (aiSkeletonBoss.CanHeal)
                {
                    aiSkeletonBoss.CanHeal = false;
                    ai.NextState(statesData.Find(x => x.State == AIState.Heal));
                }
            }
            else
            {
                if (ai.IsResetState)
                {
                    ai.IsResetState = false;
                    ai.NextState(statesData.Find(x => x.State == AIState.Patrol));
                }
                else if (ai.IsAttackState)
                {
                    ai.NextState(statesData.Find(x => x.State == AIState.Attack));
                }
            }
        }
    }
}