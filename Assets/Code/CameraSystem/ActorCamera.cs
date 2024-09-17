using SurgeEngine.Code.ActorSystem;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace SurgeEngine.Code.CameraSystem
{
    public class ActorCamera : ActorComponent
    {
        [Header("Target")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset;
        
        [Header("Collision")]
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private float collisionRadius;
        
        [Header("Follow")]
        [SerializeField] private float distance = 2.4f;
        [SerializeField, Range(0, 0.05f)] private float followPower = 0.02f;
        [SerializeField] private float timeToStartFollow = 2f;
        
        private Transform _cameraTransform;
        private float _x;
        private float _y;
        private float _collisionDistance;
        private float _autoLookDirectionX;

        private void Awake()
        {
            _cameraTransform = Camera.main.transform;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Update()
        {
            Following();
            LookAt();
            Collision();
        }

        private void Following()
        {
            var lookVector = actor.input.lookVector;
            _x += lookVector.x + _autoLookDirectionX * followPower;
            _y -= lookVector.y;
            _y = Mathf.Clamp(_y, -25, 50);
            
            _cameraTransform.position = GetTarget() + offset;
            
            if (actor.input.GetLastLookInputTime() + timeToStartFollow < Time.time)
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
            Quaternion quaternion = Quaternion.LookRotation(target.position + offset - _cameraTransform.position);
            _cameraTransform.rotation = quaternion;
        }

        private void Collision()
        {
            var ray = new Ray(target.position, -_cameraTransform.forward);
            float radius = 0.2f;
            var maxDistance = Vector3.Distance(target.position, _cameraTransform.position);

            float result = Physics.SphereCast(ray, radius, out RaycastHit hit, 
                maxDistance, collisionMask) 
                ? hit.distance
                : distance;
            
            _cameraTransform.position = target.position - _cameraTransform.forward * result;
        }

        private Vector3 GetTarget()
        {
            Vector3 v = actor.transform.position + Quaternion.Euler(_y, _x, 0) * new Vector3(0, 0, -distance);
            return v;
        }

        public Transform GetCameraTransform() => _cameraTransform;
    }
}
