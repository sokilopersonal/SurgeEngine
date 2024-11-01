using UnityEngine;
using UnityEngine.AI;

namespace SurgeEngine.Code.StateMachine
{
    public class FEState : FState
    {
        protected Transform transform;
        protected NavMeshAgent agent;

        public FEState(Transform transform, NavMeshAgent agent)
        {
            this.transform = transform;
            this.agent = agent;
        }
    }
}