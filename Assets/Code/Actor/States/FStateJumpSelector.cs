using SurgeEngine.Code.Actor.States.BaseStates;
using SurgeEngine.Code.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Actor.States
{
    public class FStateJumpSelector : FStateMove
    {
        public FStateJumpSelector(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            
            _rigidbody.linearVelocity = Vector3.zero;
        }
    }
}