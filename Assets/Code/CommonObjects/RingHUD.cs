using System.Collections;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class RingHUD : MonoBehaviour
    {
        private float _factor;

        private Camera _camera => ActorContext.Context.camera.GetCamera();
        
        private Vector3 _targetPosition;
        private Vector3 _initialScale;
        
        private Quaternion _initialRotation;
        private Quaternion _targetRotation;

        public void Initialize(float time)
        {
            transform.localScale = Vector3.one * 1.2f;
            transform.parent = _camera.transform;
            
            _initialScale = transform.localScale;
            _initialRotation = transform.rotation;

            StartCoroutine(MoveToHUD(time));
        }

        private IEnumerator MoveToHUD(float time)
        {
            _factor = 0;
            
            while (_factor < 0.75f) // For some reason it doesn't work with 1
            {
                Move();
                _factor += Time.deltaTime / time;
                yield return null;
            }
            
            Destroy(gameObject);
        }

        private void Move()
        {
            _targetPosition = SurgeMath.GetCameraMatrixPosition(_camera, 100, 100);
            transform.position = Vector3.Lerp(transform.position, _targetPosition, 
                Easings.Get(Easing.InCubic, _factor));
            
            Matrix4x4 viewMatrix = _camera.worldToCameraMatrix;
            Vector3 cameraForward = viewMatrix.inverse.MultiplyVector(_camera.transform.right);
            _targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
            transform.localRotation = Quaternion.Lerp(_initialRotation, _targetRotation, 
                Easings.Get(Easing.OutCubic, _factor));
            
            transform.localScale = Vector3.Lerp(_initialScale, Vector3.one * 0.065f, _factor * 1.5f); // Need to multiply factor to fix scale
        }
    }
}
