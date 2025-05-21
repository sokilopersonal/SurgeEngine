using SurgeEngine.Code.Core.Actor.System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurgeEngine.Code.Infrastructure.Custom.Drawers
{
    public class DebugWindow : MonoBehaviour
    {
        [SerializeField] private TMP_Text holder;
        [SerializeField] private InputActionReference windowToggleReference;

        private bool _active;

        private void OnEnable()
        {
            windowToggleReference.action.Enable();
            windowToggleReference.action.performed += ToggleWindow;
        }

        private void OnDisable()
        {
            windowToggleReference.action.Disable();
        }

        private void Update()
        {
            holder.gameObject.SetActive(_active);
            
            if (!_active) return;
            
            ActorBase actor = ActorContext.Context;
            string[] text = 
            {
                $"Position: {actor.transform.position}",
                $"Euler Angles: {actor.transform.rotation.eulerAngles}",
                $"Move Dot: {actor.stats.moveDot}",
                $"Current Speed: {actor.kinematics.Speed}",
                $"Current Vertical Speed: {actor.kinematics.Velocity.y}",
                $"Body Velocity: {actor.kinematics.Velocity}",
                $"Planar Velocity: {actor.kinematics.PlanarVelocity}",
                $"State: {actor.stateMachine.currentStateName}",
                $"Animation: {actor.animation.StateAnimator.GetCurrentAnimationState()}",
                $"Camera State: {actor.camera.stateMachine.currentStateName}"
            };
            
            holder.text = string.Join("\n", text);
        }

        private void ToggleWindow(InputAction.CallbackContext obj)
        {
            _active = !_active;
        }
    }
}