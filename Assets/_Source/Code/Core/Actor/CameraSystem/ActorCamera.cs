using System;
using System.Collections.Generic;
using SurgeEngine.Code.Core.Actor.CameraSystem.Modifiers;
using SurgeEngine.Code.Core.Actor.CameraSystem.Pans;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.CameraSystem
{
    public class ActorCamera : ActorComponent, IPointMarkerLoader
    {
        public CameraStateMachine StateMachine { get; private set; }
        
        [Header("Input")]
        [SerializeField] private float sensitivity = 0.5f;
        [SerializeField] private float maxSensitivitySpeed = 1f;
        [SerializeField] private float minSensitivitySpeed = 0.5f;

        [Header("Target")] 
        [SerializeField] private float distance = 2.9f;
        [SerializeField] private float yOffset = 0.1f;

        [Header("Auto Look")] 
        [SerializeField] private float pitchAutoLookAmplitude = 4f;
        [SerializeField] private float pitchAutoLookMinAmplitude = 0.2f;
        [SerializeField] private float yawDefaultAmplitude = 7f;
        [SerializeField] private float yawMinAmplitude = -5f;
        [SerializeField] private float yawMaxAmplitude = 5f;
        [SerializeField] private float yawMinLerpSpeed = 0.75f;
        [SerializeField] private float yawLerpSpeed = 1.65f;
        
        [Header("Z Lag")]
        [SerializeField] private float zLagMax = 0.5f;
        [SerializeField, Range(0, 1)] private float zLagTime = 0.5f;
        
        [Header("Y Lag")]
        [SerializeField] private float yLagMin = -1f;
        [SerializeField] private float yLagMax = 0.5f;
        [SerializeField, Range(0, 1)] private float yLagTime = 0.1f;
        
        [Header("Lateral Offset")]
        [SerializeField] private AnimationCurve lateralOffsetSpeedCurve;
        
        [Header("Collision")]
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private float collisionRadius = 0.2f;

        [Header("Modifiers")] 
        [SerializeField] private List<BaseCameraModifier> baseCameraModifiers;
        private readonly Dictionary<Type, BaseCameraModifier> _modifiersDictionary = new();

        public float Sensitivity => sensitivity;
        public float MaxSensitivitySpeed => maxSensitivitySpeed;
        public float MinSensitivitySpeed => minSensitivitySpeed;
        public float Distance => distance;
        public float YOffset => yOffset;
        public float PitchAutoLookAmplitude => pitchAutoLookAmplitude;
        public float PitchAutoLookMinAmplitude => pitchAutoLookMinAmplitude;
        public float YawDefaultAmplitude => yawDefaultAmplitude;
        public float YawMinAmplitude => yawMinAmplitude;
        public float YawMaxAmplitude => yawMaxAmplitude;
        public float YawMinLerpSpeed => yawMinLerpSpeed;
        public float YawLerpSpeed => yawLerpSpeed;
        public float ZLagMax => zLagMax;
        public float ZLagTime => zLagTime;
        public float YLagMin => yLagMin;
        public float YLagMax => yLagMax;
        public float YLagTime => yLagTime;
        public LayerMask CollisionMask => collisionMask;
        public float CollisionRadius => collisionRadius;
        public AnimationCurve LateralOffsetSpeedCurve => lateralOffsetSpeedCurve;
        
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
            StateMachine = new CameraStateMachine(_camera, _cameraTransform, Actor, this);
            
            StateMachine.AddState(new CameraAnimState(Actor));
            StateMachine.AddState(new NewModernState(Actor));
            StateMachine.AddState(new CameraPan(Actor));
            StateMachine.AddState(new VerticalCameraPan(Actor));
            StateMachine.AddState(new FixedCameraPan(Actor));
            StateMachine.AddState(new NormalCameraPan(Actor));
            StateMachine.AddState(new RestoreCameraPawn(Actor));
            StateMachine.AddState(new FallCameraState(Actor));
            StateMachine.AddState(new PointCameraPan(Actor));

            var start = Actor.GetStartData();
            if (start.startType == StartType.None || start.startType == StartType.Dash)
            {
                StateMachine.SetState<NewModernState>();
            }
            else
            {
                StateMachine.SetState<CameraAnimState>();
            }

            Vector3 dir = Quaternion.LookRotation(Actor.transform.forward).eulerAngles;
            dir.x = yawDefaultAmplitude;
            StateMachine.SetDirection(dir.y, dir.x);
        }

        private void Update()
        {
            StateMachine.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            StateMachine.FixedTick(Time.fixedDeltaTime);
        }

        private void LateUpdate()
        {
            StateMachine.LateTick(Time.deltaTime);
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

        public void Load(Vector3 loadPosition, Quaternion loadRotation)
        {
            StateMachine.SetState<NewModernState>();
        }
    }
}
