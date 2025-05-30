using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.Mobility;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates
{
    public class FSweepKick : FActorSubState
    {
        private readonly SweepConfig _config;
        
        public FSweepKick(ActorBase owner) : base(owner)
        {
            owner.TryGetConfig(out _config);
            Actor.Input.OnButtonPressed += ButtonPressed;
        }

        private void ButtonPressed(ButtonType button)
        {
            if (button != ButtonType.B || !_config.eligibleAnimationStates.Contains(Actor.Animation.StateAnimator.GetCurrentAnimationState()))
                return;
            Actor.StateMachine.SetState<FStateSweepKick>();
        }
    }
}