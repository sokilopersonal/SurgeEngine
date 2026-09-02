using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility;
using SurgeEngine.Source.Code.Infrastructure.Config.Sonic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates
{
    public class FSweepKick : FCharacterSubState
    {
        private readonly SweepConfig _config;

        public FSweepKick(CharacterBase owner) : base(owner)
        {
            owner.TryGetConfig(out _config);
        }

        public void OnInput(InputAction.CallbackContext ctx)
        {
            if (!ctx.started) return;
            if (!_config.EligibleAnimationStates.Contains(Character.Animation.StateAnimator.GetCurrentAnimationState()))
                return;

            Debug.Log($"<color=red>Zak, tell me if it's working please<color>");
            Character.StateMachine.SetState<FStateSweepKick>();
        }
    }
}