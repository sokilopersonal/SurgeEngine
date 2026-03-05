using System;
using Alchemy.Inspector;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UIElements;

namespace SurgeEngine.Source.Code.Core.Character.System
{
    public class CharacterInput : CharacterComponent
    {
        [SerializeField, Tooltip("Is automatic camera control enabled by default?")] 
        private bool enableAutoCameraByDefault = true;

        [SerializeField, Tooltip("How much time it takes for camera to automatically take control when there is no input from the player.")] 
        private float autoCameraTime = 1.5f;

        public Vector3 MoveVector { get; private set; }
        public Vector2 LookVector { get; private set; }
        public PlayerInput PlayerInput { get; private set; }
        
        public bool XPressed => XInputAction.WasPressedThisFrame();
        public bool XReleased => XInputAction.WasReleasedThisFrame();
        public bool XHeld => XInputAction.IsPressed();
        public bool APressed => AInputAction.WasPressedThisFrame();
        public bool AReleased => AInputAction.WasReleasedThisFrame();
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
        
        protected InputAction MovementAction => PlayerInput.actions["Movement"];
        protected InputAction LookAction => PlayerInput.actions["Camera"];
        protected InputAction XInputAction => PlayerInput.actions["XAction"];
        protected InputAction AInputAction => PlayerInput.actions["AAction"];
        protected InputAction BInputAction => PlayerInput.actions["BAction"];
        protected InputAction YInputAction => PlayerInput.actions["YAction"];
        protected InputAction BumperInputAction => PlayerInput.actions["Bumper"];
        protected InputAction TriggerAction => PlayerInput.actions["Trigger"];

        private bool _lockCamera;
        private InputDevice _device;

        private float _noInputTimer;
        private bool _autoCamera;

        public event Action<ButtonType> OnButtonPressed;
        
        private void Awake()
        {
            PlayerInput = GetComponent<PlayerInput>();
            
            _autoCamera = enableAutoCameraByDefault;
            if (enableAutoCameraByDefault) _noInputTimer = 1;
        }

        private void OnEnable()
        {
            PlayerInput.actions["XAction"].started += BoostInput;
            PlayerInput.actions["XAction"].canceled += BoostInput;

            PlayerInput.actions["AAction"].started += JumpInput;
            PlayerInput.actions["AAction"].canceled += JumpInput;
            
            PlayerInput.actions["BAction"].started += BInput;
            PlayerInput.actions["BAction"].canceled += BInput;
            
            PlayerInput.actions["YAction"].started += YInput;
            PlayerInput.actions["YAction"].canceled += YInput;
            
            PlayerInput.actions["Bumper"].started += BumperInput;
            PlayerInput.actions["Bumper"].canceled += BumperInput;
        }

        private void OnDisable()
        {
            MoveVector = Vector3.zero;
            LookVector = Vector2.zero;

            PlayerInput.actions["XAction"].started -= BoostInput;
            PlayerInput.actions["XAction"].canceled -= BoostInput;

            PlayerInput.actions["AAction"].started -= JumpInput;
            PlayerInput.actions["AAction"].canceled -= JumpInput;
            
            PlayerInput.actions["BAction"].started -= BInput;
            PlayerInput.actions["BAction"].canceled -= BInput;
            
            PlayerInput.actions["YAction"].started -= YInput;
            PlayerInput.actions["YAction"].canceled -= YInput;
            
            PlayerInput.actions["Bumper"].started -= BumperInput;
            PlayerInput.actions["Bumper"].canceled -= BumperInput;
            
            Gamepad pad = Gamepad.current;
            pad?.SetMotorSpeeds(0, 0);
        }

        private void Update()
        {
            Vector2 temp = MovementAction.ReadValue<Vector2>();
            MoveVector = new Vector3(temp.x, 0, temp.y);
            LookVector = LookAction.ReadValue<Vector2>() * (_device is Gamepad ? 100f * Time.deltaTime : 1f);

            UpdateNoInputTimer();
            
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
                LookVector = Vector2.zero;
            }
            
            if (Character.Flags.HasFlag(FlagType.OutOfControl))
            {
                MoveVector = Vector3.zero;
            }
        }

        private void UpdateNoInputTimer()
        {
            if (LookVector == Vector2.zero)
            {
                _noInputTimer += Time.deltaTime / autoCameraTime;

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
        }

        private void JumpInput(InputAction.CallbackContext obj)
        {
            if (obj.started) OnButtonPressed?.Invoke(ButtonType.A);
            
            if (Character.Flags.HasFlag(FlagType.OutOfControl))
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
                LookVector = Vector2.zero;
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
