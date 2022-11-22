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
    public sealed class AlertState : BaseAIFiniteStateMachine
    {
        private IEnumerator coroutineAlert;
        private IEnumerator coroutineRotationAlert;
        private int animAlert;
        private int animMove;
        private int idle = 0;

        #region ENEMIES

        private AIMouse aiMouse;
        private AIFish aiFish;
        private AIGolem aiGolem;
        private AISharkBoss aiSharkBoss;
        private AISkeletonBoss aiSkeletonBoss;

        #endregion

        public AlertState(BaseAI ai, List<BaseAIFiniteStateMachine> statesData)
        {
            State = AIState.Alert;
            base.ai = ai;
            base.statesData = statesData;
            
            if (ai.EnemyType.Equals(Enemies.Mouse))
            {
                aiMouse = ai.GetComponent<AIMouse>();
                animAlert = aiMouse.AnimAlert;
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
            ai.Agent.ResetPath();
            ai.Agent.isStopped = true;

            ai.IsAnimAlertFinish = false;
            
            coroutineAlert = Alert();
            coroutineRotationAlert = RotationAlert();

            ai.StartCoroutine(coroutineAlert);
        }

        public override void UpdateState()
        {
            if (ai.IsDead)
            {
                ai.NextState(statesData.Find(x => x.State == AIState.Dead));
            }
            
            MoveAnimation();
        }

        public override void ExitState()
        {
            ai.Agent.ResetPath();
            ai.StopCoroutine(coroutineAlert);
            ai.StopCoroutine(coroutineRotationAlert);
        }

        private IEnumerator Alert()
        {
            if (ai.EnemyType.Equals(Enemies.Mouse))
            {
                ai.StartCoroutine(coroutineRotationAlert);
                ai.Animator.SetTrigger(animAlert);
                yield return new WaitUntil(() => ai.IsAnimAlertFinish);
                yield return new WaitForSeconds(ai.AlertDuration);
                AfterAlertFinish();
            }
            else if (ai.EnemyType.Equals(Enemies.Fish))
            {
                AfterAlertFinish();
            }
            else if (ai.EnemyType.Equals(Enemies.Golem))
            {
                AfterAlertFinish();
            }
            else if (ai.EnemyType.Equals(Enemies.SharkBoss))
            {
                AfterAlertFinish();
            }
            else if (ai.EnemyType.Equals(Enemies.SkeletonBoss))
            {
                AfterAlertFinish();
            }
        }

        private void AfterAlertFinish()
        {
            ai.NextState(statesData.Find(x => x.State == AIState.Chase));
        }
        
        private void MoveAnimation()
        {
            ai.Animator.SetFloat(animMove, idle, ai.WalkAnimDumpTime, Time.deltaTime);
        }
        
        private IEnumerator RotationAlert()
        {
            while (!ai.IsAnimAlertFinish)
            {
                Quaternion rotation =
                    Quaternion.LookRotation(new Vector3(ai.Target.transform.position.x, 0,
                            ai.Target.transform.position.z) - new Vector3(ai.gameObject.transform.position.x, 0,
                            ai.gameObject.transform.position.z), Vector3.up);

                ai.gameObject.transform.rotation = Quaternion.Lerp(rotation, ai.gameObject.transform.rotation,
                    ai.RotationAlertDuration);
                yield return null;
            }
        }
    }
}