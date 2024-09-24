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
        [SerializeField, Range(0, 1)] private float yFollowTime;
        [SerializeField] private Vector3 lookOffset;
        
        [Header("Collision")]
        public LayerMask collisionMask;
        public float collisionRadius = 0.2f;
        
        [SerializeField] private List<CameraParameters> _parameters;

        private Camera _camera;
        private Transform _cameraTransform;
        private float _x;
        private float _y;
        private float _collisionDistance;
        private bool _autoActive;

        private Vector3 _tempFollowPoint;
        private Vector3 _tempLookPoint;
        private float _tempY;
        private float _tempTime;
        
        private Vector2 _autoLookDirection;

        private CameraParameters _currentParameters;

        private float _followPower;
        private float _fov;
        private float _distance;
        private float _timeToStartFollow;

        private Coroutine _parameterChangeCoroutine;

        private void Awake()
        {
            _camera = Camera.main;
            _cameraTransform = _camera.transform;

            _currentParameters = _parameters[0];

            _tempY = target.position.y;
            _tempTime = yFollowTime;
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
            AutoFollow();
            Following();
            Collision();
            LookAt();
        }

        private void Following()
        {
            var lookVector = actor.input.lookVector;
            _x += lookVector.x + _autoLookDirection.x;
            _y -= lookVector.y;
            _y = Mathf.Clamp(_y, -65, 65);
            
            if (actor.stats.isGrounded)
            {
                _tempTime = Mathf.Lerp(_tempTime, 0, SurgeMath.Smooth(1 - 0.965f));
            }
            else if (actor.stats.isInAir)
            {
                _tempTime = Mathf.Lerp(_tempTime, yFollowTime, SurgeMath.Smooth(1f));
            }

            float yPos = target.position.y;
            _tempY = Mathf.Lerp(_tempY, yPos, SurgeMath.Smooth(1 - _tempTime));
            _tempY = Mathf.Clamp(_tempY, yPos - 0.75f, yPos + 0.5f);
            _tempFollowPoint = target.position;
            _tempFollowPoint.y = _tempY;
        }

        private void AutoFollow()
        {
            if (Common.InDelayTime(actor.input.GetLastLookInputTime(), _timeToStartFollow))
            {
                _autoActive = true;
                
                if (actor.stats.currentSpeed > 1f)
                {
                    if (!(1 - Mathf.Abs(Vector3.Dot(actor.transform.forward, Vector3.up)) < 0.01f))
                    {
                        float fwd = actor.stats.GetForwardSignedAngle() * Time.deltaTime;
                        _autoLookDirection.x = fwd * _currentParameters.followPower; 
                        if (actor.stats.isInAir)
                        {
                            Vector3 vel = Vector3.ClampMagnitude(actor.rigidbody.linearVelocity, 2f);
                            vel.y = Mathf.Lerp(vel.y, Mathf.Clamp(vel.y, -0.5f, 0.15f), SurgeMath.Smooth(1f));
                            lookOffset.y = Mathf.Lerp(lookOffset.y, vel.y, SurgeMath.Smooth(0.05f));
                        }
                        else
                        {
                            lookOffset.y = Mathf.Lerp(lookOffset.y, 0f, 4f * Time.deltaTime);
                        }
                    }
                }
                else
                {
                    _autoLookDirection.x = 0;
                }

                if (actor.stateMachine.CurrentState is FStateGround)
                {
                    _autoLookDirection.y = 4f * (actor.stateMachine.GetSubState<FBoost>().Active ? 1.75f : 1f);
                    _autoLookDirection.y -= actor.stats.currentVerticalSpeed * 1.25f;
                    
                    if (Mathf.Approximately(actor.stats.groundAngle, 90))
                    {
                        _autoLookDirection.y = 0f;
                    }
                }
                else
                {
                    _autoLookDirection.y = 0f;
                }
                
                _y = Mathf.Lerp(_y, _autoLookDirection.y, 2.25f * Time.deltaTime);
            }
            else
            {
                _autoLookDirection.x = 0;
                lookOffset.y = Mathf.Lerp(lookOffset.y, 0f, SurgeMath.Smooth(0.1f));
                
                _autoActive = false;
            }
        }

        private void LookAt()
        {
            Quaternion targetRotation = Quaternion.LookRotation(_tempFollowPoint + lookOffset - _cameraTransform.position);
            _cameraTransform.rotation = targetRotation;
            
            _camera.fieldOfView = _fov;
        }

        private void Collision()
        {
            var ray = new Ray(target.position, -_cameraTransform.forward);
            float radius = collisionRadius;
            var maxDistance = _distance;

            float result = Physics.SphereCast(ray, radius, out RaycastHit hit,
                maxDistance, collisionMask, QueryTriggerInteraction.Ignore)
                ? hit.distance
                : _distance;
            
            _cameraTransform.position = GetTarget(result);
        }

        private Vector3 GetTarget(float distance)
        {
            Vector3 targetPosition = _tempFollowPoint;
            Vector3 v = targetPosition - Quaternion.Euler(_y, _x, 0) * Vector3.forward * distance;
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
        }

        public Camera GetCamera()
        {
            return _camera;
        }

        public Transform GetCameraTransform() => _cameraTransform;
    }
}
