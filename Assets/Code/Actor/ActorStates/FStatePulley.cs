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

            _rigidbody.interpolation = RigidbodyInterpolation.None; // For some reason the player lags behind the pulley with interpolation
            _rigidbody.linearVelocity = Vector3.zero;
        }

        public override void OnExit()
        {
            base.OnExit();

            _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (Input.JumpPressed)
            {
                StateMachine.SetState<FStateJump>();
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (_attach)
            {
                _rigidbody.Move(_attach.position, _attach.rotation);
            }
        }

        public void SetAttach(Transform attach)
        {
            _attach = attach;
        }
    }
}