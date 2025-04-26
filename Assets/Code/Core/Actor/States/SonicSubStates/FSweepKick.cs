using SurgeEngine.Code.Actor.States.BaseStates;
using SurgeEngine.Code.Actor.States.SonicSpecific;
using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Config.SonicSpecific;

namespace SurgeEngine.Code.Actor.States.SonicSubStates
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