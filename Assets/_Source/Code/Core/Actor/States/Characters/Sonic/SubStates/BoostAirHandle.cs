using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates
{
    public class BoostAirHandle : IBoostHandler
    {
        public void BoostHandle(ActorBase actor, BoostConfig config)
        {
            if (!actor.Flags.HasFlag(FlagType.OutOfControl))
            {
                FBoost boost = actor.StateMachine.GetSubState<FBoost>();
                if (actor.Input.BoostPressed && boost.CanBoost() && boost.CanAirBoost)
                {
                    actor.StateMachine.SetState<FStateAirBoost>();
                }
            }
        }
    }
}