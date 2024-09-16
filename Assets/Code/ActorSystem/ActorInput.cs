using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorInput : ActorComponent
    {
        public Vector3 moveVector;
        public Vector2 lookVector;
        
        private SurgeInput _input;

#if UNITY_EDITOR
        private bool lockCamera; // lock camera in editor if we press ESC
#endif

        private void Awake()
        {
            _input = new SurgeInput();
            _input.Enable();
        }

        private void OnEnable()
        {
            _input.Enable();
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
    }
}