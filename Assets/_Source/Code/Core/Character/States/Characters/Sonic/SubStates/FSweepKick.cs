using SurgeEngine._Source.Code.Core.Character.States.BaseStates;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility;
using SurgeEngine._Source.Code.Infrastructure.Config.SonicSpecific;

namespace SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic.SubStates
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