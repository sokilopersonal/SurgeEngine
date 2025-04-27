using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateUpreel : FStateMove
    {
        private Transform _attach;
        
        public FStateUpreel(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _rigidbody.linearVelocity = Vector3.zero;
            _rigidbody.isKinematic = true;
        }

        public override void OnExit()
        {
            base.OnExit();

            _rigidbody.isKinematic = false;
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