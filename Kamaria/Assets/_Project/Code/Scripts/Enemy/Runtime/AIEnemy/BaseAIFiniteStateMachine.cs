using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Kamaria.Enemy.AIEnemy
{
    public enum AIState
    {
        Patrol,
        Chase,
        Alert,
        Attack,
        Escape,
        Dead,
        Stun,
        Summon,
        Heal
    }
    
    public abstract class BaseAIFiniteStateMachine
    {
        public AIState State;
        
        protected BaseAI ai;
        protected List<BaseAIFiniteStateMachine> statesData;
        
        /// <summary>
        /// Do the enter state process.
        /// </summary>
        public abstract void EnterState();
        
        /// <summary>
        /// Do the update state process.
        /// </summary>
        public abstract void UpdateState();
        
        /// <summary>
        /// Do the exit state process.
        /// </summary>
        public abstract void ExitState();
    }
}