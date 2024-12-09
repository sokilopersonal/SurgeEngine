using System;
using System.Collections.Generic;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Custom;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorInput : ActorComponent
    {
        public Vector3 moveVector;

        public Vector2 lookVector;

        private PlayerInput _playerInput;

        // Boost

        public bool BoostPressed => _playerInput.actions["XAction"].WasPressedThisFrame();

        public bool BoostHeld => _playerInput.actions["XAction"].IsPressed();

        public Action<InputAction.CallbackContext> BoostAction;

        // Jump

        public bool JumpPressed => _playerInput.actions["AAction"].WasPressedThisFrame();

        public bool JumpHeld => _playerInput.actions["AAction"].IsPressed();

        public Action<InputAction.CallbackContext> JumpAction;

        // B Button

        public bool BPressed => _playerInput.actions["BAction"].WasPressedThisFrame();

        public bool BReleased => _playerInput.actions["BAction"].WasReleasedThisFrame();

        public bool BHeld => _playerInput.actions["BAction"].IsPressed();

        public Action<InputAction.CallbackContext> BAction;

        // Y Button

        public bool YPressed => _playerInput.actions["YAction"].WasPressedThisFrame();

        public bool YHeld => _playerInput.actions["YAction"].IsPressed();

        public Action<InputAction.CallbackContext> YAction;

        // Bumpers
        public Action<InputAction.CallbackContext> BumperAction;

        private bool _lockCamera;
        private InputDevice _device;

        private Dictionary<string, string> _translatedDeviceNames;

        private float _noInputTimer;
        private bool _autoCamera;

        public event Action<ButtonType> OnButtonPressed;

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            
            _translatedDeviceNames = new Dictionary<string, string>()
            {
                ["Keyboard"] = "Keyboard",
                ["XInputControllerWindows"] = "Xbox",
            };
        }

        private void OnEnable()
        {
            _playerInput.enabled = true;

            _playerInput.actions["XAction"].started += BoostInput;
            _playerInput.actions["XAction"].canceled += BoostInput;

            _playerInput.actions["AAction"].started += JumpInput;
            _playerInput.actions["AAction"].canceled += JumpInput;
            
            _playerInput.actions["BAction"].started += BInput;
            _playerInput.actions["BAction"].canceled += BInput;
            
            _playerInput.actions["YAction"].started += YInput;
            _playerInput.actions["YAction"].canceled += YInput;
            
            _playerInput.actions["Bumper"].started += BumperInput;
            _playerInput.actions["Bumper"].canceled += BumperInput;
            
            // _playerInput.Gameplay.Start.started += StartInput;
            // _playerInput.Gameplay.Start.canceled += StartInput;
        }

        private void OnDisable()
        {
            _playerInput.enabled = false;

            moveVector = Vector3.zero;
            lookVector = Vector2.zero;

            _playerInput.actions["XAction"].started -= BoostInput;
            _playerInput.actions["XAction"].canceled -= BoostInput;

            _playerInput.actions["AAction"].started -= JumpInput;
            _playerInput.actions["AAction"].canceled -= JumpInput;
            
            _playerInput.actions["BAction"].started -= BInput;
            _playerInput.actions["BAction"].canceled -= BInput;
            
            _playerInput.actions["YAction"].started -= YInput;
            _playerInput.actions["YAction"].canceled -= YInput;
            
            _playerInput.actions["Bumper"].started -= BumperInput;
            _playerInput.actions["Bumper"].canceled -= BumperInput;
            
            // _playerInput.Gameplay.Start.started -= StartInput;
            // _playerInput.Gameplay.Start.canceled -= StartInput;
            
            var pad = Gamepad.current;
            pad.SetMotorSpeeds(0, 0);
        }

        private void Update()
        {
            var temp = _playerInput.actions["Movement"].ReadValue<Vector2>();
            moveVector = new Vector3(temp.x, 0, temp.y);
            lookVector = _playerInput.actions["Camera"].ReadValue<Vector2>() * (_device is Gamepad ? 100f * Time.deltaTime : 1f);

            if (lookVector == Vector2.zero)
            {
                _noInputTimer += Time.deltaTime;

                if (_noInputTimer > 1f)
                {
                    _autoCamera = true;
                }
            }
            else
            {
                _noInputTimer = 0f;
                _autoCamera = false;
            }
            
            var devices = InputSystem.devices;

            foreach (var device in devices)
            {
                if (device.wasUpdatedThisFrame)
                {
                    if (device is Keyboard)
                    {
                        _device = device;
                    }
                    else if (device is Gamepad)
                    {
                        _device = device;
                    }
                }
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                CameraLock(true);
            }

            if (Input.GetMouseButtonDown(0))
            {
                CameraLock(false);
            }
            
            if (_lockCamera)
            {
                lookVector = Vector2.zero;
            }
#endif

            if (actor.flags.HasFlag(FlagType.OutOfControl))
            {
                moveVector = Vector3.zero;
            }
        }

        private void JumpInput(InputAction.CallbackContext obj)
        {
            if (obj.started) OnButtonPressed?.Invoke(ButtonType.A);
            
            if (actor.flags.HasFlag(FlagType.OutOfControl))
            {
                return;
            }

            JumpAction?.Invoke(obj);
        }

        private void BoostInput(InputAction.CallbackContext obj)
        {
            if (obj.started) OnButtonPressed?.Invoke(ButtonType.X);
            
            if (actor.flags.HasFlag(FlagType.OutOfControl) && !actor.flags.CheckForTag(Tags.AllowBoost))
            {
                return;
            }

            BoostAction?.Invoke(obj);
        }

        private void BInput(InputAction.CallbackContext obj)
        {
            if (obj.started) OnButtonPressed?.Invoke(ButtonType.B);
            
            if (actor.flags.HasFlag(FlagType.OutOfControl))
            {
                return;
            }
        }

        private void YInput(InputAction.CallbackContext obj)
        {
            if (obj.started) OnButtonPressed?.Invoke(ButtonType.Y);
            
            if (actor.flags.HasFlag(FlagType.OutOfControl))
            {
                return;
            }
        }

        private void BumperInput(InputAction.CallbackContext obj)
        {
            if (obj.started)
            {
                int direction = (int)obj.ReadValue<Vector2>().x;
                OnButtonPressed?.Invoke(direction == -1 ? ButtonType.LB : ButtonType.RB);
                
                if (actor.flags.HasFlag(FlagType.OutOfControl))
                {
                    return;
                }
            }
        }

        private void StartInput(InputAction.CallbackContext obj)
        {
            if (obj.started)
            {
                var dv = obj.control.device;
                _device = dv;
            }
        }

        public void CameraLock(bool value)
        {
            _lockCamera = value;
            
            if (_lockCamera)
            {
                lookVector = Vector2.zero;
            }
        }
        
        public bool IsAutoCamera() { return _autoCamera; }
        
        public InputDevice GetDevice() { return _device; }
        public string GetTranslatedDeviceName(InputDevice device) { return _translatedDeviceNames[device.name]; }
    }

    public class ActorComponent : MonoBehaviour
    {
        protected Actor actor => ActorContext.Context;
    }
}
