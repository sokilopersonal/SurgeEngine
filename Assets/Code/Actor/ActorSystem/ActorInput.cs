using System;
using System.Collections.Generic;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Custom;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorInput : ActorComponent
    {
        public Vector3 moveVector;
        public Vector2 lookVector;
        public PlayerInput playerInput;

        // Boost

        public bool BoostPressed => playerInput.actions["XAction"].WasPressedThisFrame();

        public bool BoostHeld => playerInput.actions["XAction"].IsPressed();

        public Action<InputAction.CallbackContext> BoostAction;

        // Jump

        public bool JumpPressed => playerInput.actions["AAction"].WasPressedThisFrame();

        public bool JumpHeld => playerInput.actions["AAction"].IsPressed();

        public Action<InputAction.CallbackContext> JumpAction;

        // B Button

        public bool BPressed => playerInput.actions["BAction"].WasPressedThisFrame();

        public bool BReleased => playerInput.actions["BAction"].WasReleasedThisFrame();

        public bool BHeld => playerInput.actions["BAction"].IsPressed();

        public Action<InputAction.CallbackContext> BAction;

        // Y Button

        public bool YPressed => playerInput.actions["YAction"].WasPressedThisFrame();

        public bool YHeld => playerInput.actions["YAction"].IsPressed();

        public Action<InputAction.CallbackContext> YAction;

        // Bumpers
        public bool LeftBumperPressed => playerInput.actions["Bumper"].ReadValue<Vector2>() == new Vector2(-1, 0);
        public bool RightBumperPressed => playerInput.actions["Bumper"].ReadValue<Vector2>() == new Vector2(1, 0);
        public Action<InputAction.CallbackContext> BumperAction;

        private bool _lockCamera;
        private Rect _screenRect;
        private InputDevice _device;

        private Dictionary<string, string> _translatedDeviceNames;

        private float _noInputTimer;
        private bool _autoCamera;

        public event Action<ButtonType> OnButtonPressed;

        private void Awake()
        {
            playerInput ??= GetComponent<PlayerInput>();
            _screenRect = new Rect(0, 0, Screen.width, Screen.height);
            
            _translatedDeviceNames = new Dictionary<string, string>()
            {
                ["Keyboard"] = "Keyboard",
                ["XInputControllerWindows"] = "Xbox",
            };
        }

        private void OnEnable()
        {
            playerInput.actions["XAction"].started += BoostInput;
            playerInput.actions["XAction"].canceled += BoostInput;

            playerInput.actions["AAction"].started += JumpInput;
            playerInput.actions["AAction"].canceled += JumpInput;
            
            playerInput.actions["BAction"].started += BInput;
            playerInput.actions["BAction"].canceled += BInput;
            
            playerInput.actions["YAction"].started += YInput;
            playerInput.actions["YAction"].canceled += YInput;
            
            playerInput.actions["Bumper"].started += BumperInput;
            playerInput.actions["Bumper"].canceled += BumperInput;
        }

        private void OnDisable()
        {
            moveVector = Vector3.zero;
            lookVector = Vector2.zero;

            playerInput.actions["XAction"].started -= BoostInput;
            playerInput.actions["XAction"].canceled -= BoostInput;

            playerInput.actions["AAction"].started -= JumpInput;
            playerInput.actions["AAction"].canceled -= JumpInput;
            
            playerInput.actions["BAction"].started -= BInput;
            playerInput.actions["BAction"].canceled -= BInput;
            
            playerInput.actions["YAction"].started -= YInput;
            playerInput.actions["YAction"].canceled -= YInput;
            
            playerInput.actions["Bumper"].started -= BumperInput;
            playerInput.actions["Bumper"].canceled -= BumperInput;
            
            Gamepad pad = Gamepad.current;
            pad?.SetMotorSpeeds(0, 0);
        }

        private void Update()
        {
            Vector2 temp = playerInput.actions["Movement"].ReadValue<Vector2>();
            moveVector = new Vector3(temp.x, 0, temp.y);
            lookVector = playerInput.actions["Camera"].ReadValue<Vector2>() * (_device is Gamepad ? 100f * Time.deltaTime : 1f);

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
            
            ReadOnlyArray<InputDevice> devices = InputSystem.devices;

            foreach (InputDevice device in devices)
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
            
            CameraLock(!_screenRect.Contains(playerInput.actions["MousePosition"].ReadValue<Vector2>()));

            if (_lockCamera)
            {
                lookVector = Vector2.zero;
            }
            
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

            BoostAction?.Invoke(obj);
        }

        private void BInput(InputAction.CallbackContext obj)
        {
            if (obj.started) OnButtonPressed?.Invoke(ButtonType.B);
        }

        private void YInput(InputAction.CallbackContext obj)
        {
            if (obj.started) OnButtonPressed?.Invoke(ButtonType.Y);
        }

        private void BumperInput(InputAction.CallbackContext obj)
        {
            if (obj.started)
            {
                int direction = (int)obj.ReadValue<Vector2>().x;
                OnButtonPressed?.Invoke(direction == -1 ? ButtonType.LB : ButtonType.RB);
            }
        }

        private void StartInput(InputAction.CallbackContext obj)
        {
            if (obj.started)
            {
                InputDevice dv = obj.control.device;
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
