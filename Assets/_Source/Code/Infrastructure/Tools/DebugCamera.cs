using SurgeEngine._Source.Code.Core.Character.HUD;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Infrastructure.Tools.Managers;
using SurgeEngine._Source.Code.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.HighDefinition;
using Zenject;

namespace SurgeEngine._Source.Code.Infrastructure.Tools
{
    [RequireComponent(typeof(PlayerInput), typeof(Camera))]
    public class DebugCamera : MonoBehaviour
    {
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private float speed = 10f;
        [SerializeField] private float sensitivity = 1f;
        [SerializeField] private float accelerationTime = 1f;
        [SerializeField] private float maxSpeedMultiplier = 5f;
        
        private Vector2 MoveInput => MoveAction.ReadValue<Vector2>();
        private float VerticalInput => VerticalAction.ReadValue<Vector2>().y;
        private Vector2 LookInput => LookAction.ReadValue<Vector2>();
        
        private InputAction MoveAction => playerInput.actions["Move"];
        private InputAction VerticalAction => playerInput.actions["Vertical"];
        private InputAction LookAction => playerInput.actions["Look"];
        private InputAction ToggleAction => playerInput.actions["Toggle"];
        private InputAction TeleportPlayerAction => playerInput.actions["Teleport"];
        private InputAction TimeAction => playerInput.actions["Time"];
        private InputAction SlowdownAction => playerInput.actions["Slowdown"];

        [Inject] private CharacterBase _character;
        [Inject] private CharacterStageHUD _hud;
        [Inject] private PauseHandler _pauseHandler;

        [Inject] private GameSettings _gameSettings;
        [Inject] private UserInput _userInput;
        
        private Camera _camera;
        private bool _active;
        private float _currentSpeedMultiplier = 1f;
        private float _moveTime;

        private float _yaw;
        private float _pitch;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
            _camera.enabled = false;

            gameObject.hideFlags = HideFlags.HideInHierarchy;

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

            var speedMultiplier = SlowdownAction.IsPressed() ? 0.5f : 1f;
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
            var userSensitivity = _userInput.GetData().Sensitivity.Value;
            _yaw += LookInput.x * sensitivity * 10 * userSensitivity * Time.unscaledDeltaTime;
            _pitch -= LookInput.y * sensitivity * 10 * userSensitivity * Time.unscaledDeltaTime;
            
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
            gameCameraData.fullscreenPassthrough = _active;
            
            _character.Input.playerInput.enabled = !_active;
            _pauseHandler.enabled = !_active;
            _hud.GetComponent<Canvas>().enabled = !_active;

            if (_active)
            {
                _camera.transform.position = gameCamera.transform.position;
                _camera.transform.rotation = gameCamera.transform.rotation;
                
                _yaw = gameCamera.transform.eulerAngles.y;
                _pitch = gameCamera.transform.eulerAngles.x;
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
                _character.Kinematics.ResetVelocity();
                _character.Kinematics.Set2DPath(null);
                _character.Kinematics.SetForwardPath(null);
                _character.Kinematics.SetDashPath(null);

                _character.Flags.Clear();

                _character.Rigidbody.position = _camera.transform.position + Vector3.up * 0.5f + _camera.transform.forward * 5;
            }
        }

        private void OnTime(InputAction.CallbackContext obj)
        {
            if (_active)
            {
                Time.timeScale = Time.timeScale > 0 ? 0 : 1f;
            }
        }
    }
}