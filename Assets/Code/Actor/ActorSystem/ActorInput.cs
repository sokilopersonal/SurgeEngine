using System;
using System.Collections.Generic;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Custom;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorInput : MonoBehaviour, IActorComponent
    {
        public Actor actor { get; set; }

        public Vector3 moveVector;

        public Vector2 lookVector;

        private SurgeInput _input;

        // Boost

        public bool BoostPressed => _input.Gameplay.Boost.WasPressedThisFrame();

        public bool BoostHeld => _input.Gameplay.Boost.IsPressed();

        public Action<InputAction.CallbackContext> BoostAction;

        // Jump

        public bool JumpPressed => _input.Gameplay.Jump.WasPressedThisFrame();

        public bool JumpHeld => _input.Gameplay.Jump.IsPressed();

        public Action<InputAction.CallbackContext> JumpAction;

        // B Button

        public bool BPressed => _input.Gameplay.BButton.WasPressedThisFrame();

        public bool BReleased => _input.Gameplay.BButton.WasReleasedThisFrame();

        public bool BHeld => _input.Gameplay.BButton.IsPressed();

        public Action<InputAction.CallbackContext> BAction;

        // Y Button

        public bool YPressed => _input.Gameplay.YButton.WasPressedThisFrame();

        public bool YHeld => _input.Gameplay.YButton.IsPressed();

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
            _input = new SurgeInput();
            _input.Enable();

            _translatedDeviceNames = new Dictionary<string, string>()
            {
                ["Keyboard"] = "Keyboard",
                ["XInputControllerWindows"] = "Xbox",
            };
        }

        public void OnInit()
        {
        }

        private void OnEnable()
        {
            _input.Enable();

            _input.Gameplay.Boost.started += BoostInput;
            _input.Gameplay.Boost.canceled += BoostInput;

            _input.Gameplay.Jump.started += JumpInput;
            _input.Gameplay.Jump.canceled += JumpInput;
            
            _input.Gameplay.BButton.started += BInput;
            _input.Gameplay.BButton.canceled += BInput;
            
            _input.Gameplay.YButton.started += YInput;
            _input.Gameplay.YButton.canceled += YInput;
            
            _input.Gameplay.Quickstep.started += QuickstepInput;
            _input.Gameplay.Quickstep.canceled += QuickstepInput;

            _input.Gameplay.Start.started += StartInput;
            _input.Gameplay.Start.canceled += StartInput;
        }

        private void OnDisable()
        {
            _input.Disable();

            moveVector = Vector3.zero;
            lookVector = Vector2.zero;

            _input.Gameplay.Boost.started -= BoostInput;
            _input.Gameplay.Boost.canceled -= BoostInput;

            _input.Gameplay.Jump.started -= JumpInput;
            _input.Gameplay.Jump.canceled -= JumpInput;
            
            _input.Gameplay.BButton.started -= BInput;
            _input.Gameplay.BButton.canceled -= BInput;
            
            _input.Gameplay.YButton.started -= YInput;
            _input.Gameplay.YButton.canceled -= YInput;
            
            _input.Gameplay.Quickstep.started -= QuickstepInput;
            _input.Gameplay.Quickstep.canceled -= QuickstepInput;
            
            _input.Gameplay.Start.started -= StartInput;
            _input.Gameplay.Start.canceled -= StartInput;
            
            var pad = Gamepad.current;
            pad.SetMotorSpeeds(0, 0);
        }

        private void Update()
        {
            var temp = _input.Gameplay.Movement.ReadValue<Vector2>();
            moveVector = new Vector3(temp.x, 0, temp.y);
            lookVector = _input.Gameplay.Camera.ReadValue<Vector2>();

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

        private void QuickstepInput(InputAction.CallbackContext obj)
        {
            if (obj.started)
            {
                int direction = (int)obj.ReadValue<Vector2>().x;
                
                Debug.Log(direction);
            }
        }

        private void LBInput(InputAction.CallbackContext obj)
        {
            if (obj.started) OnButtonPressed?.Invoke(ButtonType.LB);
            
            if (actor.flags.HasFlag(FlagType.OutOfControl))
            {
                return;
            }
        }

        private void RBInput(InputAction.CallbackContext obj)
        {
            if (obj.started) OnButtonPressed?.Invoke(ButtonType.RB);
            
            if (actor.flags.HasFlag(FlagType.OutOfControl))
            {
                return;
            }
        }

        private void StartInput(InputAction.CallbackContext obj)
        {
            if (obj.started)
            {
                var dv = obj.control.device;
                Debug.Log(dv.name);
                
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
}
