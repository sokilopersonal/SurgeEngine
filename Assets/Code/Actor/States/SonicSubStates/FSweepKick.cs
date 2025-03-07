using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Config.SonicSpecific;

namespace SurgeEngine.Code.ActorStates.SonicSubStates
{
    public class FSweepKick : FActorSubState
    {
        private readonly SweepConfig _config;
        
        public FSweepKick(Actor owner) : base(owner)
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