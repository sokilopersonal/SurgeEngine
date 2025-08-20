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
            holder.fontSize = 26;
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
                "Camera State: {9}\n" +
                "Path 2D: {10}\n" +
                "Path Forward: {11}\n" +
                "Path Dash: {12}\n" +
                "Is Auto Running: {13}\n",
                character.transform.position,
                character.transform.rotation.eulerAngles,
                character.Kinematics.MoveDot,
                character.Kinematics.Speed,
                character.Kinematics.Velocity.y,
                character.Kinematics.Velocity,
                character.Kinematics.PlanarVelocity,
                character.StateMachine.currentStateName,
                character.Animation.StateAnimator.GetCurrentAnimationState(),
                character.Camera.StateMachine.currentStateName,
                character.Kinematics.Path2D != null ? "Exists" : "None",
                character.Kinematics.PathForward != null ? "Exists" : "None",
                character.Kinematics.PathDash != null ? "Exists" : "None", 
                character.Flags.HasFlag(FlagType.Autorun));
        }

        private void ToggleWindow(InputAction.CallbackContext obj)
        {
            _active = !_active;
        }
    }
}