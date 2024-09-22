using System;
using System.Collections;
using System.Collections.Generic;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.Parameters.SonicSubStates;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace SurgeEngine.Code.CameraSystem
{
    public class ActorCamera : ActorComponent
    {
        [Header("Target")]
        public Transform target;
        
        [Header("Collision")]
        public LayerMask collisionMask;
        public float collisionRadius = 0.2f;
        
        [SerializeField] private List<CameraParameters> _parameters;

        private Camera _camera;
        private Transform _cameraTransform;
        private NormalCamera _normalCamera;
        private float _x;
        private float _y;
        private float _collisionDistance;
        private float _timeToStartFollow;
        
        private Vector2 _autoLookDirection;

        private CameraParameters _currentParameters;

        private float _followPower;
        private float _fov;
        private float _distance;

        private Coroutine _parameterChangeCoroutine;

        private void Awake()
        {
            _camera = Camera.main;
            _cameraTransform = _camera.transform;
            _normalCamera = _camera.GetComponentInParent<NormalCamera>();

            _currentParameters = _parameters[0];
            _timeToStartFollow = _currentParameters.timeToStartFollow;
            
            _distance = _currentParameters.distance;
            _fov = _currentParameters.fov;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            
            Quaternion dir = Quaternion.LookRotation(actor.transform.forward);
            _x = dir.eulerAngles.y;
            _y = dir.eulerAngles.x;
        }

        private void OnEnable()
        {
            if (actor.stats != null) actor.stateMachine.GetSubState<FBoost>().OnActiveChanged += OnBoostActivate;
        }

        private void OnDisable()
        {
            actor.stateMachine.GetSubState<FBoost>().OnActiveChanged -= OnBoostActivate;
        }

        private void Update()
        {
            Following();
            LookAt();
            Collision();
        }

        private void Following()
        {
            if (Common.InDelayTime(actor.input.GetLastLookInputTime(), _timeToStartFollow) && !_normalCamera.active)
            {
                if (actor.stats.currentSpeed > 1f)
                {
                    if (!(1 - Mathf.Abs(Vector3.Dot(actor.transform.forward, Vector3.up)) < 0.01f))
                    {
                        float fwd = actor.stats.GetForwardSignedAngle();
                        _autoLookDirection.x = fwd * _currentParameters.followPower * Time.deltaTime;
                    }
                }
                else
                {
                    _autoLookDirection.x = 0;
                }
                
                _autoLookDirection.y = 11;
                if (actor.stats.groundAngle < 15) _y = Mathf.Lerp(_y, _autoLookDirection.y, 
                    actor.stats.currentSpeed * 0.1f * Time.deltaTime);
            }
            else
            {
                _autoLookDirection.x = 0;
            }
            
            var lookVector = actor.input.lookVector;
            _x += lookVector.x + _autoLookDirection.x;
            _y -= lookVector.y;
            _y = Mathf.Clamp(_y, -35, 50);

            _cameraTransform.localPosition = GetTarget();
        }

        private void LookAt()
        {
            Quaternion quaternion = Quaternion.LookRotation(target.position - _cameraTransform.position, 
                _cameraTransform.parent.up);
            _cameraTransform.rotation = quaternion;

            _camera.fieldOfView = _fov;
        }

        private void Collision()
        {
            var ray = new Ray(target.position, -_cameraTransform.forward);
            float radius = collisionRadius;
            var maxDistance = Vector3.Distance(target.position, _cameraTransform.position);

            float result = Physics.SphereCast(ray, radius, out RaycastHit hit,
                maxDistance, collisionMask, QueryTriggerInteraction.Ignore)
                ? hit.distance
                : _distance;

            _cameraTransform.position = target.position - _cameraTransform.forward * result;
        }

        private Vector3 GetTarget()
        {
            Vector3 v = Quaternion.Euler(_y, _x, 0) * new Vector3(0, 0, -_distance);
            return v;
        }

        private void OnBoostActivate(FSubState obj, bool value)
        {
            if (_parameterChangeCoroutine != null)
            {
                StopCoroutine(_parameterChangeCoroutine);
            }
            
            if (obj is FBoost && value)
            {
                TransitionTo("BoostOut", _ =>
                {
                    TransitionTo("BoostIn");
                });
            }
            else
            {
                TransitionTo("Default");
            }
        }

        private void TransitionTo(string parameter, Action<CameraParameters> callback = null)
        {
            if (_parameterChangeCoroutine != null)
            {
                StopCoroutine(_parameterChangeCoroutine);
            }

            var param = _parameters.Find(x => x.name == parameter);
            _parameterChangeCoroutine = StartCoroutine(ChangeParametersCoroutine(param, callback));
        }

        private IEnumerator ChangeParametersCoroutine(CameraParameters target, Action<CameraParameters> callback = null)
        {
            float tDistance = 0f;
            float tFov = 0f;
            float tFollow = 0f;

            float startDistance = _distance;
            float startFov = _fov;
            float startFollow = _timeToStartFollow;
            
            float targetDistance = target.distance;
            float targetFov = target.fov;
            float targetFollow = target.timeToStartFollow;

            while (tDistance < target.distanceDuration || tFov < target.fovDuration || tFollow < targetFollow)
            {
                if (tDistance < target.distanceDuration)
                {
                    tDistance += Time.deltaTime;
                    _distance = Mathf.Lerp(startDistance, targetDistance, Easings.Get(target.distanceEasing, tDistance / target.distanceDuration));
                }

                if (tFov < target.fovDuration)
                {
                    tFov += Time.deltaTime;
                    _fov = Mathf.Lerp(startFov, targetFov, Easings.Get(target.fovEasing, tFov / target.fovDuration));
                }
                
                if (tFollow < targetFollow)
                {
                    tFollow += Time.deltaTime;
                    _timeToStartFollow = Mathf.Lerp(startFollow, targetFollow, Easings.Get(Easing.Linear, tFollow / targetFollow));
                }

                yield return null;
            }

            _distance = targetDistance;
            _fov = targetFov;

            _currentParameters = target;
            callback?.Invoke(target);
            Debug.Log($"[Actor Camera] Camera changed to {_currentParameters.name}");
        }

        public Camera GetCamera()
        {
            return _camera;
        }
        
        public NormalCamera GetNormalCamera()
        {
            return _normalCamera;
        }

        public Transform GetCameraTransform() => _cameraTransform;
    }
}
