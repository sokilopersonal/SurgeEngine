using System;
using SurgeEngine.Code.Gameplay.CommonObjects.Mobility;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XInput;

namespace SurgeEngine.Code.Core.Actor.System
{
    public class ActorInput : ActorComponent
    {
        public Vector3 moveVector;
        public Vector2 lookVector;
        public PlayerInput playerInput;
        
        public bool XPressed => XInputAction.WasPressedThisFrame();
        public bool XHeld => XInputAction.IsPressed();
        public bool APressed => AInputAction.WasPressedThisFrame();
        public bool AHeld => AInputAction.IsPressed();
        public bool BPressed => BInputAction.WasPressedThisFrame();
        public bool BReleased => BInputAction.WasReleasedThisFrame();
        public bool BHeld => BInputAction.IsPressed();
        public bool YPressed => YInputAction.WasPressedThisFrame();
        public bool YHeld => YInputAction.IsPressed();
        public bool LeftBumperPressed => BumperInputAction.WasPressedThisFrame() && BumperInputAction.ReadValue<Vector2>().x < 0;
        public bool RightBumperPressed => BumperInputAction.WasPressedThisFrame() && BumperInputAction.ReadValue<Vector2>().x > 0;
        public bool LeftBumperHeld => BumperInputAction.ReadValue<Vector2>().x < 0;
        public bool RightBumperHeld => BumperInputAction.ReadValue<Vector2>().x > 0;
        
        
        public Action<InputAction.CallbackContext> XAction;
        public Action<InputAction.CallbackContext> AAction;
        public Action<InputAction.CallbackContext> BAction;
        public Action<InputAction.CallbackContext> YAction;
        public Action<InputAction.CallbackContext> BumperAction;
        
        protected InputAction XInputAction => playerInput.actions["XAction"];
        protected InputAction AInputAction => playerInput.actions["AAction"];
        protected InputAction BInputAction => playerInput.actions["BAction"];
        protected InputAction YInputAction => playerInput.actions["YAction"];
        protected InputAction BumperInputAction => playerInput.actions["Bumper"];
        protected InputAction TriggerAction => playerInput.actions["Trigger"];

        private bool _lockCamera;
        private InputDevice _device;

        private float _noInputTimer;
        private bool _autoCamera;

        public event Action<ButtonType> OnButtonPressed;

        private void Awake()
        {
            playerInput ??= GetComponent<PlayerInput>();
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

#if UNITY_EDITOR
            if (Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CameraLock(true);
            }

            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                CameraLock(false);
            }
#endif

            if (_lockCamera)
            {
                lookVector = Vector2.zero;
            }
            
            if (Actor.Flags.HasFlag(FlagType.OutOfControl))
            {
                moveVector = Vector3.zero;
            }
        }

        private void JumpInput(InputAction.CallbackContext obj)
        {
            if (obj.started) OnButtonPressed?.Invoke(ButtonType.A);
            
            if (Actor.Flags.HasFlag(FlagType.OutOfControl))
            {
                return;
            }

            AAction?.Invoke(obj);
        }

        private void BoostInput(InputAction.CallbackContext obj)
        {
            if (obj.started) OnButtonPressed?.Invoke(ButtonType.X);

            XAction?.Invoke(obj);
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

        public GameDevice GetDevice()
        {
            GameDevice device = GameDevice.Keyboard;

            switch (_device)
            {
                case Keyboard:
                    device = GameDevice.Keyboard;
                    break;
                case XInputController:
                    device = GameDevice.XboxController;
                    break;
                case Gamepad:
                {
                    if (_device is DualShockGamepad)
                    {
                        device = GameDevice.Playstation;
                    }

                    break;
                }
            }

            return device;
        }
    }

    public enum GameDevice
    {
        Keyboard,
        XboxController,
        Playstation
    }
}
