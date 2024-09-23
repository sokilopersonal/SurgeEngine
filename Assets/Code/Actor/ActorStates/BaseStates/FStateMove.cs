using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public abstract class FStateMove : FActorState
    {
        protected Rigidbody _rigidbody { get; private set; }
        
        public override void OnEnter()
        {
            _rigidbody = actor.rigidbody;
            
            _rigidbody.solverIterations = 12;
            _rigidbody.solverVelocityIterations = 4;
        }
    }
}