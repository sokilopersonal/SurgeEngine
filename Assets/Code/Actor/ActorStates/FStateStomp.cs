using System.Numerics;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters.SonicSubStates;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

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

            _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);

            _timer = 0;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            if (Common.CheckForGround(out var hit))
            {
                animation.ResetAction();
                
                stats.groundAngle = Vector3.Angle(hit.normal, Vector3.up);
                
                var point = hit.point;
                var normal = hit.normal;

                _rigidbody.position = point + normal;
                _rigidbody.linearVelocity = Vector3.zero;

                stateMachine.SetState<FStateIdle>();
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            Vector3 velocity = _rigidbody.linearVelocity;

            float smoothingFactor = 1f;
            Vector3 xzVelocity = new Vector3(velocity.x, 0, velocity.z);
            Vector3 smoothedXZVelocity = Vector3.Lerp(xzVelocity, Vector3.zero, smoothingFactor * dt);

            float stompSpeed = -stats.stompParameters.stompSpeed;
            
            float minYVelocity = -40f;
            float maxYVelocity = 10f;

            velocity = new Vector3(smoothedXZVelocity.x, 
                Mathf.Clamp(velocity.y + stompSpeed, minYVelocity, maxYVelocity), 
                smoothedXZVelocity.z);

            _rigidbody.linearVelocity = velocity;

        }

    }
}