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
            string[] text = 
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
            
            holder.text = string.Join("\n", text);
        }

        private void ToggleWindow(InputAction.CallbackContext obj)
        {
            _active = !_active;
        }
    }
}