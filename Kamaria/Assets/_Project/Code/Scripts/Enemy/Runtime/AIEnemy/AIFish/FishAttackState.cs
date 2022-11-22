using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using RandomUnity = UnityEngine.Random;
using RandomSystem = System.Random;

namespace Kamaria.Enemy.AIEnemy.Fish
{
    public sealed class FishAttackState : BaseAIFiniteStateMachine
    {
        private AIFish aiFish;
        private int walk = 1;
        private int idle = 0;
        private RandomSystem random = new RandomSystem();
        private EnemyAttackData[] attacksFullFar;
        private EnemyAttackData[] attacksFarLevel1;
        private EnemyAttackData[] attacksFarLevel2;
        private List<EnemyAttackData[]> attacksFar;
        private IEnumerator coroutineDurationAttack;
        private Sequence tweenSequence;

        #region ANIMATION
        
        private EnemyAttackData animNormalAttack;
        private EnemyAttackData animCurrent;

        #endregion
        
        public FishAttackState(BaseAI ai, List<BaseAIFiniteStateMachine> statesData)
        {
            #region GET_DATA
            
            State = AIState.Attack;
            base.ai = ai;
            base.statesData = statesData;
            
            aiFish = ai.GetComponent<AIFish>();
            animNormalAttack = new EnemyAttackData(aiFish.AnimNormalAttack, EnemyAttackTypes.Normal);

            GetAttackLevel();

            #endregion
        }
        
        public override void EnterState()
        {
            ai.Agent.ResetPath();
            ai.Agent.isStopped = true;
            
            coroutineDurationAttack = DurationAttack();
            RotationAttack();

            ai.IsAttackFinish = false;
            ai.IsFirstDefinitelyAttack = false;
            ai.CanAttack = false;

            RandomAttackType();
            
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
            ai.DefinitelyAttack();
            ai.SpAmIcon.SetActive(false);
        }
        
        private void GetAttackLevel()
        {
            attacksFarLevel1 = new[] { animNormalAttack };
            attacksFarLevel2 = new[] { animNormalAttack, animNormalAttack };
            attacksFullFar = new[] { animNormalAttack, animNormalAttack, animNormalAttack };
            
            attacksFar = new List<EnemyAttackData[]> { attacksFarLevel1, attacksFarLevel2, attacksFullFar };
        }
        
        private void RandomAttackType()
        {
            aiFish.HowAttack = random.Next(attacksFar[ai.IndexLevel(ai.EnemyLevel)].Length);
            animCurrent = attacksFar[ai.IndexLevel(ai.EnemyLevel)][aiFish.HowAttack];
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
        
        private IEnumerator DurationAttack()
        {
            yield return new WaitForSeconds(ai.CanAttackDuration);
            ai.CanAttack = true;
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
    }
}