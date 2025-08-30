using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom;

namespace SurgeEngine.Code.Core.Actor.States
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
                Kinematics.Snap(hit.point, hit.normal);
            }
            
            if (Utility.TickTimer(ref _timer, 2f, false))
            {
                StateMachine.SetState<FStateIdle>();
            }
        }
    }
}