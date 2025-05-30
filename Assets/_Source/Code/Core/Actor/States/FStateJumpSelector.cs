using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateJumpSelector : FActorState
    {
        public FStateJumpSelector(ActorBase owner) : base(owner)
        {
            
        }
        
        public override void OnEnter()
        {
            base.OnEnter();
            
            Rigidbody.linearVelocity = Vector3.zero;
        }
    }
}