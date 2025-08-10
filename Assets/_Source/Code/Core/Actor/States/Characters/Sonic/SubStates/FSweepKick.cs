using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.Mobility;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates
{
    public class FSweepKick : FActorSubState
    {
        private readonly SweepConfig _config;
        
        public FSweepKick(CharacterBase owner) : base(owner)
        {
            owner.TryGetConfig(out _config);
            owner.Input.OnButtonPressed += ButtonPressed;
        }

        private void ButtonPressed(ButtonType button)
        {
            if (button != ButtonType.B || !_config.eligibleAnimationStates.Contains(character.Animation.StateAnimator.GetCurrentAnimationState()))
                return;
            character.StateMachine.SetState<FStateSweepKick>();
        }
    }
}