using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Config;
using UnityEngine;

namespace SurgeEngine.Code.Actor.States
{
    public class FStateJump : FStateAir
    {
        private float _jumpTime;
        private BaseActorConfig _config;
        protected float _maxAirTime;

        public FStateJump(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            _maxAirTime = 0.8f;
            _config = Actor.config;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Vector3 horizontalVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
            Vector3 jumpVelocity = Kinematics.Normal * _config.jumpForce;
            _rigidbody.linearVelocity = horizontalVelocity + jumpVelocity;
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

            if (!Actor.flags.HasFlag(FlagType.OutOfControl))
            {
                if (Input.JumpHeld)
                {
                    if (_jumpTime < _config.jumpStartTime)
                    {
                        if (_rigidbody.linearVelocity.y > 0) 
                            _rigidbody.linearVelocity += Actor.transform.up * (_config.jumpHoldForce * dt);
                        _jumpTime += dt;
                    }
                }
            }

            Kinematics.Normal = Vector3.up;

            if (GetAirTime() > _maxAirTime)
            {
                StateMachine.SetState<FStateAir>();
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (Actor.animation.StateAnimator.GetCurrentAnimationState() == "Ball" 
                && HurtBox.Create(Actor, Actor.transform.position + new Vector3(0f, -0.45f, 0f), Actor.transform.rotation, new Vector3(0.6f, 0.6f, 0.6f), 
                    HurtBoxTarget.Enemy | HurtBoxTarget.Breakable))
            {
                StateMachine.SetState<FStateJump>(allowSameState: true);
            }
        }
    }
}