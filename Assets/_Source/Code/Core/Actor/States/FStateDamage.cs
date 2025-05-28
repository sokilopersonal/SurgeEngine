using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Config;
using SurgeEngine.Code.Infrastructure.Custom;
using SurgeEngine.Code.Infrastructure.Custom.Extensions;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public enum DamageState
    {
        Alive,
        Dead
    }
    
    public class FStateDamage : FStateMove
    {
        private readonly DamageKickConfig _config;
        private float _timer;

        private DamageState _state;
        public DamageState State => _state;

        public FStateDamage(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            owner.TryGetConfig(out _config);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            StateMachine.GetSubState<FBoost>().Active = false;
            Kinematics.ResetVelocity();
            _timer = 0.4f;
            
            _rigidbody.AddForce(-Actor.transform.forward * _config.directionalForce, ForceMode.VelocityChange);
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            Kinematics.ApplyGravity(-Physics.gravity.y);
            
            if (Kinematics.CheckForGround(out var hit))
            {
                Kinematics.Point = hit.point;
                Kinematics.Normal = hit.normal;
                
                Kinematics.Snap(hit.point, hit.normal, true);
                Kinematics.Project();
                _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
                
                if (Utility.TickTimer(ref _timer, 0.6f, false))
                {
                    if (_state == DamageState.Alive)
                    {
                        StateMachine.SetState<FStateDamageLand>();
                    }
                    else
                    {
                        _rigidbody.linearVelocity = Vector3.zero;
                    }
                }
                
                Model.RotateBody(-_rigidbody.GetHorizontalVelocity(), hit.normal);
            }
        }
        
        public void SetState(DamageState state) => _state = state;
    }
}