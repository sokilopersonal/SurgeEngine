using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects;
using SurgeEngine.Code.Infrastructure.Config;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateJump : FStateAir
    {
        private float _jumpTime;
        private PhysicsConfig _config;
        private readonly float _maxAirTime;
        private Vector3 _lastNormal;

        private bool _released;

        public FStateJump(ActorBase owner) : base(owner)
        {
            _maxAirTime = 0.8f;
            _config = Actor.Config;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _lastNormal = Kinematics.Normal;
            ExecuteJump();

            _jumpTime = 0;
            Kinematics.Normal = Vector3.up;
            
            Vector3 currentRotation = Kinematics.Rigidbody.rotation.eulerAngles;
            Vector3 newRotation = new Vector3(0f, currentRotation.y, 0f);
            Kinematics.Rigidbody.rotation = Quaternion.Euler(newRotation);
            
            Model.SetCollisionParam(_config.jumpCollisionHeight, _config.jumpCollisionCenter, _config.jumpCollisionRadius);
        }

        public override void OnExit()
        {
            Model.ResetCollisionToDefault();

            _released = false;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            VariableJumpHeight(dt);

            if (Kinematics.AirTime > _maxAirTime)
            {
                StateMachine.SetState<FStateAir>();
            }
        }

        private void VariableJumpHeight(float dt)
        {
            if (Input.AReleased && !_released)
                _released = true;

            if (Actor.Flags.HasFlag(FlagType.OutOfControl)) return;
            if (!Input.AHeld || _released) return;
            if (!(_jumpTime < _config.jumpStartTime)) return;

            Vector3 horizontal = Kinematics.HorizontalVelocity;
            Vector3 vertical = Kinematics.VerticalVelocity;
                
            vertical.y += _config.jumpHoldForce * dt;
            vertical.y = Mathf.Min(vertical.y, _config.jumpMaxSpeed);
            
            Rigidbody.linearVelocity = horizontal + vertical;
            
            _jumpTime += dt;
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            float drag = 1f;
            Vector3 horizontal = Kinematics.HorizontalVelocity;
            Vector3 vertical = Kinematics.VerticalVelocity;

            if (_jumpTime > 0.128f)
            {
                horizontal *= Mathf.Exp(-drag * dt);
                Rigidbody.linearVelocity = horizontal + vertical;
            }
            
            if (Actor.Animation.StateAnimator.GetCurrentAnimationState() == "Ball" 
                && HurtBox.CreateAttached(Actor, Actor.transform, new Vector3(0f, -0.45f, 0f), new Vector3(0.6f, 0.6f, 0.6f), 
                    HurtBoxTarget.Enemy | HurtBoxTarget.Breakable))
            {
                ExecuteJump(true);
            }
        }

        private void ExecuteJump(bool bounce = false)
        {
            if (!bounce)
            {
                Vector3 horizontalVelocity = new Vector3(Rigidbody.linearVelocity.x, 0, Rigidbody.linearVelocity.z);
                Vector3 jumpVelocity = _lastNormal * _config.jumpForce;
                Rigidbody.linearVelocity = horizontalVelocity + jumpVelocity;
            }
            
            if (bounce)
                Rigidbody.linearVelocity = new Vector3(Rigidbody.linearVelocity.x, _config.jumpForce * 4, Rigidbody.linearVelocity.z);
        }
    }
}