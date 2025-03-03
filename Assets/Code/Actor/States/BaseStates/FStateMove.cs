using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates.BaseStates
{
    public abstract class FStateMove : FActorState
    {
        protected Rigidbody _rigidbody { get; }

        public FStateMove(Actor owner, Rigidbody rigidbody) : base(owner)
        {
            _rigidbody = rigidbody;
            
            _rigidbody.solverIterations = 16;
            _rigidbody.solverVelocityIterations = 8;
        }
    }
}