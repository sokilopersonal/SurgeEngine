using System;
using System.Collections.Generic;
using SurgeEngine.Source.Code.Core.Character.CameraSystem.Modifiers;
using SurgeEngine.Source.Code.Core.Character.CameraSystem.Pans;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using SurgeEngine.Source.Code.Infrastructure.Tools.Managers;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Source.Code.Core.Character.CameraSystem
{
    public class CharacterCamera : CharacterComponent, IPointMarkerLoader
    {
        public CameraStateMachine StateMachine { get; private set; }
        
        [Header("Input")]
        [SerializeField] private float sensitivity = 0.5f;

        [Header("Target")] 
        [SerializeField] private float distance = 2.9f;
        [SerializeField] private float yOffset = 0.1f;

        [Header("Auto Look")] 
        [SerializeField] private float pitchAutoLookAmplitude = 4f;
        [SerializeField] private float pitchAutoLookMinAmplitude = 0.2f;
        [SerializeField] private float yawDefaultAmplitude = 7f;
        [SerializeField] private float yawMinAmplitude = -5f;
        [SerializeField] private float yawMaxAmplitude = 5f;
        
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
        private readonly Dictionary<Type, BaseCameraModifier> _modifiersDictionary = new();

        [SerializeField] private bool showDebugText;

        public float Sensitivity => sensitivity;
        public float Distance => distance;
        public float YOffset => yOffset;
        public float PitchAutoLookAmplitude => pitchAutoLookAmplitude;
        public float PitchAutoLookMinAmplitude => pitchAutoLookMinAmplitude;
        public float YawDefaultAmplitude => yawDefaultAmplitude;
        public float YawMinAmplitude => yawMinAmplitude;
        public float YawMaxAmplitude => yawMaxAmplitude;
        public float ZLagMax => zLagMax;
        public float ZLagTime => zLagTime;
        public float YLagMin => yLagMin;
        public float YLagMax => yLagMax;
        public float YLagTime => yLagTime;
        public LayerMask CollisionMask => collisionMask;
        public float CollisionRadius => collisionRadius;
        public AnimationCurve LateralOffsetSpeedCurve => lateralOffsetSpeedCurve;
        public float UserSensitivityMultiplier => _userInput.GetData().Sensitivity.Value;
        
        private Camera _camera;
        private Transform _cameraTransform;

        [Inject] private UserInput _userInput;

        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            _camera = Camera.main;
            if (_camera != null) _cameraTransform = _camera.transform;
            else
            {
                Debug.LogError("For some reason, there is no camera...");
            }

            var cameraModifiers = _cameraTransform.GetComponentsInChildren<BaseCameraModifier>();
            foreach (var modifier in cameraModifiers)
            {
                modifier.Set(Character);
                _modifiersDictionary.Add(modifier.GetType(), modifier);
            }
        }

        private void Start()
        {
            StateMachine = new(_camera, _cameraTransform, Character);
            
            StateMachine.AddState(new CameraAnimState(Character));
            StateMachine.AddState(new NewModernState(Character));
            StateMachine.AddState(new Camera2DState(Character));
            StateMachine.AddState(new CameraPan(Character));
            StateMachine.AddState(new ParallelCameraPan(Character));
            StateMachine.AddState(new FixedCameraPan(Character));
            StateMachine.AddState(new NormalCameraPan(Character));
            StateMachine.AddState(new FallCameraState(Character));
            StateMachine.AddState(new PointCameraPan(Character));
            StateMachine.AddState(new PathCameraPan(Character));
            StateMachine.AddState(new PathTargetCameraPan(Character));

            var start = Character.GetStartData();
            if (start.startType == StartType.None || start.startType == StartType.Dash)
            {
                StateMachine.SetState<NewModernState>();
            }
            else
            {
                StateMachine.SetState<CameraAnimState>();
            }

            Vector3 dir = Quaternion.LookRotation(Character.transform.forward).eulerAngles;
            dir.x = yawDefaultAmplitude;
            StateMachine.SetDirection(dir.y, dir.x);

            foreach (var volume in Utility.GetVolumesInBounds(Character.transform.position))
            {
                StateMachine.RegisterVolume(volume);
            }
            
            StateMachine.CompleteBlend();
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

        public void Load()
        {
            StateMachine.Load();
        }

        private void OnGUI()
        {
            if (Debug.isDebugBuild && showDebugText && StateMachine != null && StateMachine.CurrentState != null)
            {
                var style = new GUIStyle();
                style.stretchWidth = true;
                style.stretchHeight = true;
                style.normal.background = new Texture2D(1, 1);
                style.normal.background.SetPixel(0, 0, new Color(0.8f, 0.8f, 0.8f, 0.75f));
                style.normal.background.Apply();

                var labelStyle = new GUIStyle();
                labelStyle.fontSize = 16; 
                labelStyle.normal.textColor = Color.black;
                
                using (new GUILayout.VerticalScope(style))
                {
                    GUILayout.BeginVertical();
                    
                    var blendFactor = StateMachine.BlendFactor;
                    
                    string mCameraLabel = $"SE Camera: {StateMachine.CurrentState.GetType().Name}";
                    if (StateMachine.Top)
                    {
                        if (blendFactor < 1) mCameraLabel = $"SE Camera currently blending to: {StateMachine.Top.Target.name}";
                        else mCameraLabel = $"SE Camera: {StateMachine.Top.Target.name}";
                    }
                    
                    GUILayout.Label(mCameraLabel, labelStyle);
                    if (blendFactor < 1) GUILayout.Label($"Blend Progress %{Mathf.RoundToInt(blendFactor * 100)}", labelStyle);
                    GUILayout.Label($"Field Of View: {Mathf.RoundToInt(_camera.fieldOfView)}", labelStyle);
                    GUILayout.EndVertical();
                }
            }
        }
    }
}
