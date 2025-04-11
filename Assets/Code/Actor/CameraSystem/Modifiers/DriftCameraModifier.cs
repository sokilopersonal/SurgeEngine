using SurgeEngine.Code.Actor.States.SonicSpecific;
using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Actor.CameraSystem.Modifiers
{
    public class DriftCameraModifier : BaseCameraModifier, ICameraFloatModifier
    {
        public float Value { get; set; }

        public override void Set(ActorBase actor)
        {
            base.Set(actor);
            
            actor.stateMachine.OnStateAssign += OnDrift;
        }

        private void OnDrift(FState obj)
        {
            Value = obj is FStateDrift ? multiplier : 1f;
        }
    }
}