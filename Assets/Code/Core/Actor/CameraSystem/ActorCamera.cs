using System;
using System.Collections.Generic;
using SurgeEngine.Code.Core.Actor.CameraSystem.Modifiers;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pawns;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem
{
    public class ActorCamera : ActorComponent
    {
        public CameraStateMachine stateMachine;
        
        [Header("Input")]
        public float sensitivity = 0.5f;
        public float maxSensitivitySpeed = 1f;
        public float minSensitivitySpeed = 0.5f;

        [Header("Target")] 
        public float distance = 2.9f;
        public float yOffset = 0.1f;
        public Vector3 lookOffset;

        [Header("Auto Look")] 
        public float horizontalAutoLookAmplitude = 4f;
        public float horizontalAutoLookMinAmplitude = 0.2f;
        public float verticalDefaultAmplitude = 7f;
        public float verticalMinAmplitude = -5f;
        public float verticalMaxAmplitude = 5f;
        public float verticalMinLerpSpeed = 0.75f;
        public float verticalLerpSpeed = 1.65f;
        
        [Header("Z Lag")]
        public float zLagMax = 0.5f;
        [Range(0, 1)] public float zLagTime = 0.5f;
        
        [Header("Y Lag")]
        public float yLagMin = -1f;
        public float yLagMax = 0.5f;
        [Range(0, 1)] public float yLagTime = 0.1f;
        
        [Header("Lateral Offset")]
        [SerializeField] private AnimationCurve lateralOffsetSpeedCurve;
        public AnimationCurve LateralOffsetSpeedCurve => lateralOffsetSpeedCurve;
        
        [Header("Collision")]
        public LayerMask collisionMask;
        public float collisionRadius = 0.2f;

        [Header("Modifiers")] 
        [SerializeField] private List<BaseCameraModifier> baseCameraModifiers;
        private readonly Dictionary<Type, BaseCameraModifier> _modifiersDictionary = new();
        
        private Camera _camera;
        private Transform _cameraTransform;

        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            _camera = Camera.main;
            _cameraTransform = _camera.transform;
        }

        internal override void Set(ActorBase actor)
        {
            base.Set(actor);
            
            foreach (var modifier in baseCameraModifiers)
            {
                modifier.Set(Actor);
                _modifiersDictionary.Add(modifier.GetType(), modifier);
            }
        }

        public void Start()
        {
            stateMachine = new CameraStateMachine(_camera, _cameraTransform, this);
            
            stateMachine.AddState(new NewModernState(Actor));
            stateMachine.AddState(new CameraPan(Actor));
            stateMachine.AddState(new VerticalCameraPan(Actor));
            stateMachine.AddState(new FixedCameraPan(Actor));
            stateMachine.AddState(new RestoreCameraPawn(Actor));

            stateMachine.SetState<NewModernState>();
            stateMachine.SetDirection(Actor.transform.forward);
        }

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
            stateMachine.LateTick(Time.deltaTime);
        }
        
        public Camera GetCamera()
        {
            return _camera;
        }

        public Transform GetCameraTransform() => _cameraTransform;
        
        public T GetModifier<T>() where T : BaseCameraModifier => _modifiersDictionary[typeof(T)] as T;
        public T GetModifier<T>(out T modifier) where T : BaseCameraModifier
        {
            if (_modifiersDictionary.TryGetValue(typeof(T), out var value))
            {
                modifier = value as T;
                return modifier;
            }
            
            modifier = null;
            return null;
        }
    }
}
