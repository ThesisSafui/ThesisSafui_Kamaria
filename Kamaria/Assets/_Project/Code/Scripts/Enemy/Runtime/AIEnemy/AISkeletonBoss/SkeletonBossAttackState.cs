using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = System.Random;

namespace Kamaria.Enemy.AIEnemy.SkeletonBoss
{
    public sealed class SkeletonBossAttackState : BaseAIFiniteStateMachine
    {
        private AISkeletonBoss aiSkeletonBoss;
        private Random random = new Random();
        private bool isChargeAttack = false;
        private bool isLookTarget = false;
        private float chargeAttackDuration;
        private IEnumerator coroutineChargeAttack;
        private IEnumerator coroutineDurationAttack;
        private IEnumerator coroutineLookTarget;
        private Sequence tweenSequence;
        private SkeletonBossAnimation attackType;

        #region ANIMATION
        
        private int animIsChargeAttack;
        private EnemyAttackData animNormalAttack;
        private EnemyAttackData animPistolAttack;
        private EnemyAttackData animMeteorAttack;
        private EnemyAttackData animCurrent;

        #endregion
        
        public SkeletonBossAttackState(BaseAI ai, List<BaseAIFiniteStateMachine> statesData)
        {
            #region GET_DATA
            
            State = AIState.Attack;
            base.ai = ai;
            base.statesData = statesData;
            
            aiSkeletonBoss = ai.GetComponent<AISkeletonBoss>();
            animIsChargeAttack = aiSkeletonBoss.AnimIsChargeAttack;
            animNormalAttack = new EnemyAttackData(aiSkeletonBoss.AnimNormalAttack, EnemyAttackTypes.Normal);
            animPistolAttack = new EnemyAttackData(aiSkeletonBoss.AnimPistolAttack, EnemyAttackTypes.CanCharge);
            animMeteorAttack = new EnemyAttackData(aiSkeletonBoss.AnimMeteorAttack, EnemyAttackTypes.Normal);

            #endregion
        }
        
        public override void EnterState()
        {
            ai.Agent.ResetPath();
            ai.Agent.isStopped = true;

            coroutineChargeAttack = ChargeAttack();
            coroutineDurationAttack = DurationAttack();
            coroutineLookTarget = LookTarget();
            
            RotationAttack();

            ai.IsAttackFinish = false;
            ai.IsFirstDefinitelyAttack = false;
            ai.CanAttack = false;
            isChargeAttack = true;
            isLookTarget = false;

            AttackType();
            
            ai.Animator.SetBool(animIsChargeAttack, isChargeAttack);
            ai.Animator.SetTrigger(animCurrent.Anim);

            if (attackType == SkeletonBossAnimation.meteorAttack)
            {
                aiSkeletonBoss.MeteorAttack();
                aiSkeletonBoss.CoolDownMeteor();
            }
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
            ai.Animator.SetBool(animIsChargeAttack, false);
            ai.ResetColliderAttackAndVFX();
        }

        private void AttackType()
        {
            if (aiSkeletonBoss.IsAttackNear)
            {
                if (aiSkeletonBoss.CanMeteor && !aiSkeletonBoss.CanPistol)
                {
                    attackType = SkeletonBossAnimation.meteorAttack;
                    animCurrent = animMeteorAttack;
                }
                else if (!aiSkeletonBoss.CanMeteor && aiSkeletonBoss.CanPistol)
                {
                    attackType = SkeletonBossAnimation.pistolAttack;
                    animCurrent = animPistolAttack;
                }
                else if (!aiSkeletonBoss.CanMeteor && !aiSkeletonBoss.CanPistol)
                {
                    attackType = SkeletonBossAnimation.normalAttack;
                    animCurrent = animNormalAttack;
                }
                else if (aiSkeletonBoss.CanMeteor && aiSkeletonBoss.CanPistol)
                {
                    int attack = random.Next(2);
                    switch (attack)
                    {
                        case 0:
                            attackType = SkeletonBossAnimation.meteorAttack;
                            animCurrent = animMeteorAttack;
                            break;
                        case 1:
                            attackType = SkeletonBossAnimation.pistolAttack;
                            animCurrent = animPistolAttack;
                            break;
                    }
                }
            }
            else
            {
                if (aiSkeletonBoss.CanMeteor && !aiSkeletonBoss.CanPistol)
                {
                    attackType = SkeletonBossAnimation.meteorAttack;
                    animCurrent = animMeteorAttack;
                }
                else if (!aiSkeletonBoss.CanMeteor && aiSkeletonBoss.CanPistol)
                {
                    attackType = SkeletonBossAnimation.pistolAttack;
                    animCurrent = animPistolAttack;
                }
                else if (aiSkeletonBoss.CanMeteor && aiSkeletonBoss.CanPistol)
                {
                    int attack = random.Next(2);
                    switch (attack)
                    {
                        case 0:
                            attackType = SkeletonBossAnimation.meteorAttack;
                            animCurrent = animMeteorAttack;
                            break;
                        case 1:
                            attackType = SkeletonBossAnimation.pistolAttack;
                            animCurrent = animPistolAttack;
                            break;
                    }
                }
            }

            if (animCurrent.AttackTypes != EnemyAttackTypes.CanCharge) return;
            
            isChargeAttack = true;
            isLookTarget = true;
            ai.StartCoroutine(coroutineChargeAttack);
            ai.StartCoroutine(coroutineLookTarget);

        }
        
        private void ChangeState()
        {
            ai.NextState(statesData.Find(x => x.State == AIState.Chase));
        }
        
        private IEnumerator ChargeAttack()
        { 
            if (attackType == SkeletonBossAnimation.pistolAttack)
            {
                aiSkeletonBoss.IndicatorPistolAttack.SetActive(true);
            }
            yield return new WaitForSeconds(aiSkeletonBoss.MaxChargeAttackDuration);
            isLookTarget = false;
            ai.Animator.SetBool(animIsChargeAttack, false);
            
            if (attackType == SkeletonBossAnimation.pistolAttack)
            {
                aiSkeletonBoss.IndicatorPistolAttack.SetActive(false);
                aiSkeletonBoss.CoolDownPistol();
                ai.Agent.ResetPath();
                ai.Agent.isStopped = true;
            }
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

        private IEnumerator LookTarget()
        {
            while (isLookTarget)
            {
                RotationAttack();
                yield return null;
            }
        }
    }
}