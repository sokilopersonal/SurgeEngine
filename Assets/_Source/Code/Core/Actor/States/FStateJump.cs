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
        protected float _maxAirTime;

        private bool _released;

        public FStateJump(ActorBase owner) : base(owner)
        {
            _maxAirTime = 0.8f;
            _config = Actor.Config;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
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
            
            if (Kinematics.Velocity.y > 0 || Kinematics.Speed >= Actor.Config.topSpeed / 2) 
                Rigidbody.linearVelocity += Actor.transform.up * (_config.jumpHoldForce * dt);
            _jumpTime += dt;
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            SurgeMath.SplitPlanarVector(Kinematics.Velocity, Vector3.up, out var planar, out var vertical);
            var physicsConfig = Actor.Config;
            vertical.y = Mathf.Clamp(vertical.y, -physicsConfig.minVerticalSpeed, physicsConfig.maxVerticalSpeed);
            Rigidbody.linearVelocity = planar + vertical;
            
            if (Actor.Animation.StateAnimator.GetCurrentAnimationState() == "Ball" 
                && HurtBox.CreateAttached(Actor, Actor.transform, new Vector3(0f, -0.45f, 0f), new Vector3(0.6f, 0.6f, 0.6f), 
                    HurtBoxTarget.Enemy | HurtBoxTarget.Breakable))
            {
                ExecuteJump(true);
            }
        }

        private void ExecuteJump(bool bounce = false)
        {
            Rigidbody.linearVelocity += Actor.transform.up * (_config.jumpForce * 2f * (!bounce ? 1f : _config.jumpHoldForce / 2));
        }
    }
}