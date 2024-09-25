using System;
using SurgeEngine.Code.Custom;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorInput : ActorComponent
    {
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

        private float _lastLookInputTime;

#if UNITY_EDITOR
        private bool lockCamera; // lock camera in editor if we press ESC
#endif

        private void Awake()
        {
            _input = new SurgeInput();
            _input.Enable();

            _lastLookInputTime = Time.time - 1f;
        }

        private void OnEnable()
        {
            _input.Enable();
            
            _input.Gameplay.Boost.started += BoostInput;
            _input.Gameplay.Boost.canceled += BoostInput;
            
            _input.Gameplay.Jump.started += JumpInput;
            _input.Gameplay.Jump.canceled += JumpInput;
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
        }

        private void Update()
        {
            var temp = _input.Gameplay.Movement.ReadValue<Vector2>();
            moveVector = new Vector3(temp.x, 0, temp.y);
            lookVector = _input.Gameplay.Camera.ReadValue<Vector2>();
            
            if (lookVector.sqrMagnitude > 0.1f)
            {
                _lastLookInputTime = Time.time;
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                lockCamera = true;
            }
            
            if (Input.GetMouseButtonDown(0))
            {
                lockCamera = false;
            }

            if (lockCamera)
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
            if (actor.flags.HasFlag(FlagType.OutOfControl))
            {
                return;
            }
            
            JumpAction?.Invoke(obj);
        }
    
        private void BoostInput(InputAction.CallbackContext obj)
        {
            if (actor.flags.HasFlag(FlagType.OutOfControl) && !actor.flags.CheckForTag(Tags.AllowBoost))
            {
                return;
            }
            
            BoostAction?.Invoke(obj);
        }

        public float GetLastLookInputTime()
        {
            return _lastLookInputTime;
        }
    }
}