using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public abstract class FStateMove : FActorState
    {
        protected Rigidbody _rigidbody { get; private set; }
        
        public override void OnEnter()
        {
            if (_rigidbody == null)
            {
                _rigidbody = actor.GetComponent<Rigidbody>();

                _rigidbody.solverIterations = 12;
                _rigidbody.solverVelocityIterations = 4;
            }
        }
    }
}