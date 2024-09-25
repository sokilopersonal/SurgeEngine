using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public class FStateJump : FStateAir
    {
        private float _jumpTime;
        
        public override void OnEnter()
        {
            base.OnEnter();
            
            animation.TransitionToState("Ball", 0f, true);
            _rigidbody.AddForce(actor.transform.up * stats.jumpParameters.jumpForce, ForceMode.Impulse);
            _jumpTime = stats.jumpParameters.jumpStartTime;
            
            actor.transform.rotation = Quaternion.Euler(0, actor.transform.rotation.eulerAngles.y, 0);
        }

        public override void OnExit()
        {
            base.OnExit();
            
            animation.ResetAction();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (!actor.flags.HasFlag(FlagType.OutOfControl))
            {
                if (input.JumpHeld)
                {
                    if (_jumpTime > 0)
                    {
                        if (_rigidbody.linearVelocity.y > 0) 
                            _rigidbody.linearVelocity += actor.transform.up * (stats.jumpParameters.jumpHoldForce * dt);
                        _jumpTime -= dt;
                    }
                }
            }
            
            stats.transformNormal = Vector3.up;

            Vector3 vel = _rigidbody.linearVelocity;
            vel = Vector3.ProjectOnPlane(vel, Vector3.up);

            if (vel.magnitude > 0.1f)
            {
                Quaternion rot = Quaternion.LookRotation(vel, stats.transformNormal);
                actor.transform.rotation = rot;
            }

            if (GetAirTime() > 0.85f)
            {
                stateMachine.SetState<FStateAir>();
            }
        }
    }
}