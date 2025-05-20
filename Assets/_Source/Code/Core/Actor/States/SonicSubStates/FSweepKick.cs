using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.States.SonicSpecific;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.Mobility;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;

namespace SurgeEngine.Code.Core.Actor.States.SonicSubStates
{
    public class FSweepKick : FActorSubState
    {
        private readonly SweepConfig _config;
        
        public FSweepKick(ActorBase owner) : base(owner)
        {
            owner.TryGetConfig(out _config);
            actor.input.OnButtonPressed += ButtonPressed;
        }

        private void ButtonPressed(ButtonType button)
        {
            if (button != ButtonType.B || !_config.eligibleAnimationStates.Contains(actor.animation.StateAnimator.GetCurrentAnimationState()))
                return;
            actor.stateMachine.SetState<FStateSweepKick>();
        }
    }
}