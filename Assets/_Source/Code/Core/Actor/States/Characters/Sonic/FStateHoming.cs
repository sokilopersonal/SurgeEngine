using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Interfaces;
using SurgeEngine.Code.Gameplay.CommonObjects;
using SurgeEngine.Code.Infrastructure.Config;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic
{
    public class FStateHoming : FStateMove, IStateTimeout
    {
        private HomingTarget _target;
        private float _timer;

        private readonly HomingConfig _config;

        public FStateHoming(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            owner.TryGetConfig(out _config);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            StateMachine.GetSubState<FBoost>().Active = false;

            _timer = 0f;
            Timeout = _config.delay;

            BaseActorConfig config = Actor.config;
            Model.SetCollisionParam(config.jumpCollisionHeight, config.jumpCollisionCenter, config.jumpCollisionRadius);
        }

        public override void OnExit()
        {
            base.OnExit();

            Model.ResetCollisionToDefault();
            _target = null;
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (_target != null)
            {
                Vector3 direction = (_target.transform.position - Actor.transform.position + Actor.transform.up * 0.5f).normalized;
                _rigidbody.linearVelocity = direction * _config.speed;
                _rigidbody.rotation = Quaternion.LookRotation(direction, Vector3.up);
                
                float distance = Vector3.Distance(Actor.transform.position, _target.transform.position);
                if (distance <= 0.7f)
                {
                    _target.OnTargetReached.Invoke();
                }
                
                // If for some reason Sonic get stuck
                _timer += dt / _config.maxTime;
                if (_timer >= 1f)
                {
                    Debug.Log("Stuck");
                    StateMachine.SetState<FStateAir>();
                }
            }
            else
            {
                if (Kinematics.CheckForGround(out _, CheckGroundType.PredictJump))
                {
                    //StateMachine.SetState<FStateAir>();
                }
                
                Vector3 direction = Vector3.Cross(Actor.transform.right, Vector3.up);
                _rigidbody.linearVelocity = direction * (_config.jumpDashDistance *
                                                         _config.JumpDashCurve.Evaluate(_timer));
                _rigidbody.rotation = Quaternion.LookRotation(direction, Vector3.up);
                
                _timer += dt / _config.jumpDashTime;
                if (_timer >= 1f)
                {
                    Debug.Log("Over time");
                    StateMachine.SetState<FStateAir>();
                }
            }
            
            bool foundDamageable = HurtBox.CreateAttached(Actor, Actor.transform, Vector3.one * 0.5f, HurtBoxTarget.Enemy | HurtBoxTarget.Breakable);
            if (foundDamageable)
            {
                StateMachine.SetState<FStateAfterHoming>();
            }
        }

        public void SetTarget(HomingTarget target)
        {
            _target = target;
        }

        public float Timeout { get; set; }
    }
}