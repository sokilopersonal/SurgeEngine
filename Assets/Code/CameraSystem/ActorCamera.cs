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

        private void Awake()
        {
            _cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            Following();
            LookAt();
            Collision();
        }

        private void Following()
        {
            var lookVector = owner.input.lookVector;
            _x += lookVector.x;
            _y -= lookVector.y;
            const float clampValue = 90f * 0.99f;
            _y = Mathf.Clamp(_y, -clampValue, clampValue);
            
            _cameraTransform.position = GetTarget();
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
            Vector3 v = owner.transform.position + Quaternion.Euler(_y, _x, 0) * new Vector3(0, 0, -distance);
            return v;
        }

        public Transform GetCameraTransform() => _cameraTransform;
    }
}
