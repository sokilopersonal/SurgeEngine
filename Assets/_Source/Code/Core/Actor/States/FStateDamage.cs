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
    
    public class FStateDamage : FCharacterState
    {
        private readonly DamageKickConfig _config;
        private float _timer;

        private DamageState _state;
        public DamageState State => _state;

        public FStateDamage(CharacterBase owner) : base(owner)
        {
            owner.TryGetConfig(out _config);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            StateMachine.GetSubState<FBoost>().Active = false;
            Kinematics.ResetVelocity();
            _timer = 0.4f;
            
            Rigidbody.AddForce(-character.transform.forward * _config.directionalForce, ForceMode.VelocityChange);
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
                Rigidbody.linearVelocity = new Vector3(Rigidbody.linearVelocity.x, 0, Rigidbody.linearVelocity.z);
                
                if (Utility.TickTimer(ref _timer, 0.6f, false))
                {
                    if (_state == DamageState.Alive)
                    {
                        StateMachine.SetState<FStateDamageLand>();
                    }
                    else
                    {
                        Rigidbody.linearVelocity = Vector3.zero;
                    }
                }
                
                Model.RotateBody(-Rigidbody.GetHorizontalVelocity(), hit.normal);
            }
        }
        
        public void SetState(DamageState state) => _state = state;
    }
}