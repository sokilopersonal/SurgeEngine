using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateUpreel : FActorState
    {
        private Transform _attach;
        
        public FStateUpreel(ActorBase owner) : base(owner)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Rigidbody.linearVelocity = Vector3.zero;
            Rigidbody.isKinematic = true;
        }

        public override void OnExit()
        {
            base.OnExit();

            Rigidbody.isKinematic = false;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
        }

        public void SetAttach(Transform attach)
        {
            _attach = attach;
        }
    }
}