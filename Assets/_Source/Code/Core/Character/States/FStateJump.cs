using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Gameplay.CommonObjects;
using SurgeEngine._Source.Code.Infrastructure.Config;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.States
{
    public class FStateJump : FStateAir
    {
        private PhysicsConfig _config;
        private readonly float _maxAirTime;

        private float _time;
        private bool _released;

        public FStateJump(CharacterBase owner) : base(owner)
        {
            _maxAirTime = 0.8f;
            _config = character.Config;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            ExecuteJump();

            _time = 0;

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

            if (Input.AReleased && !_released)
                _released = true;

            if (!character.Flags.HasFlag(FlagType.OutOfControl))
            {
                if (_time < _config.jumpMaxTime)
                    _time += dt;
                
                if (_released && _time < _config.jumpMaxTime)
                    _time = _config.jumpMaxTime;
            }
                
            if (Kinematics.AirTime > _maxAirTime)
            {
                StateMachine.SetState<FStateAir>();
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            Vector3 horizontal = Kinematics.HorizontalVelocity;
            Vector3 vertical = Kinematics.VerticalVelocity;
            if (_time < _config.jumpMaxTime)
            {
                Rigidbody.linearVelocity += Rigidbody.transform.up * (_config.jumpHoldSpeed * dt);
            }
            else
            {
                if (Rigidbody.linearVelocity.y > 0f)
                {
                    vertical.y /= 1.04f;
                    Rigidbody.linearVelocity = horizontal + vertical;
                }
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
                Rigidbody.AddForce(Rigidbody.transform.up * _config.jumpFirstSpeed, ForceMode.Impulse);
            }
            
            if (bounce)
                Rigidbody.linearVelocity = new Vector3(Rigidbody.linearVelocity.x, _config.jumpFirstSpeed, Rigidbody.linearVelocity.z);
        }
    }
}