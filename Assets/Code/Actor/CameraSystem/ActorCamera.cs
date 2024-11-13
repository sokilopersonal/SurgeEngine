using System.Collections.Generic;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CameraSystem.Pawns;
using SurgeEngine.Code.Parameters;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem
{
    public class ActorCamera : MonoBehaviour, IActorComponent
    {
        public Actor actor { get; set; }
        
        public MasterCamera stateMachine;

        [Header("Target")] 
        public float distance = 2.9f;
        public float yOffset = 0.1f;
        
        [Header("Z Lag")]
        public float zLagMax = 0.5f;
        [Range(0, 1)] public float zLagTime = 0.5f;
        
        [Header("Y Lag")]
        public float yLagMin = -1f;
        public float yLagMax = 0.5f;
        [Range(0, 1)] public float yLagTime = 0.1f;
        
        public Vector3 lookOffset;
        
        [Header("Collision")]
        public LayerMask collisionMask;
        public float collisionRadius = 0.2f;
        
        private Camera _camera;
        private Transform _cameraTransform;

        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            _camera = Camera.main;
            _cameraTransform = _camera.transform;
            stateMachine = new MasterCamera(_camera, _cameraTransform, this);
            
            stateMachine.AddState(new NewModernState(actor));
            stateMachine.AddState(new CameraPan(actor));
            stateMachine.AddState(new VerticalCameraPan(actor));
            stateMachine.AddState(new FixedCameraPan(actor));
            stateMachine.AddState(new RestoreCameraPawn(actor));
            
            stateMachine.SetState<NewModernState>();
        }

        public void OnInit() {}

        private void Update()
        {
            stateMachine.Tick(Time.deltaTime);
        }
        
        private void FixedUpdate()
        {
            stateMachine.FixedTick(Time.fixedDeltaTime);
        }
        
        private void LateUpdate()
        {
            stateMachine.LateTick(Time.unscaledDeltaTime);
        }

        public Camera GetCamera()
        {
            return _camera;
        }

        public Transform GetCameraTransform() => _cameraTransform;
    }
}
