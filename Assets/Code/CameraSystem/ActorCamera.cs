using SurgeEngine.Code.Parameters;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Parameters.SonicSubStates;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace SurgeEngine.Code.CameraSystem
{
    public class ActorCamera : ActorComponent
    {
        [SerializeField] private CameraParameters _defaultParameters;
        [SerializeField] private CameraParameters _boostParameters;
        
        private Camera _camera; 
        private Transform _cameraTransform;
        private float _x;
        private float _y;
        private float _collisionDistance;
        private float _autoLookDirectionX;
        
        private CameraParameters _currentParameters;

        private float _followPower;
        private float _fov;
        private float _distance;

        private void Awake()
        {
            _camera = Camera.main; 
            _cameraTransform = _camera.transform;
            
            _currentParameters = _defaultParameters;
            
            _distance = _currentParameters.distance;
            _fov = _currentParameters.fov;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            
            //actor.stats.boost.OnActiveChanged += OnBoostActivate;
        }

        private void OnEnable()
        {
            if (actor.stats != null) actor.stats.boost.OnActiveChanged += OnBoostActivate;
        }

        private void OnDisable()
        {
            actor.stats.boost.OnActiveChanged -= OnBoostActivate;
        }

        private void Update()
        {
            LerpValues();
            
            Following();
            LookAt();
            Collision();
        }

        private void LerpValues()
        {
            _distance = Mathf.Lerp(_distance, _currentParameters.distance, _currentParameters.distanceChangeSpeed * Time.deltaTime);
            _fov = Mathf.Lerp(_fov, _currentParameters.fov, _currentParameters.fovChangeSpeed * Time.deltaTime); 
        }

        private void Following()
        {
            var lookVector = actor.input.lookVector;
            _x += lookVector.x + _autoLookDirectionX * _currentParameters.followPower;
            _y -= lookVector.y;
            _y = Mathf.Clamp(_y, -35, 50);
            
            _cameraTransform.position = GetTarget();
            
            if (actor.input.GetLastLookInputTime() + _currentParameters.timeToStartFollow < Time.time)
            {
                float dot = Vector3.Dot(actor.stats.inputDir, actor.transform.forward);
                bool enable = dot is < 0.999f and > -0.7f && actor.stats.inputDir.magnitude > 0.2f;
                if (actor.stats.planarVelocity.magnitude > 1f && enable)
                {
                    float fwd = actor.stats.GetForwardSignedAngle();
                    _autoLookDirectionX = Mathf.Lerp(_autoLookDirectionX, fwd, 12 * Time.deltaTime);
                }
                else
                {
                    _autoLookDirectionX = Mathf.Lerp(_autoLookDirectionX, 0, 6 * Time.deltaTime);
                }
            }
            else
            {
                _autoLookDirectionX = 0;
            }
        }

        private void LookAt()
        {
            Quaternion quaternion = Quaternion.LookRotation(_currentParameters.target.position - _cameraTransform.position);
            _cameraTransform.rotation = quaternion;
            
            _camera.fieldOfView = _fov;
        }

        private void Collision()
        {
            var ray = new Ray(_currentParameters.target.position, -_cameraTransform.forward);
            float radius = _currentParameters.collisionRadius;
            var maxDistance = Vector3.Distance(_currentParameters.target.position, _cameraTransform.position);

            float result = Physics.SphereCast(ray, radius, out RaycastHit hit, 
                maxDistance, _currentParameters.collisionMask) 
                ? hit.distance
                : _distance;
            
            _cameraTransform.position = _currentParameters.target.position - _cameraTransform.forward * result;
        }

        private Vector3 GetTarget()
        {
            Vector3 v = actor.transform.position + Quaternion.Euler(_y, _x, 0) * new Vector3(0, 0, -_distance);
            return v;
        }

        private void OnBoostActivate(FSubState obj, bool value)
        {
            if (obj is FBoost && value)
            {
                _currentParameters = _boostParameters;
            }
            else 
            {
                _currentParameters = _defaultParameters;
            }
        }

        public Camera GetCamera()
        {
            return _camera;
        }

        public Transform GetCameraTransform() => _cameraTransform;
    }
}
