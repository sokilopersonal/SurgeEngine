using System.Collections.Generic;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CameraSystem.Pawns;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem
{
    public class ActorCamera : ActorComponent
    {
        public FStateMachine stateMachine;
        
        [Header("Target")] 
        public Transform target;
        public float yFollowTime;
        public float zLagMax = 0.5f;
        public float zLagSmoothness = 2.5f;
        public Vector3 lookOffset;
        
        [Header("Collision")]
        public LayerMask collisionMask;
        public float collisionRadius = 0.2f;
        
        [SerializeField] private CameraPawn[] pawns;
        public List<CameraParameters> parameters;
        
        private Camera _camera;
        private Transform _cameraTransform;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            
            _camera = Camera.main;
            _cameraTransform = _camera.transform;
        }

        private void Awake()
        {
            stateMachine = new FStateMachine();

            // foreach (var pawn in pawns)
            // {
            //     pawn.SetOwner(actor);
            //     pawn.Initialize(_camera, _cameraTransform);
            //     stateMachine.AddState(pawn);
            // }
            
            stateMachine.AddState(new DefaultModernPawn(actor));
            stateMachine.AddState(new CameraPan(actor));
            stateMachine.AddState(new VerticalCameraPan(actor));
            stateMachine.AddState(new RestoreCameraPawn(actor));

            stateMachine.SetState<DefaultModernPawn>();
        }

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
