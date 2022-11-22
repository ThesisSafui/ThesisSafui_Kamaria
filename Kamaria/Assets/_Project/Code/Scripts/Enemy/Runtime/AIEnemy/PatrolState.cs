using System.Collections;
using System.Collections.Generic;
using Kamaria.Enemy.AIEnemy.Fish;
using Kamaria.Enemy.AIEnemy.Golem;
using Kamaria.Enemy.AIEnemy.Mouse;
using Kamaria.Enemy.AIEnemy.SharkBoss;
using Kamaria.Enemy.AIEnemy.SkeletonBoss;
using UnityEngine;
using Random = System.Random;

namespace Kamaria.Enemy.AIEnemy
{
    public sealed class PatrolState : BaseAIFiniteStateMachine
    {
        private int currentWaypointIndex = 0;
        private IEnumerator coroutinePatrol;
        private bool isMove = false;
        private bool isPatrolState = false;
        private Random random = new Random();
        private int animMove;
        private int walk = 1;
        private int idle = 0;
        private bool isAnimMove = false;
        private bool canCheckRemainingDistance = false;

        #region ENEMIES

        private AIMouse aiMouse;
        private AIFish aiFish;
        private AIGolem aiGolem;
        private AISharkBoss aiSharkBoss;
        private AISkeletonBoss aiSkeletonBoss;

        #endregion

        public PatrolState(BaseAI ai, List<BaseAIFiniteStateMachine> statesData)
        {
            State = AIState.Patrol;
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
            ai.Agent.isStopped = false;
            ai.Agent.ResetPath();
            
            coroutinePatrol = Patrol();
            
            currentWaypointIndex = 0;
            isPatrolState = true;
            isMove = false;
            isAnimMove = false;
            canCheckRemainingDistance = true;

            ai.StartCoroutine(coroutinePatrol);
        }

        public override void UpdateState()
        {
            if (ai.IsDead)
            {
                ai.NextState(statesData.Find(x => x.State == AIState.Dead));
            }
            
            MoveAnimation();
            
            CheckRemainingDistance();
            
            ChangeToAlertState();
        }

        public override void ExitState()
        {
            ai.Agent.ResetPath();
            ai.Agent.isStopped = true;
            ai.StopCoroutine(coroutinePatrol);
        }

        private IEnumerator Patrol()
        {
            while (isPatrolState)
            {
                yield return new WaitWhile(() => isMove);
                isMove = true;
                canCheckRemainingDistance = false;
               
                int lastWaypointIndex = currentWaypointIndex;
                currentWaypointIndex = random.Next(ai.WaypointHandled.Waypoints.Count);
                while (currentWaypointIndex == lastWaypointIndex)
                {
                    currentWaypointIndex = random.Next(ai.WaypointHandled.Waypoints.Count);
                }

                yield return new WaitForSeconds(ai.PatrolNextPointDuration);
                
                canCheckRemainingDistance = true;
                isAnimMove = true;
                
                ai.Agent.SetDestination(ai.WaypointHandled.Waypoints[currentWaypointIndex].position);
            }
        }
        
        private void CheckRemainingDistance()
        {
            if (ai.Agent.remainingDistance < ai.Agent.stoppingDistance && canCheckRemainingDistance)
            {
                isMove = false;
                isAnimMove = false;
            }
        }
        
        private void ChangeToAlertState()
        {
            if (!ai.IsCanAlertState) return;
            
            ai.NextState(statesData.Find(x => x.State == AIState.Alert));
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
    }
}