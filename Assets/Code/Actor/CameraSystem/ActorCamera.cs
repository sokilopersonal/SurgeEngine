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
        public Transform target;
        public float yFollowTime;
        public float zLagMax = 0.5f;
        public float zLagSmoothness = 2.5f;
        public Vector3 lookOffset;
        
        [Header("Collision")]
        public LayerMask collisionMask;
        public float collisionRadius = 0.2f;

        public List<CameraParameters> parameters;
        
        private Camera _camera;
        private Transform _cameraTransform;

        private void Awake()
        {
            _camera = Camera.main;
            _cameraTransform = _camera.transform;
            stateMachine = new MasterCamera(_camera, _cameraTransform);
            
            stateMachine.AddState(new NewModernState(_camera, _cameraTransform, actor));
            stateMachine.SetState<NewModernState>();

            // stateMachine.AddState(new DefaultModernPawn(actor));
            // stateMachine.AddState(new CameraPan(actor));
            // stateMachine.AddState(new VerticalCameraPan(actor));
            // stateMachine.AddState(new RestoreCameraPawn(actor));
            //
            // stateMachine.SetState<DefaultModernPawn>();
        }

        public void OnInit() {}

        private void Update()
        {
            stateMachine.Tick(Time.unscaledDeltaTime);
        }
        
        private void FixedUpdate()
        {
            stateMachine.FixedTick(Time.fixedUnscaledDeltaTime);
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
