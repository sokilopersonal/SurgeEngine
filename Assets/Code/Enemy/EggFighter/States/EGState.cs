using SurgeEngine.Code.StateMachine;
using UnityEngine;
using UnityEngine.AI;

namespace SurgeEngine.Code.Enemy.States
{
    public class EGState : FEState
    {
        protected EggFighter eggFighter;
        
        public EGState(EggFighter eggFighter, Transform transform, NavMeshAgent agent) : base(transform, agent)
        {
            this.eggFighter = eggFighter;
        }
    }
}