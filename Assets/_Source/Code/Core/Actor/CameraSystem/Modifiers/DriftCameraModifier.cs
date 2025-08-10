using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Base;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Modifiers
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