using System;
using System.Collections;
using SurgeEngine.Source.Code.Core.Character.HUD;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Infrastructure.Tools.Managers;
using SurgeEngine.Source.Code.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;
using Zenject;

namespace SurgeEngine.Source.Code.Infrastructure.Tools
{
    [RequireComponent(typeof(Camera))]
    public class DebugCamera : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float sensitivity = 1f;
        [SerializeField] private float accelerationTime = 1f;
        [SerializeField] private float maxSpeedMultiplier = 5f;
        
        private DebugCameraInput _input;
        
        private Vector2 MoveInput => MoveAction.ReadValue<Vector2>();
        private float VerticalInput => VerticalAction.ReadValue<Vector2>().y;
        private Vector2 LookInput => !_blocked ? LookAction.ReadValue<Vector2>() : Vector2.zero;
        
        private InputAction MoveAction => _input.Debug.Move;
        private InputAction VerticalAction => _input.Debug.Vertical;
        private InputAction LookAction => _input.Debug.Look;
        private InputAction ToggleAction => _input.Debug.Toggle;
        private InputAction TeleportPlayerAction => _input.Debug.Teleport;
        private InputAction TimeAction => _input.Debug.Time;
        private InputAction SlowdownAction => _input.Debug.Slowdown;
        private InputAction AccelerateAction => _input.Debug.Accelerate;

        [Inject] private CharacterBase _character;
        [Inject] private CharacterStageHUD _hud;
        [Inject] private PauseHandler _pauseHandler;

        [Inject] private GameSettings _gameSettings;
        [Inject] private UserInput _userInput;
        
        private Camera _camera;
        private bool _active;
        private float _currentSpeedMultiplier = 1f;
        private float _moveTime;

        private bool _blocked;

        private float _yaw;
        private float _pitch;

        private void Awake()
        {
            _input = new DebugCameraInput();
            _input.Enable();
            
            _camera = GetComponent<Camera>();
            _camera.enabled = false;

            if (!_gameSettings.IsDebug)
            {
                gameObject.SetActive(false);
                _active = false;
            }
        }

        private void Update()
        {
            if (_active)
            {
                Position();
                Rotation();
                
#if UNITY_EDITOR
                var keyboard = Keyboard.current;
                if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
                {
                    CameraLock(true);
                }

                var mouse = Mouse.current;
                if (mouse != null && mouse.leftButton.wasPressedThisFrame)
                {
                    CameraLock(false);
                }
#endif
            }
            else
            {
                _blocked = false;
            }
        }

        private void OnEnable()
        {
            ToggleAction.performed += OnCameraToggle;
            TeleportPlayerAction.performed += OnPlayerTeleport;
            TimeAction.performed += OnTime;
        }

        private void OnDisable()
        {
            ToggleAction.performed -= OnCameraToggle;
            TeleportPlayerAction.performed -= OnPlayerTeleport;
            TimeAction.performed -= OnTime;
        }

        private void Position()
        {
            var dir = transform.forward * MoveInput.y + transform.right * MoveInput.x;
            if (!SlowdownAction.IsPressed() && (dir.magnitude > 0.1f || VerticalInput != 0))
            {
                _moveTime += Time.unscaledDeltaTime;
                _currentSpeedMultiplier = Mathf.Lerp(1f, maxSpeedMultiplier, _moveTime / accelerationTime);
            }
            else
            {
                _moveTime = 0;
                _currentSpeedMultiplier = 1f;
            }

            var speedMultiplier = SlowdownAction.IsPressed() ? 0.5f : AccelerateAction.IsPressed() ? 3f : 1f;
            transform.position += dir * (speed * speedMultiplier * _currentSpeedMultiplier * Time.unscaledDeltaTime);
            if (VerticalInput > 0)
            {
                transform.position += transform.up * (VerticalInput * speed * speedMultiplier * _currentSpeedMultiplier * Time.unscaledDeltaTime);
            }
            else if (VerticalInput < 0)
            {
                transform.position += transform.up * (VerticalInput * speed * speedMultiplier * _currentSpeedMultiplier * Time.unscaledDeltaTime);
            }
        }

        private void Rotation()
        {
            var userSensitivity = _userInput.GetData().Sensitivity.Value * 0.077f;
            _yaw += LookInput.x * sensitivity * userSensitivity;
            _pitch -= LookInput.y * sensitivity * userSensitivity;
            
            var rotation = Quaternion.Euler(_pitch, _yaw, 0);
            _camera.transform.rotation = rotation;
        }

        private void OnCameraToggle(InputAction.CallbackContext obj)
        {
            if (_pauseHandler.Active) return;
            
            _active = !_active;

            var gameCamera = _character.Camera.GetCamera();
            var gameCameraData = gameCamera.GetComponent<HDAdditionalCameraData>();
            _camera.enabled = _active;
            gameCameraData.GetComponent<HDAdditionalCameraData>().CopyTo(_camera.GetComponent<HDAdditionalCameraData>());
            gameCamera.enabled = !_active;
            
            _character.Input.PlayerInput.enabled = !_active;
            _pauseHandler.enabled = !_active;
            _hud.GetComponent<Canvas>().enabled = !_active;

            if (_active)
            {
                _camera.transform.position = gameCamera.transform.position;
                _camera.transform.rotation = gameCamera.transform.rotation;
                
                _yaw = gameCamera.transform.eulerAngles.y;
                _pitch = gameCamera.transform.eulerAngles.x;
                
                Time.timeScale = 0;
            }
            else
            {
                Time.timeScale = 1;
            }
        }

        private void OnPlayerTeleport(InputAction.CallbackContext obj)
        {
            if (_active)
            {
                IEnumerator Routine()
                {
                    float stored = Time.timeScale;
                    Time.timeScale = 1;
                    
                    _character.Rigidbody.position = _camera.transform.position + Vector3.up * 0.5f + _camera.transform.forward * 5;
                    
                    _character.Kinematics.ResetVelocity();
                    var uPos = _character.Rigidbody.position;
                    _character.Kinematics.Mode.Path2D?.Spline.UpdateTime(uPos);
                    _character.Kinematics.Mode.PathForward?.Spline.UpdateTime(uPos);
                    _character.Kinematics.Mode.PathDash?.Spline.UpdateTime(uPos);

                    _character.StateMachine.SetState<FStateIdle>();
                    _character.Flags.Clear();
                    
                    yield return new WaitForFixedUpdate();
                    
                    Time.timeScale = stored;
                }

                StartCoroutine(Routine());
            }
        }
        
        private void OnTime(InputAction.CallbackContext obj)
        {
            if (_active)
            {
                Time.timeScale = Time.timeScale > 0 ? 0 : 1f;
            }
        }

        private void CameraLock(bool value)
        {
            _blocked = value;
        }

        private void OnDestroy()
        {
            _input.Disable();
            _input.Dispose();
        }
    }
}