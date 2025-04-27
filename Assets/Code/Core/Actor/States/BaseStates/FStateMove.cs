using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States.BaseStates
{
    public abstract class FStateMove : FActorState
    {
        protected Rigidbody _rigidbody { get; }

        public FStateMove(ActorBase owner, Rigidbody rigidbody) : base(owner)
        {
            _rigidbody = rigidbody;
            
            _rigidbody.solverIterations = 16;
            _rigidbody.solverVelocityIterations = 8;
        }
    }
}