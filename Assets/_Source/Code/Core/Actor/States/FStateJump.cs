using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects;
using SurgeEngine.Code.Infrastructure.Config;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateJump : FStateAir
    {
        private float _jumpTime;
        private BaseActorConfig _config;
        protected float _maxAirTime;

        public FStateJump(ActorBase owner) : base(owner)
        {
            _maxAirTime = 0.8f;
            _config = Actor.Config;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Rigidbody.linearVelocity += Rigidbody.transform.up * Mathf.Sqrt(_config.jumpForce * 2f * Kinematics.Gravity);
            _jumpTime = 0;
            
            Actor.transform.rotation = Quaternion.Euler(0, Actor.transform.rotation.eulerAngles.y, 0);
            
            Model.SetCollisionParam(_config.jumpCollisionHeight, _config.jumpCollisionCenter, _config.jumpCollisionRadius);
        }

        public override void OnExit()
        {
            Model.ResetCollisionToDefault();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (!Actor.Flags.HasFlag(FlagType.OutOfControl))
            {
                if (Input.AHeld)
                {
                    if (_jumpTime < _config.jumpStartTime)
                    {
                        if (Rigidbody.linearVelocity.y > 0) 
                            Rigidbody.linearVelocity += Actor.transform.up * (_config.jumpHoldForce * dt);
                        _jumpTime += dt;
                    }
                }
            }

            Kinematics.Normal = Vector3.up;

            if (Kinematics.AirTime > _maxAirTime)
            {
                StateMachine.SetState<FStateAir>();
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (Actor.Animation.StateAnimator.GetCurrentAnimationState() == "Ball" 
                && HurtBox.CreateAttached(Actor, Actor.transform, new Vector3(0f, -0.45f, 0f), new Vector3(0.6f, 0.6f, 0.6f), 
                    HurtBoxTarget.Enemy | HurtBoxTarget.Breakable))
            {
                StateMachine.SetState<FStateJump>(allowSameState: true);
            }
        }
    }
}