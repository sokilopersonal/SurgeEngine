using System;
using SurgeEngine.Code.ActorStates.SonicSpecific;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.CameraSystem.Modifiers
{
    public class DriftCameraModifier : MonoBehaviour, ICameraFloatModifier
    {
        [SerializeField] private float multiplier;
        
        public float Value { get; set; }
        
        private Actor _actor => ActorContext.Context;

        private void OnEnable()
        {
            _actor.camera.AddFloatModifier(this);
            _actor.stateMachine.OnStateAssign += OnDrift;
        }
        
        private void OnDisable()
        {
            _actor.camera.RemoveFloatModifier(this);
            _actor.stateMachine.OnStateAssign -= OnDrift;
        }

        private void OnDrift(FState obj)
        {
            Value = obj is FStateDrift ? multiplier : 1f;
        }
    }
}