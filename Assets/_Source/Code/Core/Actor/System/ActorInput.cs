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
        
        public bool LeftPressed => LeftInputAction.WasPressedThisFrame();
        public bool LeftHeld => LeftInputAction.IsPressed();
        public bool UpPressed => UpInputAction.WasPressedThisFrame();
        public bool UpHeld => UpInputAction.IsPressed();
        public bool DownPressed => DownInputAction.WasPressedThisFrame();
        public bool DownReleased => DownInputAction.WasReleasedThisFrame();
        public bool DownHeld => playerInput.actions["BAction"].IsPressed();
        public bool SpecialPressed => SpecialInputAction.WasPressedThisFrame();
        public bool SpecialHeld => SpecialInputAction.IsPressed();
        public bool LeftBumperPressed => BumperInputAction.ReadValue<Vector2>() == new Vector2(-1, 0);
        public bool RightBumperPressed => BumperInputAction.ReadValue<Vector2>() == new Vector2(1, 0);
        
        public Action<InputAction.CallbackContext> LeftAction;
        public Action<InputAction.CallbackContext> UpAction;
        public Action<InputAction.CallbackContext> DownAction;
        public Action<InputAction.CallbackContext> SpecialAction;
        public Action<InputAction.CallbackContext> BumperAction;
        
        protected InputAction LeftInputAction => playerInput.actions["XAction"];
        protected InputAction UpInputAction => playerInput.actions["AAction"];
        protected InputAction DownInputAction => playerInput.actions["BAction"];
        protected InputAction SpecialInputAction => playerInput.actions["YAction"];
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

            UpAction?.Invoke(obj);
        }

        private void BoostInput(InputAction.CallbackContext obj)
        {
            if (obj.started) OnButtonPressed?.Invoke(ButtonType.X);

            LeftAction?.Invoke(obj);
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
