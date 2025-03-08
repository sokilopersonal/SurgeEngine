using System;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.CameraSystem.Modifiers
{
    public class DriftCameraModifier : BaseCameraModifier, ICameraFloatModifier
    {
        [SerializeField] private float multiplier;
        
        public float Value { get; set; }

        public override void Set(Actor actor)
        {
            base.Set(actor);
            
            actor.camera.AddFloatModifier(this);
            actor.stateMachine.OnStateAssign += OnDrift;
        }

        private void OnDrift(FState obj)
        {
            Value = obj is FStateDrift ? multiplier : 1f;
        }
    }
}