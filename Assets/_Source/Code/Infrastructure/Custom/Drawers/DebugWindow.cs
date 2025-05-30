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
                $"Move Dot: {actor.Stats.moveDot}",
                $"Current Speed: {actor.Kinematics.Speed}",
                $"Current Vertical Speed: {actor.Kinematics.Velocity.y}",
                $"Body Velocity: {actor.Kinematics.Velocity}",
                $"Planar Velocity: {actor.Kinematics.PlanarVelocity}",
                $"State: {actor.stateMachine.currentStateName}",
                $"Animation: {actor.Animation.StateAnimator.GetCurrentAnimationState()}",
                $"Camera State: {actor.Camera.stateMachine.currentStateName}"
            };
            
            holder.text = string.Join("\n", text);
        }

        private void ToggleWindow(InputAction.CallbackContext obj)
        {
            _active = !_active;
        }
    }
}