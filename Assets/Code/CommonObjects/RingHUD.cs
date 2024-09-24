using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class RingHUD : MonoBehaviour
    {
        private float _factor;

        private Camera _camera => ActorContext.Context.camera.GetCamera();
        
        private Vector3 _initialPosition;
        private Vector3 _targetPosition;
        private Vector3 _initialScale;
        
        private Quaternion _initialRotation;
        private Quaternion _targetRotation;

        private float time;
        
        private float rotationDuration = 1.0f;
        private bool shouldRotate;

        public void Initialize(float time)
        {
            transform.localScale = Vector3.one * 1.5f;
            transform.parent = _camera.transform;

            this.time = time;

            _initialPosition = transform.position;
            _initialRotation = transform.rotation;
            _initialScale = transform.localScale;
            
            shouldRotate = Random.value < 0.5f;
            rotationDuration = 15f * Mathf.RoundToInt(Random.Range(1, 4));
            rotationDuration *= Random.value < 0.5f ? 1 : -1;

            if (shouldRotate)
            {
                Matrix4x4 viewMatrix = _camera.worldToCameraMatrix;
                Vector3 cameraForward = viewMatrix.inverse.MultiplyVector(_camera.transform.right);
                Quaternion targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
                transform.rotation = targetRotation;
            }
        }

        private void FixedUpdate()
        {
            _targetPosition = SurgeMath.GetCameraMatrixPosition(_camera, -0.875f, -0.75f);
            transform.position = Vector3.Lerp(transform.position, _targetPosition, 
                Easings.Get(Easing.InCubic, _factor));
            
            Matrix4x4 viewMatrix = _camera.worldToCameraMatrix;
            Vector3 cameraForward = viewMatrix.inverse.MultiplyVector(_camera.transform.right);
            _targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
            
            if (shouldRotate)
            {
                Quaternion euler = Quaternion.AngleAxis(_factor * rotationDuration, Vector3.up);
                transform.localRotation *= euler * _initialRotation;
            }
            else
            {
                transform.localRotation = Quaternion.Lerp(_initialRotation, _targetRotation, 
                    Easings.Get(Easing.OutCubic, _factor));
            }

            transform.localScale = Vector3.Lerp(_initialScale, Vector3.one * 0.065f, _factor * 1.5f); // Need to multiply factor to fix scale
            
            _factor += Time.fixedDeltaTime / time;

            if (_factor >= 0.75f)
            {
                Destroy(gameObject);
            }
        }
    }
}
