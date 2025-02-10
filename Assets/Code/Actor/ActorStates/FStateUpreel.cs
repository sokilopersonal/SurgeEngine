using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateUpreel : FStateMove
    {
        private Transform _attach;
        
        public FStateUpreel(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
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

            if (Input.JumpPressed)
            {
                StateMachine.SetState<FStateJump>();
            }
        }

        public void SetAttach(Transform attach)
        {
            _attach = attach;
        }
    }
}