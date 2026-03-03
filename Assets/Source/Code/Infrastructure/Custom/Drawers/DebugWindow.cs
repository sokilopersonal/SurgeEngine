using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ImGuiNET;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.Character.System.Characters.Sonic;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Source.Code.Infrastructure.Tools.Managers;
using SurgeEngine.Source.Code.Rendering;
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
        private bool _active;
        private bool _cursorActive;
        
        [Inject] private GameSettings _gameSettings;
        [Inject] private CharacterBase _character;
        [Inject] private Stage _stage;

        // CSM
        private CascadeDemandRendering _cdm;
        private bool _cachedShadowMapsFound;
        private bool _cachedShadowMaps;
        
        // Frame Rate Limit
        private int _frameRateLimit;
        
        // Grass Renderer
        private GrassRenderer _grassRenderer;
        private bool _grassRendererEnabled;
        private float _grassRenderDistance;
        private bool _grassUseRenderDistance;

        private void Awake()
        {
            if (uImGui == null) uImGui = GetComponent<UImGui.UImGui>();

            FindCDM();
            FindGrassRenderer();

            uImGui.enabled = false;

            _toggleAction = InputSystem.actions.FindAction("DebugWindow");
        }

        private void OnEnable()
        {
            uImGui.Layout += OnLayout;
            uImGui.OnInitialize += OnInitialize;
            
            _toggleAction.Enable();
            
            _toggleAction.performed += ToggleWindow;
        }

        private void OnDisable()
        {
            uImGui.Layout -= OnLayout;
            uImGui.OnInitialize -= OnInitialize;
            
            _toggleAction.performed -= ToggleWindow;
        }

        private void OnLayout(UImGui.UImGui obj)
        {
            ImGui.Text("Surge Engine");
            ImGui.Text("Press F3 to toggle cursor");

            if (ImGui.CollapsingHeader("Application Info"))
            {
                ImGui.Text($"Version: {Application.version}");
                ImGui.Text($"Unity Version: {Application.unityVersion}");
                ImGui.Text($"FPS: {Mathf.RoundToInt(1f / Time.unscaledDeltaTime)}");
            }

            if (ImGui.CollapsingHeader("Character Info"))
            {
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
            
            if (ImGui.CollapsingHeader("Rendering"))
            {
                if (ImGui.TreeNode("Cached Shadow Maps"))
                {
                    if (_cachedShadowMapsFound)
                    {
                        ImGui.Checkbox("Enable", ref _cachedShadowMaps);
                
                        _cdm.enabled = _cachedShadowMaps;
                    }
                    else
                    {
                        ImGui.Text("Not found.");
                    }
                    
                    ImGui.TreePop();
                }

                if (ImGui.TreeNode("Framerate"))
                {
                    ImGui.SliderInt("Limit", ref _frameRateLimit, 10, 240);
                    Application.targetFrameRate = _frameRateLimit;
                    
                    ImGui.TreePop();
                }

                if (ImGui.TreeNode("Grass Renderers"))
                {
                    if (_grassRenderer != null)
                    {
                        var type = _grassRenderer.GetType();
                        var flags = BindingFlags.Instance | BindingFlags.NonPublic;
                        var distanceField = type.GetField("maxRenderDistance", 
                            flags);
                        var useDistanceField = type.GetField("useRenderDistance", 
                            flags);

                        ImGui.Checkbox("Enable", ref _grassRendererEnabled);
                        _grassRenderer.enabled = _grassRendererEnabled;
                    
                        ImGui.SliderFloat("Render Distance", ref _grassRenderDistance, 10f, 1000f);
                        ImGui.Checkbox("Use Distance Culling", ref _grassUseRenderDistance);
                    
                        distanceField.SetValue(_grassRenderer, _grassRenderDistance);
                        useDistanceField.SetValue(_grassRenderer, _grassUseRenderDistance);
                    }
                    else
                    {
                        ImGui.Text("Not found.");
                    }
                    
                    ImGui.TreePop();
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

        private void OnInitialize(UImGui.UImGui obj)
        {
            float dpiScale = Mathf.Max(1f, Screen.dpi / 96f);

            var io = ImGui.GetIO();
            
            io.Fonts.Clear();
            io.Fonts.AddFontDefault();
            io.FontGlobalScale = dpiScale;
            
            var style = ImGui.GetStyle();
            style.ScaleAllSizes(dpiScale);
        }

        private void FindCDM()
        {
            _cdm = FindFirstObjectByType<CascadeDemandRendering>();
            _cachedShadowMapsFound = _cdm != null;
            if (_cachedShadowMapsFound) _cachedShadowMaps = _cdm.enabled;
        }

        private void FindGrassRenderer()
        {
            _grassRenderer = FindFirstObjectByType<GrassRenderer>();
            
            if (_grassRenderer != null)
            {
                _grassRendererEnabled = _grassRenderer.enabled;
                GetGrassRendererData(_grassRenderer, out _grassRenderDistance, out _grassUseRenderDistance);
            }
        }

        private void GetGrassRendererData(GrassRenderer grassRenderer, out float distance, out bool useDistance)
        {
            var type = grassRenderer.GetType();
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var distanceField = type.GetField("maxRenderDistance", 
                flags);
            var useDistanceField = type.GetField("useRenderDistance", 
                flags);

            distance = (float)distanceField.GetValue(grassRenderer);
            useDistance = (bool)useDistanceField.GetValue(grassRenderer);
        }

        private void ToggleWindow(InputAction.CallbackContext obj)
        {
            if (_gameSettings.IsDebug)
            {
                _active = !_active;
                uImGui.enabled = _active;
                
                ToggleCursor();
            }
        }

        private void ToggleCursor()
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