using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic
{
    public class FStateAirBoost : FActorState
    {
        private float _timer;
        private readonly BoostConfig _config;
        
        public FStateAirBoost(ActorBase owner) : base(owner)
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

            if (!Actor.Flags.HasFlag(FlagType.OutOfControl))
            {
                if (Input.BPressed)
                {
                    StateMachine.SetState<FStateStomp>();
                }
            }
        }
    }
}