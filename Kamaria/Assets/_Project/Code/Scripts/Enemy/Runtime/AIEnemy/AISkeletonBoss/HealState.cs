using System.Collections.Generic;
using Kamaria.Enemy.AIEnemy.Fish;
using Kamaria.Enemy.AIEnemy.Golem;
using Kamaria.Enemy.AIEnemy.Mouse;
using Kamaria.Item;
using Kamaria.Player.Data;
using UnityEngine;

namespace Kamaria.Enemy.AIEnemy.SkeletonBoss
{
    public sealed class HealState : BaseAIFiniteStateMachine
    {
        private AISkeletonBoss aiSkeletonBoss;
        private List<AIFish> aiFishes = new List<AIFish>();
        private List<AIMouse> aiMouses = new List<AIMouse>();
        private List<AIGolem> aiGolems = new List<AIGolem>();
        
        public HealState(BaseAI ai, List<BaseAIFiniteStateMachine> statesData)
        {
            #region GET_DATA

            State = AIState.Heal;
            base.ai = ai;
            base.statesData = statesData;
            
            aiSkeletonBoss = ai.GetComponent<AISkeletonBoss>();

            #endregion
        }
        
        public override void EnterState()
        {
            aiFishes.Clear();
            aiMouses.Clear();
            aiGolems.Clear();
            
            aiSkeletonBoss.Agent.ResetPath();
            aiSkeletonBoss.Agent.isStopped = true;
            aiSkeletonBoss.Animator.SetTrigger(aiSkeletonBoss.AnimHeal);
            GetEnemySummon();
            
            Heal();
        }

        public override void UpdateState()
        {
            if (aiSkeletonBoss.HealFinish)
            {
                ai.NextState(statesData.Find(x => x.State == AIState.Chase));
            }
        }

        public override void ExitState()
        {
            aiSkeletonBoss.Agent.ResetPath();
            aiSkeletonBoss.Agent.isStopped = true;
        }
        
        private void Heal()
        {
            int heal = ((aiSkeletonBoss.EnemyStatusData.MaxHealth *
                                (aiSkeletonBoss.PercentHealFixed + 
                                 (aiSkeletonBoss.CountSummonLife() * aiSkeletonBoss.PercentHealSummonLife))) / 100);
            
            if (aiSkeletonBoss.EnemyStatusData.CurrentHealth + heal > aiSkeletonBoss.EnemyStatusData.MaxHealth)
            {
                aiSkeletonBoss.EnemyStatusData.CurrentHealth = aiSkeletonBoss.EnemyStatusData.MaxHealth;
            }
            else
            {
                aiSkeletonBoss.EnemyStatusData.CurrentHealth += heal;
            }
            
            KillEnemySummon();
            
            aiSkeletonBoss.HealthBar.ShowBar(aiSkeletonBoss.EnemyStatusData.CurrentHealth);
        }

        private void GetEnemySummon()
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