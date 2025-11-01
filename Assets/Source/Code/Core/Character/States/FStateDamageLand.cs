using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public class FStateDamageLand : FCharacterState
    {
        private float _timer;
        
        public FStateDamageLand(CharacterBase owner) : base(owner)
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
                Kinematics.Snap(hit.point, Vector3.up);
            }
            
            if (Utility.TickTimer(ref _timer, 2f, false))
            {
                StateMachine.SetState<FStateIdle>();
            }
        }
    }
}