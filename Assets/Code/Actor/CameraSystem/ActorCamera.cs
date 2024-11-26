using System.Collections;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CameraSystem.Pawns;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters.SonicSubStates;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem
{
    public class ActorCamera : ActorComponent
    {
        public MasterCamera stateMachine;
        
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

        [Header("Boost Blend")] 
        public AnimationCurve boostBlendCurve;
        public AnimationCurve boostBlendFovCurve;
        public AnimationCurve boostDeblendCurve;
        [SerializeField] private float boostBlendTime = 4f;
        [SerializeField] private float boostDeblendTime = 1f;
        public float boostBlendFactor { get; private set; }
        private Coroutine _boostBlendCoroutine;
        
        [Header("Collision")]
        public LayerMask collisionMask;
        public float collisionRadius = 0.2f;
        
        private Camera _camera;
        private Transform _cameraTransform;

        private void Awake()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void Start()
        {
            _camera = Camera.main;
            _cameraTransform = _camera.transform;
            stateMachine = new MasterCamera(_camera, _cameraTransform, this);
            
            stateMachine.AddState(new NewModernState(actor));
            stateMachine.AddState(new CameraPan(actor));
            stateMachine.AddState(new VerticalCameraPan(actor));
            stateMachine.AddState(new FixedCameraPan(actor));
            stateMachine.AddState(new RestoreCameraPawn(actor));
            
            stateMachine.SetState<NewModernState>().SetDirection(transform.forward);
            
            ActorContext.Context.stateMachine.GetSubState<FBoost>().OnActiveChanged += (state, value) => 
            {
                if (_boostBlendCoroutine != null)
                    StopCoroutine(_boostBlendCoroutine);
                
                _boostBlendCoroutine = StartCoroutine(OnBoostActivate(state, value));
            };
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

        private IEnumerator OnBoostActivate(FSubState sub, bool active)
        {
            if (sub is FBoost)
            {
                float t = 0;
                float blendTime = active ? boostBlendTime : boostDeblendTime;
                float lastBlendFactor = boostBlendFactor;
                NewModernState state = stateMachine.GetState<NewModernState>();
                float lastDistance = state.GetBoostDistance();
                float baseFov = 60f;
                float lastFov = _camera.fieldOfView;
                while (t < 1f)
                {
                    t += Time.deltaTime / blendTime;

                    if (active)
                    {
                        boostBlendFactor = t;
                        state.SetBoostDistance(boostBlendCurve.Evaluate(t));
                        _camera.fieldOfView = baseFov * boostBlendFovCurve.Evaluate(t);
                    }
                    else
                    {
                        boostBlendFactor = Mathf.Lerp(lastBlendFactor, 0f, boostDeblendCurve.Evaluate(t));
                        state.SetBoostDistance(Mathf.Lerp(lastDistance, 1f, boostDeblendCurve.Evaluate(t)));
                        _camera.fieldOfView = Mathf.Lerp(lastFov, baseFov, boostDeblendCurve.Evaluate(t));
                    }
                    
                    yield return null;
                }
            }
        }
        
        public Camera GetCamera()
        {
            return _camera;
        }

        public Transform GetCameraTransform() => _cameraTransform;

        private void OnDrawGizmos()
        {
            if (Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(transform.position + transform.TransformDirection(lookOffset), 0.2f);
            }
        }
        
    }
}
