using UnityEngine;
using UnityEngine.AI;

namespace SurgeEngine.Code.StateMachine
{
    public class FEState : FState
    {
        protected Transform transform;
        protected Rigidbody Rb;

        public FEState(Transform transform, Rigidbody rb)
        {
            this.transform = transform;
            this.Rb = rb;
        }
    }
}