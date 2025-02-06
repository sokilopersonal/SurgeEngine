using System;
using System.Collections.Generic;
using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Config;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateDamage : FStateMove
    {
        private readonly DamageKickConfig _config;
        private Vector3 _direction;
        private float _timer;

        private List<string> _allowedDamageStates = new List<string>()
        {
            nameof(FStateGround),
            nameof(FStateAir),
            nameof(FStateBrake),
            nameof(FStateGrind),
            nameof(FStateGrindJump),
            nameof(FStateGrindSquat),
            nameof(FStateIdle),
            nameof(FStateBrakeTurn)
        };

        public FStateDamage(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            owner.TryGetConfig(out _config);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            StateMachine.GetSubState<FBoost>().Active = false;
            Common.ResetVelocity(ResetVelocityType.Both);
            _timer = 0.6f;
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            _rigidbody.linearVelocity = -Actor.transform.forward * _config.directionalForce;
            
            if (Common.CheckForGround(out var hit))
            {
                Kinematics.Snap(hit.point, hit.normal, true);
                
                if (Common.TickTimer(ref _timer, 0.6f, false))
                {
                    StateMachine.SetState<FStateDamageLand>();
                }
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
            
            Common.ApplyGravity(-Physics.gravity.y, dt);
        }

        public void SetDirection(Vector3 direction)
        {
            _direction = direction;
        }
        
        public List<string> GetAllowedDamageStates()
        {
            Debug.Log(_allowedDamageStates[0]);
            return _allowedDamageStates;
        }
    }
}