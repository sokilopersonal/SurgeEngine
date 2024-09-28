using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters.SonicSubStates;
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
            stateMachine.GetSubState<FBoost>().Active = false;

            _timer = 0;
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
    
            if (Common.CheckForGround(out var hit))
            {
                animation.ResetAction();
                
                stats.groundAngle = Vector3.Angle(hit.normal, Vector3.up);
                
                var point = hit.point;
                var normal = hit.normal;

                _rigidbody.position = point + normal;
                _rigidbody.linearVelocity = Vector3.zero;

                if (!Mathf.Approximately(stats.groundAngle, 0))
                {
                    stateMachine.SetState<FStateGround>();
                }
                else
                {
                    Debug.Log("hey");
                    stateMachine.SetState<FStateIdle>();
                }
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