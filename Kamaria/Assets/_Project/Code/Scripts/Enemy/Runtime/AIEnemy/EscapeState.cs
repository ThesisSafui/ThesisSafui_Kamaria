using System.Collections;
using System.Collections.Generic;
using Kamaria.Enemy.AIEnemy.Fish;
using Kamaria.Enemy.AIEnemy.Mouse;
using UnityEngine;

namespace Kamaria.Enemy.AIEnemy
{
    public sealed class EscapeState : BaseAIFiniteStateMachine
    {
        private int animMove;
        private int walk = 1;
        private IEnumerator coroutineEscapeTime;
        private float stoppingDistance;

        #region ENEMIES

        private AIMouse aiMouse;
        private AIFish aiFish;

        #endregion
        
        public EscapeState(BaseAI ai, List<BaseAIFiniteStateMachine> statesData)
        {
            State = AIState.Escape;
            base.ai = ai;
            base.statesData = statesData;
            stoppingDistance = ai.Agent.stoppingDistance;
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
        }
        
        public override void EnterState()
        {
            ai.Agent.stoppingDistance = 0;
            
            ai.Agent.ResetPath();
            ai.Agent.isStopped = false;

            coroutineEscapeTime = EscapeTime();
            
            ai.IsEscapeState = true;
            ai.StartCoroutine(coroutineEscapeTime);
        }

        public override void UpdateState()
        {
            if (ai.IsDead)
            {
                ai.NextState(statesData.Find(x => x.State == AIState.Dead));
            }
            
            MoveAnimation();
            
            Escape();
            
            ChangeState();
        }

        public override void ExitState()
        {
            ai.Agent.stoppingDistance = stoppingDistance;
            ai.StopCoroutine(coroutineEscapeTime);
            ai.IsEscapeState = false;
        }

        private void Escape()
        {
            Vector3 target = ai.Target.transform.position + ai.Target.transform.forward;
            ai.Agent.SetDestination((ai.transform.position - target) + ai.transform.position);
        }

        private void ChangeState()
        {
            if (!ai.IsEscapeState)
            {
                ai.NextState(statesData.Find(x => x.State == AIState.Chase));
            }
            else if (ai.IsAttackState)
            {
                ai.NextState(statesData.Find(x => x.State == AIState.Attack));
            }
            else if (ai.IsResetState)
            {
                ai.IsResetState = false;
                ai.NextState(statesData.Find(x => x.State == AIState.Patrol));
            }
        }
        
        private void MoveAnimation()
        {
            ai.Animator.SetFloat(animMove, walk, ai.WalkAnimDumpTime, Time.deltaTime);
        }
        
        private IEnumerator EscapeTime()
        {
            yield return new WaitForSeconds(ai.EscapeTime);
            ai.IsEscapeState = false;
        }
    }
}