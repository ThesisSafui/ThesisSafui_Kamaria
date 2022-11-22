using System.Collections;
using System.Collections.Generic;
using Kamaria.Enemy.AIEnemy.Fish;
using Kamaria.Enemy.AIEnemy.Golem;
using Kamaria.Enemy.AIEnemy.Mouse;
using UnityEngine;

namespace Kamaria.Enemy.AIEnemy
{
    public sealed class StunState : BaseAIFiniteStateMachine
    {
        private int animMove;
        private int animIsStun;
        private int walk = 1;
        private int idle = 0;
        private float stunTime;
        private bool isAnimMove;
        private IEnumerator coroutineStun;
        
        #region ENEMIES

        private AIMouse aiMouse;
        private AIFish aiFish;
        private AIGolem aiGolem;
        
        #endregion
        
        public StunState(BaseAI ai, List<BaseAIFiniteStateMachine> statesData)
        {
            State = AIState.Stun;
            base.ai = ai;
            base.statesData = statesData;
           
            if (ai.EnemyType.Equals(Enemies.Mouse))
            {
                aiMouse = ai.GetComponent<AIMouse>();
                animMove = aiMouse.AnimMove;
                animIsStun = aiMouse.AnimIsStun;
            }
            else if (ai.EnemyType.Equals(Enemies.Fish))
            {
                aiFish = ai.GetComponent<AIFish>();
                animMove = aiFish.AnimMove;
                animIsStun = aiFish.AnimIsStun;
            }
            else if (ai.EnemyType.Equals(Enemies.Golem))
            {
                aiGolem = ai.GetComponent<AIGolem>();
                animMove = aiGolem.AnimMove;
                animIsStun = aiGolem.AnimIsStun;
            }
        }
        
        public override void EnterState()
        {
            ai.ResetCurrentCombo();
            ai.ResetColliderAttackAndVFX();
            coroutineStun = StunTime();
            ai.StopCoroutine(coroutineStun);
            ai.StartCoroutine(coroutineStun);
        }

        public override void UpdateState()
        {
            MoveAnimation();

            if (!ai.IsStun)
            {
                ai.NextState(statesData.Find(x => x.State == AIState.Chase));
            }
            
            if (ai.IsDead)
            {
                ai.NextState(statesData.Find(x => x.State == AIState.Dead));
            }
        }

        public override void ExitState()
        {
            Stun(false);
        }
        
        private IEnumerator StunTime()
        {
            Stun(true);
            ai.VfxStun.SetActive(true);
            yield return new WaitForSeconds(ai.StunTime);
            ai.VfxStun.SetActive(false);
            Stun(false);
        }

        private void Stun(bool isStun)
        {
            ai.IsStun = isStun;
            ai.Agent.isStopped = isStun;
            ai.Animator.SetBool(animIsStun, ai.IsStun);
            isAnimMove = !isStun;
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