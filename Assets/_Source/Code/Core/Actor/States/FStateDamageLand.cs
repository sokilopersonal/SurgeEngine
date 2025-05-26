using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateDamageLand : FStateMove
    {
        private float _timer;
        
        public FStateDamageLand(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 2f;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            Kinematics.ResetVelocity();
            
            if (Kinematics.CheckForGround(out var hit))
            {
                Kinematics.Snap(hit.point, hit.normal, true);
            }
            
            if (Common.TickTimer(ref _timer, 2f, false))
            {
                StateMachine.SetState<FStateIdle>();
            }
        }
    }
}