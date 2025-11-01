using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.StateMachine.Base;

namespace SurgeEngine.Source.Code.Core.Character.CameraSystem.Modifiers
{
    public class DriftCameraModifier : BaseCameraModifier, ICameraFloatModifier
    {
        public float Value { get; set; }

        public override void Set(CharacterBase character)
        {
            base.Set(character);
            
            Character.StateMachine.OnStateAssign += OnDrift;
        }

        private void OnDrift(FState obj)
        {
            Value = obj is FStateDrift ? multiplier : 1f;
        }
    }
}