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
            
            CharacterBase character = CharacterContext.Context;
            /*string[] text = 
            {
                $"Position: {character.transform.position}",
                $"Euler Angles: {character.transform.rotation.eulerAngles}",
                $"Move Dot: {character.Kinematics.MoveDot}",
                $"Current Speed: {character.Kinematics.Speed}",
                $"Current Vertical Speed: {character.Kinematics.Velocity.y}",
                $"Body Velocity: {character.Kinematics.Velocity}",
                $"Planar Velocity: {character.Kinematics.PlanarVelocity}",
                $"State: {character.StateMachine.currentStateName}",
                $"Animation: {character.Animation.StateAnimator.GetCurrentAnimationState()}",
                $"Camera State: {character.Camera.StateMachine.currentStateName}"
            };
            
            holder.text = string.Join("\n", text);*/
            
            // Rewrite the text but instead of direct variables use string.Format
            holder.text = string.Format(
                "Position: {0}\n" +
                "Euler Angles: {1}\n" +
                "Move Dot: {2}\n" +
                "Current Speed: {3:0.0}\n" +
                "Current Vertical Speed: {4:0.0}\n" +
                "Body Velocity: {5}\n" +
                "Planar Velocity: {6}\n" +
                "State: {7}\n" +
                "Animation: {8}\n" +
                "Camera State: {9}\n",
                character.transform.position,
                character.transform.rotation.eulerAngles,
                character.Kinematics.MoveDot,
                character.Kinematics.Speed,
                character.Kinematics.Velocity.y,
                character.Kinematics.Velocity,
                character.Kinematics.PlanarVelocity,
                character.StateMachine.currentStateName,
                character.Animation.StateAnimator.GetCurrentAnimationState(),
                character.Camera.StateMachine.currentStateName);
        }

        private void ToggleWindow(InputAction.CallbackContext obj)
        {
            _active = !_active;
        }
    }
}