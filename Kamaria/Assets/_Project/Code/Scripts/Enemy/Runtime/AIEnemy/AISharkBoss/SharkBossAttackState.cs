using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using RandomUnity = UnityEngine.Random;
using RandomSystem = System.Random;

namespace Kamaria.Enemy.AIEnemy.SharkBoss
{
    public sealed class SharkBossAttackState : BaseAIFiniteStateMachine
    {
        private AISharkBoss aiSharkBoss;
        private RandomSystem random = new RandomSystem();
        private bool isChargeAttack = false;
        private bool isRushing = false;
        private bool isLookTarget = false;
        private float chargeAttackDuration;
        private IEnumerator coroutineChargeAttack;
        private IEnumerator coroutineDurationAttack;
        private IEnumerator coroutineRushAttack;
        private IEnumerator coroutineSlamAttack;
        private IEnumerator coroutineMoveAttackTime;
        private IEnumerator coroutineLookTarget;
        private Sequence tweenSequence;
        private SharkBossAnimation attackType;
        private bool moveSlam;
        private float speed;

        #region ANIMATION
        
        private int animIsChargeAttack;
        private int animIsRushAttack;
        private int animIsAttackSlam;
        private EnemyAttackData animNormalAttack;
        private EnemyAttackData animSlamAttack;
        private EnemyAttackData animRushAttack;
        private EnemyAttackData animCurrent;

        #endregion
        
        public SharkBossAttackState(BaseAI ai, List<BaseAIFiniteStateMachine> statesData)
        {
            #region GET_DATA
            
            State = AIState.Attack;
            base.ai = ai;
            base.statesData = statesData;
            
            aiSharkBoss = ai.GetComponent<AISharkBoss>();
            animIsChargeAttack = aiSharkBoss.AnimIsChargeAttack;
            animIsRushAttack = aiSharkBoss.AnimIsRushAttack;
            animIsAttackSlam = aiSharkBoss.AnimIsAttackSlam;
            animNormalAttack = new EnemyAttackData(aiSharkBoss.AnimNormalAttack, EnemyAttackTypes.CanCharge);
            animSlamAttack = new EnemyAttackData(aiSharkBoss.AnimSlamAttack, EnemyAttackTypes.CanCharge);
            animRushAttack = new EnemyAttackData(aiSharkBoss.AnimRushAttack, EnemyAttackTypes.CanCharge);

            #endregion
        }
        
        public override void EnterState()
        {
            ai.Agent.ResetPath();
            ai.Agent.isStopped = true;
            speed = ai.Agent.speed;

            coroutineChargeAttack = ChargeAttack();
            coroutineDurationAttack = DurationAttack();
            coroutineSlamAttack = SlamAttack();
            coroutineRushAttack = RushAttack();
            coroutineMoveAttackTime = MoveAttackTime();
            coroutineLookTarget = LookTarget();
            
            RotationAttack();

            ai.IsAttackFinish = false;
            ai.IsFirstDefinitelyAttack = false;
            ai.CanAttack = false;
            isChargeAttack = true;
            isRushing = false;
            isLookTarget = false;
            moveSlam = false;

            AttackType();
            
            ai.Animator.SetBool(animIsChargeAttack, isChargeAttack);
            ai.Animator.SetBool(animIsRushAttack, true);
            ai.Animator.SetBool(animIsAttackSlam, true);
            ai.Animator.SetTrigger(animCurrent.Anim);
        }

        public override void UpdateState()
        {
            if (ai.IsDead)
            {
                tweenSequence.Pause();
                ai.NextState(statesData.Find(x => x.State == AIState.Dead));
            }

            if (moveSlam)
            {
                aiSharkBoss.Agent.SetDestination(aiSharkBoss.Target.transform.position);
            }
            
            if (ai.IsAttackFinish)
            {
                ChangeState();
            }
        }

        public override void ExitState()
        {
            ai.Agent.ResetPath();
            ai.Agent.isStopped = true;
            ai.StartCoroutine(coroutineDurationAttack);
            ai.CanAttack = false;
            ai.IsAttackState = false;
            ai.IsAttackNear = false;
            moveSlam = false;
            ai.Agent.speed = speed;
            ai.Animator.SetBool(animIsChargeAttack, false);
            ai.ResetColliderAttackAndVFX();
        }

        private void AttackType()
        {
            if (ai.IsAttackNear)
            {
                if (aiSharkBoss.CanRush && !aiSharkBoss.CanSlam)
                {
                    attackType = SharkBossAnimation.rushAttack;
                    animCurrent = animRushAttack;
                }
                else if (!aiSharkBoss.CanRush && aiSharkBoss.CanSlam)
                {
                    attackType = SharkBossAnimation.slamAttack;
                    animCurrent = animSlamAttack;
                }
                else if (!aiSharkBoss.CanRush && !aiSharkBoss.CanSlam)
                {
                    attackType = SharkBossAnimation.normalAttack;
                    animCurrent = animNormalAttack;
                }
                else if (aiSharkBoss.CanRush && aiSharkBoss.CanSlam)
                {
                    int attack = random.Next(2);
                    switch (attack)
                    {
                        case 0:
                            attackType = SharkBossAnimation.rushAttack;
                            animCurrent = animRushAttack;
                            break;
                        case 1:
                            attackType = SharkBossAnimation.slamAttack;
                            animCurrent = animSlamAttack;
                            break;
                    }
                }
            }
            else
            {
                attackType = SharkBossAnimation.rushAttack;
                animCurrent = animRushAttack;
            }
            
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
            aiSharkBoss.BlinkEffect.ContinuousBlinking();
            if (attackType == SharkBossAnimation.slamAttack)
            {
                aiSharkBoss.IndicatorSlamAttack.SetActive(true);
            }
            else if (attackType == SharkBossAnimation.rushAttack)
            {
                aiSharkBoss.IndicatorRushAttack.SetActive(true);
            }
            yield return new WaitForSeconds(aiSharkBoss.MaxChargeAttackDuration);
            isLookTarget = false;
            ai.Animator.SetBool(animIsChargeAttack, false);
            
            if (attackType == SharkBossAnimation.slamAttack)
            {
                aiSharkBoss.VfxSlamAttack.SetActive(true);
                aiSharkBoss.IndicatorSlamAttack.SetActive(false);
                aiSharkBoss.StartCoroutine(coroutineSlamAttack);
            }
            else if (attackType == SharkBossAnimation.rushAttack)
            {
                aiSharkBoss.IndicatorRushAttack.SetActive(false);
                aiSharkBoss.StartCoroutine(coroutineRushAttack);
            }
        }
        
        private IEnumerator SlamAttack()
        {
            aiSharkBoss.SlamCollider.RestCountStun();
            moveSlam = true;
            ai.Agent.isStopped = false;
            ai.Agent.speed = aiSharkBoss.MoveSpeedSlam;
            aiSharkBoss.PlaySfxSpin();
            yield return new WaitForSeconds(aiSharkBoss.SlamTime);
            var damageColliderSlam =
                aiSharkBoss.DamageColliders.Find(x => x.CollidersDamage == SharkBossCollidersDamage.SlamAttack);
            aiSharkBoss.StopSfxSpin();
            damageColliderSlam.gameObject.SetActive(false);
            ai.Animator.SetBool(animIsAttackSlam, false);
            aiSharkBoss.CoolDownSlam();
            moveSlam = false;
            ai.Agent.speed = speed;
            ai.Agent.ResetPath();
            ai.Agent.isStopped = true;
        }
        
        private IEnumerator RushAttack()
        {
            isRushing = true;
            aiSharkBoss.VfxRushAttack.SetActive(true);
            ai.StartCoroutine(coroutineMoveAttackTime);
            aiSharkBoss.SetColliderRush(true);
            yield return new WaitForSeconds(aiSharkBoss.RushTime);
            aiSharkBoss.VfxRushAttack.SetActive(false);
            aiSharkBoss.SetColliderRush(false);
            isRushing = false;
            ai.Animator.SetBool(animIsRushAttack, false);
            aiSharkBoss.CoolDownRush();
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

        private IEnumerator MoveAttackTime()
        {
            ai.ControllerRb.isKinematic = false;
            ai.EnemySetCollider(0);
            
            while (isRushing)
            {
                if (ai.IsDead)
                {
                    break;
                }
                
                if (!ai.PlayerData.IsUsingUI)
                {
                    var acceleration = aiSharkBoss.transform.forward.normalized *
                                       aiSharkBoss.MoveRushAttackAcceleration;

                    ai.ControllerRb.AddForce(acceleration, ForceMode.VelocityChange);
                }
                yield return null;
            }

            ai.ControllerRb.isKinematic = true;
            ai.EnemySetCollider(1);
        }
    }
}