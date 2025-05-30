using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Base;

namespace SurgeEngine.Code.Core.Actor.CameraSystem.Modifiers
{
    public class DriftCameraModifier : BaseCameraModifier, ICameraFloatModifier
    {
        public float Value { get; set; }

        public override void Set(ActorBase actor)
        {
            base.Set(actor);
            
            Actor.StateMachine.OnStateAssign += OnDrift;
        }

        private void OnDrift(FState obj)
        {
            Value = obj is FStateDrift ? multiplier : 1f;
        }
    }
}