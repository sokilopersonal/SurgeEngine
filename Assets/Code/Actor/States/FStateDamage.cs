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
        private float _timer;

        public FStateDamage(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            owner.TryGetConfig(out _config);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            StateMachine.GetSubState<FBoost>().Active = false;
            Common.ResetVelocity(ResetVelocityType.Both);
            _timer = 0.4f;
            
            _rigidbody.AddForce(-Actor.transform.forward * _config.directionalForce, ForceMode.VelocityChange);
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            Common.ApplyGravity(-Physics.gravity.y, dt);
            
            if (Common.CheckForGround(out var hit))
            {
                Kinematics.Snap(hit.point, hit.normal, true);
                _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0, _rigidbody.linearVelocity.z);
                
                if (Common.TickTimer(ref _timer, 0.6f, false))
                {
                    StateMachine.SetState<FStateDamageLand>();
                }
            }
        }
    }
}