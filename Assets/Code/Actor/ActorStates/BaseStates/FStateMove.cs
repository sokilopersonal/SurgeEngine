using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public abstract class FStateMove : FActorState
    {
        public FStateMove(Actor owner, Rigidbody rigidbody) : base(owner)
        {
            _rigidbody = rigidbody;
        }

        protected Rigidbody _rigidbody { get; private set; }

        public override void OnEnter()
        {
            _rigidbody = Actor.rigidbody; 
            
            _rigidbody.solverIterations = 16;
            _rigidbody.solverVelocityIterations = 8;
        }
    }
}