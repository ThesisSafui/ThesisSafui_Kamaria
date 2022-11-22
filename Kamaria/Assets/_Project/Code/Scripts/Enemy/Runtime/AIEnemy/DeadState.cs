using System.Collections;
using System.Collections.Generic;
using Kamaria.Enemy.AIEnemy.Fish;
using Kamaria.Enemy.AIEnemy.Golem;
using Kamaria.Enemy.AIEnemy.Mouse;
using Kamaria.Enemy.AIEnemy.SharkBoss;
using Kamaria.Enemy.AIEnemy.SkeletonBoss;
using Kamaria.Item;
using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Enemy.AIEnemy
{
    public class DeadState : BaseAIFiniteStateMachine
    {
        private IEnumerator coroutineDead;
        private List<AIFish> aiFishes = new List<AIFish>();
        private List<AIMouse> aiMouses = new List<AIMouse>();
        private List<AIGolem> aiGolems = new List<AIGolem>();
        
        #region ENEMIES

        private AIMouse aiMouse;
        private AIGolem aiGolem;
        private AIFish aiFish;
        private AISharkBoss aiSharkBoss;
        private AISkeletonBoss aiSkeletonBoss;

        #endregion
        
        public DeadState(BaseAI ai, List<BaseAIFiniteStateMachine> statesData)
        {
            State = AIState.Dead;
            base.ai = ai;
            base.statesData = statesData;
            
            if (ai.EnemyType.Equals(Enemies.Mouse))
            {
                aiMouse = ai.GetComponent<AIMouse>();
            }
            else if (ai.EnemyType.Equals(Enemies.Golem))
            {
                aiGolem = ai.GetComponent<AIGolem>();
            }
            else if (ai.EnemyType.Equals(Enemies.Fish))
            {
                aiFish = ai.GetComponent<AIFish>();
            }
            else if (ai.EnemyType.Equals(Enemies.SharkBoss))
            {
                aiSharkBoss = ai.GetComponent<AISharkBoss>();
            }
            else if (ai.EnemyType.Equals(Enemies.SkeletonBoss))
            {
                aiSkeletonBoss = ai.GetComponent<AISkeletonBoss>();
            }
        }
        public override void EnterState()
        {
            aiFishes.Clear();
            aiMouses.Clear();
            aiGolems.Clear();
            
            ai.ResetColliderAttackAndVFX();
            ai.Agent.ResetPath();
            ai.Agent.isStopped = true;
            coroutineDead = DeadTime();
            ai.StartCoroutine(coroutineDead);
            ai.DropItem();

            if (ai.EnemyType is Enemies.SharkBoss or Enemies.SkeletonBoss)
            {
                switch (ai.EnemyType)
                {
                    case Enemies.SharkBoss:
                        aiSharkBoss.UIFinish.gameObject.SetActive(true);
                        break;
                    case Enemies.SkeletonBoss:
                        aiSkeletonBoss.UIFinish.gameObject.SetActive(true);
                        break;
                }
                GetEnemySummon();
                KillEnemySummon();
            }

            ai.StopSfx();
        }

        public override void UpdateState()
        {
        }

        public override void ExitState()
        {
        }

        private IEnumerator DeadTime()
        {
            yield return new WaitForSeconds(4);
            ai.Agent.ResetPath();
            ai.gameObject.SetActive(false);
        }
        
        private void GetEnemySummon()
        {
            switch (ai.EnemyType)
            {
                case Enemies.SkeletonBoss:
                {
                    for (int i = 0; i < aiSkeletonBoss.EnemySummon.Count; i++)
                    {
                        if (aiSkeletonBoss.EnemySummon[i].gameObject.TryGetComponent(out AIFish aiFish))
                        {
                            aiFishes.Add(aiFish);
                        }
                        else if (aiSkeletonBoss.EnemySummon[i].gameObject.TryGetComponent(out AIGolem aiGolem))
                        {
                            aiGolems.Add(aiGolem);
                        }
                        else if (aiSkeletonBoss.EnemySummon[i].gameObject.TryGetComponent(out AIMouse aiMouse))
                        {
                            aiMouses.Add(aiMouse);
                        }
                    }

                    break;
                }
                case Enemies.SharkBoss:
                {
                    for (int i = 0; i < aiSharkBoss.EnemySummon.Count; i++)
                    {
                        if (aiSharkBoss.EnemySummon[i].gameObject.TryGetComponent(out AIFish aiFish))
                        {
                            aiFishes.Add(aiFish);
                        }
                        else if (aiSharkBoss.EnemySummon[i].gameObject.TryGetComponent(out AIGolem aiGolem))
                        {
                            aiGolems.Add(aiGolem);
                        }
                        else if (aiSharkBoss.EnemySummon[i].gameObject.TryGetComponent(out AIMouse aiMouse))
                        {
                            aiMouses.Add(aiMouse);
                        }
                    }

                    break;
                }
            }
        }

        private void KillEnemySummon()
        {
            if (aiFishes.Count != 0)
            {
                for (int i = 0; i < aiFishes.Count; i++)
                {
                    aiFishes[i].TakeDamage(99999, EffectAttack.None, Vector3.zero, 0, 0, false, 0, WeaponTypes.None,
                        KeyStones.None);
                }
            }
            
            if (aiGolems.Count != 0)
            {
                for (int i = 0; i < aiGolems.Count; i++)
                {
                    aiGolems[i].TakeDamage(99999, EffectAttack.None, Vector3.zero, 0, 0, false, 0, WeaponTypes.None,
                        KeyStones.None);
                }
            }
            
            if (aiMouses.Count != 0)
            {
                for (int i = 0; i < aiMouses.Count; i++)
                {
                    aiMouses[i].TakeDamage(99999, EffectAttack.None, Vector3.zero, 0, 0, false, 0, WeaponTypes.None,
                        KeyStones.None);
                }
            }
        }
    }
}