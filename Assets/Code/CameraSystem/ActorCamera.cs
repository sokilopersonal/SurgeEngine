using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.CameraSystem
{
    public class ActorCamera : ActorComponent
    {
        [SerializeField] private Transform target;
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private float distance;
        
        private Transform _cameraTransform;
        private float _x;
        private float _y;
        private float _collisionDistance;
        private Vector2 _lookRotation;

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
            _x += lookVector.x + _lookRotation.x * 0.01f;
            _y -= lookVector.y;
            const float clampValue = 90f * 0.99f;
            _y = Mathf.Clamp(_y, -clampValue, clampValue);
            
            _cameraTransform.position = GetTarget();
            //_lookRotation.x = actor.stats.GetSignedAngle();
        }

        private void LookAt()
        {
            Quaternion quaternion = Quaternion.LookRotation(target.position - _cameraTransform.position);
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
