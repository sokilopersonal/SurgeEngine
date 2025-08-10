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
        private Vector3 _jumpVelocity;
        private Vector3 _jumpNormal;

        private bool _released;
        private bool _reachedMaxHeight;

        public FStateJump(CharacterBase owner) : base(owner)
        {
            _maxAirTime = 0.8f;
            _config = character.Config;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _jumpNormal = Vector3.up;
            _jumpVelocity = Vector3.zero;
            
            _reachedMaxHeight = false;

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

            if (character.Flags.HasFlag(FlagType.OutOfControl) || !Input.AHeld || _released)
                return;
            
            _jumpTime += dt;

            if (_jumpTime < _config.jumpMaxShortTime)
                return;

            Vector3 horizontal = Kinematics.HorizontalVelocity;
            Vector3 vertical = Kinematics.VerticalVelocity;

            if (vertical.y > 0f && vertical.magnitude < _config.jumpMaxSpeed && !_reachedMaxHeight)
            {
                vertical += _jumpNormal * (_config.jumpHoldSpeed * dt);
            }
            else
            {
                _reachedMaxHeight = true;
            }

            Rigidbody.linearVelocity = horizontal + vertical;
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            float drag = _config.jumpDrag;
            Vector3 horizontal = Kinematics.HorizontalVelocity;
            Vector3 vertical = Kinematics.VerticalVelocity;

            if (_jumpTime > _config.jumpMaxShortTime)
            {
                horizontal *= Mathf.Exp(-drag * dt);
                Rigidbody.linearVelocity = horizontal + vertical;
            }
            
            if (character.Animation.StateAnimator.GetCurrentAnimationState() == "Ball" 
                && HurtBox.CreateAttached(character, character.transform, new Vector3(0f, -0.45f, 0f), new Vector3(0.6f, 0.6f, 0.6f), 
                    HurtBoxTarget.Enemy | HurtBoxTarget.Breakable))
            {
                ExecuteJump(true);
            }
        }

        private void ExecuteJump(bool bounce = false)
        {
            if (!bounce)
            {
                Vector3 horizontalVelocity = Kinematics.HorizontalVelocity;
                _jumpVelocity = _jumpNormal * _config.jumpFirstSpeed;
                
                Rigidbody.linearVelocity = horizontalVelocity + _jumpVelocity;
            }
            
            if (bounce)
                Rigidbody.linearVelocity = new Vector3(Rigidbody.linearVelocity.x, _config.jumpFirstSpeed, Rigidbody.linearVelocity.z);
        }
    }
}