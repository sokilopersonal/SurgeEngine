using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public class FStateStomp : FStateMove
    {
        private float _timer;

        public override void OnEnter()
        {
            base.OnEnter();
            
            animation.TransitionToState("Stomp", 0, true);

            _timer = 0;
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
    
            if (Common.CheckForGround(out var hit))
            {
                animation.ResetAction();
                
                
                stateMachine.SetState<FStateIdle>();
                
                var point = hit.point;
                var normal = hit.normal;

                _rigidbody.position = point + normal;
                _rigidbody.linearVelocity = Vector3.zero;
            }

            _timer += dt;

            if (_timer > 0.5f)
            {
                _timer = 0.5f;
            }
            
            Vector3 downVelocity = Vector3.down * stats.stompParameters.stompSpeed;
            _rigidbody.linearVelocity += downVelocity;
            _rigidbody.linearVelocity = Vector3.ClampMagnitude(_rigidbody.linearVelocity, stats.stompParameters.stompSpeed);
        }

    }
}