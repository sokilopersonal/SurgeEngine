using ImGuiNET;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.Character.System.Characters.Sonic;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;
using Zenject;

namespace SurgeEngine.Source.Code.Infrastructure.Custom.Drawers
{
    public class DebugWindow : MonoBehaviour
    {
        [SerializeField] private UImGui.UImGui uImGui; 
        private InputAction _toggleAction;
        private InputAction _toggleCursorAction;
        private bool _active;
        private bool _cursorActive;
        
        [Inject] private CharacterBase _character;
        [Inject] private Stage _stage;

        private void Awake()
        {
            if (uImGui == null) uImGui = GetComponent<UImGui.UImGui>();
            
            uImGui.SetCamera(Camera.main);
            uImGui.Reload();
            uImGui.enabled = false;
            Destroy(uImGui.GetComponent<HDAdditionalCameraData>());
            Destroy(uImGui.GetComponent<Camera>());

            _toggleAction = InputSystem.actions.FindAction("DebugWindow");
            _toggleCursorAction = InputSystem.actions.FindAction("ToggleCursor");
        }

        private void OnEnable()
        {
            uImGui.Layout += OnLayout;
            
            _toggleAction.Enable();
            _toggleCursorAction.Enable();
            
            _toggleAction.performed += ToggleWindow;
            _toggleCursorAction.performed += ToggleCursor;
        }

        private void OnDisable()
        {
            uImGui.Layout -= OnLayout;
            
            _toggleAction.performed -= ToggleWindow;
            _toggleCursorAction.performed -= ToggleCursor;
        }

        private void OnLayout(UImGui.UImGui obj)
        {
            ImGui.Text("Surge Engine");
            ImGui.Text("Press F3 to toggle cursor");
            
            ImGui.SeparatorText("Character Info");
            
            ImGui.Text($"Position: {_character.Rigidbody.position}");
            ImGui.Text($"Rotation: {_character.Rigidbody.rotation}");
            ImGui.Text($"Velocity: {_character.Rigidbody.linearVelocity}");
            ImGui.Text($"Speed: {_character.Kinematics.Speed}");
            ImGui.Text($"Vertical Speed: {_character.Kinematics.VerticalVelocity.y}");
            ImGui.Text($"State: {_character.StateMachine.CurrentState?.GetType().Name}");
            ImGui.Text($"Camera State: {_character.Camera.StateMachine.CurrentState?.GetType().Name}");
            ImGui.Text($"Animation State: {_character.Animation.StateAnimator.GetCurrentAnimationState()}");
            
            ImGui.SeparatorText("Path Info");
            if (_character.Kinematics.Path2D != null || _character.Kinematics.PathForward != null || _character.Kinematics.PathDash != null)
            {
                DrawPathInfo(_character.Kinematics.Path2D, "Path 2D");
                DrawPathInfo(_character.Kinematics.PathForward, "Path Forward");
                DrawPathInfo(_character.Kinematics.PathDash, "Path Dash");
            }
            else
            {
                ImGui.BulletText("Currently you don't have any active paths.");
            }

            if (ImGui.CollapsingHeader("Character Utility"))
            {
                ImGui.SeparatorText("Base Utility");
                
                if (ImGui.Button("Add 10 Rings"))
                {
                    _stage.Data.RingCount += 10;
                }
                ImGui.SameLine();
                
                if (ImGui.Button("Add 100 Rings"))
                {
                    _stage.Data.RingCount += 100;
                }

                if (_character is Sonic)
                {
                    ImGui.SeparatorText("Sonic Utility");
                    
                    if (_character.StateMachine.GetState(out FBoost boost))
                    {
                        if (ImGui.Button("Fill Boost"))
                        {
                            boost.BoostEnergy = boost.MaxBoostEnergy;
                        }
                    }
                }
            }

            void DrawPathInfo(ChangeModeData data, string name)
            {
                if (data != null)
                {
                    ImGui.BulletText(name);
                
                    ImGui.Text($"Name: {data.Spline.Container.name}");
                    ImGui.Text($"Time: {data.Spline.NormalizedTime}");
                }
            }
        }

        private void Update()
        {
            /*holder.gameObject.SetActive(_active);
            
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
                "Is Auto Running: {13}\n" +
                "Turn Rate: {14}\n",
                character.transform.position,
                character.transform.rotation.eulerAngles,
                character.Kinematics.MoveDot,
                character.Kinematics.Speed,
                character.Kinematics.Velocity.y,
                character.Kinematics.Velocity,
                character.Kinematics.PlanarVelocity,
                character.StateMachine.CurrentState?.GetType().Name,
                character.Animation.StateAnimator.GetCurrentAnimationState(),
                character.Camera.StateMachine.CurrentState?.GetType().Name,
                character.Kinematics.Path2D != null ? "Exists" : "None",
                character.Kinematics.PathForward != null ? "Exists" : "None",
                character.Kinematics.PathDash != null ? "Exists" : "None", 
                character.Flags.HasFlag(FlagType.Autorun),
                character.Kinematics.TurnRate);*/
        }

        private void ToggleWindow(InputAction.CallbackContext obj)
        {
            if (Debug.isDebugBuild)
            {
                _active = !_active;
                uImGui.enabled = _active;
            }
        }

        private void ToggleCursor(InputAction.CallbackContext obj)
        {
            _cursorActive = !_cursorActive;
            if (_cursorActive)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            
            _character.Input.PlayerInput.enabled = !_cursorActive;
        }
    }
}