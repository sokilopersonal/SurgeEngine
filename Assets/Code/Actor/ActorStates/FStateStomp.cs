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
            
            stateMachine.GetSubState<FBoost>().Active = false;
            _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);

            _timer = 0;
        }

        public override void OnExit()
        {
            base.OnExit();
            
            animation.ResetAction();
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

            float horizontalSpeedMultiplier = stats.stompParameters.stompCurve.Evaluate(_timer);
            Vector3 smoothedXZVelocity = new Vector3(velocity.x * horizontalSpeedMultiplier, velocity.y, velocity.z * horizontalSpeedMultiplier);
            
            float stompSpeed = -stats.stompParameters.stompSpeed;
            
            float minYVelocity = stompSpeed * 1.25f;
            float maxYVelocity = 5f;

            velocity = new Vector3(smoothedXZVelocity.x, 
                Mathf.Clamp(velocity.y + stompSpeed, minYVelocity, maxYVelocity), 
                smoothedXZVelocity.z);

            _rigidbody.linearVelocity = velocity;
            
            _timer += dt;
        }
    }
}