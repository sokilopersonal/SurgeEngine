using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility;
using SurgeEngine.Source.Code.Infrastructure.Config.Sonic;

namespace SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates
{
    public class FSweepKick : FCharacterSubState
    {
        private readonly SweepConfig _config;
        
        public FSweepKick(CharacterBase owner) : base(owner)
        {
            owner.TryGetConfig(out _config);
            owner.Input.OnButtonPressed += ButtonPressed;
        }

        private void ButtonPressed(ButtonType button)
        {
            if (button != ButtonType.B || !_config.eligibleAnimationStates.Contains(Character.Animation.StateAnimator.GetCurrentAnimationState()))
                return;
            Character.StateMachine.SetState<FStateSweepKick>();
        }
    }
}