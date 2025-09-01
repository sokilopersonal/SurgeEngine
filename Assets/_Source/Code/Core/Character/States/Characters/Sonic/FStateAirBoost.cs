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
        private readonly BoostConfig _config;
        
        public FStateAirBoost(CharacterBase owner) : base(owner)
        {
            owner.TryGetConfig(out _config);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0.3f;
        }

        public override void OnExit()
        {
            base.OnExit();
            
            FBoost boost = StateMachine.GetSubState<FBoost>();
            boost.CanAirBoost = false;
            boost.Active = false;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            FBoost boost = StateMachine.GetSubState<FBoost>();
            if (boost.CanAirBoost)
            {
                
            }
            
            if (Utility.TickTimer(ref _timer, 0.3f))
            {
                StateMachine.SetState<FStateAir>();
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