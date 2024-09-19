using System;
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
            
            _input.Gameplay.Boost.started += context => BoostAction?.Invoke(context);
            _input.Gameplay.Boost.canceled += context => BoostAction?.Invoke(context);
        }

        private void OnDisable()
        {
            _input.Disable();
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
        }
        
        public float GetLastLookInputTime()
        {
            return _lastLookInputTime;
        }
    }
}