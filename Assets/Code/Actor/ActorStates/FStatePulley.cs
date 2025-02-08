using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    public class FStatePulley : FStateMove
    {
        private Transform _attach;
        
        public FStatePulley(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
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
            
            if (_attach)
            {
                _rigidbody.transform.position = _attach.position;
                _rigidbody.transform.rotation = _attach.rotation;
            }
        }

        public void SetAttach(Transform attach)
        {
            _attach = attach;
        }
    }
}