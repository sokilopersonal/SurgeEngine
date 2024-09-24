using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public abstract class FStateMove : FActorState
    {
        protected Rigidbody _rigidbody { get; private set; }
        
        public override void OnEnter()
        {
            _rigidbody = actor.rigidbody;
            
            _rigidbody.solverIterations = 16;
            _rigidbody.solverVelocityIterations = 8;
        }
    }
}