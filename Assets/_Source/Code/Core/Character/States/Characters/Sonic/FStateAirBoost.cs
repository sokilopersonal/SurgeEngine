using SurgeEngine._Source.Code.Core.Character.States.BaseStates;
using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Infrastructure.Config.SonicSpecific;
using SurgeEngine._Source.Code.Infrastructure.Custom;

namespace SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic
{
    public class FStateAirBoost : FCharacterState
    {
        private float _timer;
        
        public FStateAirBoost(CharacterBase owner) : base(owner)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0.3f;
        }

        public override void OnExit()
        {
            base.OnExit();

            if (StateMachine.GetState(out FBoost boost))
            {
                boost.CanAirBoost = false;
                boost.Active = false;
            }
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            if (Utility.TickTimer(ref _timer, 0.3f, false))
            {
                StateMachine.SetState<FStateAir>();
            }

            if (Kinematics.CheckForGround(out _))
            {
                StateMachine.SetState<FStateGround>();
            }

            if (!character.Flags.HasFlag(FlagType.OutOfControl))
            {
                if (Input.BPressed)
                {
                    StateMachine.SetState<FStateStomp>();
                }
            }
        }
    }
}