using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Config;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateJump : FStateAir
    {
        private float _jumpTime;
        private BaseActorConfig _config;
        protected float _maxAirTime;

        public FStateJump(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            _maxAirTime = 0.8f;
            _config = Actor.config;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _rigidbody.AddForce(Actor.transform.up * _config.jumpForce, ForceMode.Impulse);
            _jumpTime = 0;
            
            Actor.transform.rotation = Quaternion.Euler(0, Actor.transform.rotation.eulerAngles.y, 0);
            
            Actor.model.SetCollisionParam(_config.jumpCollisionHeight, _config.jumpCollisionCenter, _config.jumpCollisionRadius);

        }

        public override void OnExit()
        {
            Animation.ResetAction();
            Actor.model.SetCollisionParam(0,0);
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
    }
}