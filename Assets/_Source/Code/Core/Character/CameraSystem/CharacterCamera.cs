using System;
using System.Collections.Generic;
using System.Reflection;
using SurgeEngine._Source.Code.Core.Character.CameraSystem.Modifiers;
using SurgeEngine._Source.Code.Core.Character.CameraSystem.Pans;
using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.CameraObjects;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine._Source.Code.Infrastructure.Tools.Managers;
using UnityEngine;
using Zenject;

namespace SurgeEngine._Source.Code.Core.Character.CameraSystem
{
    public class CharacterCamera : CharacterComponent, IPointMarkerLoader
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

        [SerializeField] private bool showDebugText;

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
        public float UserSensitivityMultiplier => _userInput.GetData().Sensitivity.Value;
        
        private Camera _camera;
        private Transform _cameraTransform;

        [Inject] private UserInput _userInput;

        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            
            _camera = Camera.main;
            _cameraTransform = _camera.transform;
        }

        internal override void Set(CharacterBase character)
        {
            base.Set(character);
            
            foreach (var modifier in baseCameraModifiers)
            {
                modifier.Set(this.character);
                _modifiersDictionary.Add(modifier.GetType(), modifier);
            }
        }

        private void Start()
        {
            StateMachine = new CameraStateMachine(_camera, _cameraTransform, character);
            
            StateMachine.AddState(new CameraAnimState(character));
            StateMachine.AddState(new NewModernState(character));
            StateMachine.AddState(new CameraPan(character));
            StateMachine.AddState(new ParallelCameraPan(character));
            StateMachine.AddState(new FixedCameraPan(character));
            StateMachine.AddState(new NormalCameraPan(character));
            StateMachine.AddState(new FallCameraState(character));
            StateMachine.AddState(new PointCameraPan(character));
            StateMachine.AddState(new PathCameraPan(character));
            StateMachine.AddState(new PathTargetCameraPan(character));

            var start = character.GetStartData();
            if (start.startType == StartType.None || start.startType == StartType.Dash)
            {
                StateMachine.SetState<NewModernState>();
            }
            else
            {
                StateMachine.SetState<CameraAnimState>();
            }

            Vector3 dir = Quaternion.LookRotation(character.transform.forward).eulerAngles;
            dir.x = yawDefaultAmplitude;
            StateMachine.SetDirection(dir.y, dir.x);
            
            foreach (var volume in FindObjectsByType<ChangeCameraVolume>(FindObjectsInactive.Include, FindObjectsSortMode.None))
            {
                if (volume.GetComponent<BoxCollider>().bounds.Contains(character.transform.position))
                {
                    StateMachine.RegisterVolume(volume);
                }
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

        public void Load()
        {
            StateMachine.SetDirection(character.transform.forward);
            
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
                    
                    var blendFactor = (float)StateMachine.GetType().GetField("_interpolatedBlendFactor",
                        BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(StateMachine)!;
                    
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
