using SurgeEngine.Code.ActorHUD;
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

        public void Initialize(float time)
        {
            this.time = time;
            transform.SetParent(_camera.transform);
            
            _initialPosition = transform.localPosition;
            _initialRotation = transform.localRotation;
            _initialScale = transform.localScale;
        }

        private void Update()
        {
            Vector3 rect = ActorStageHUD.Context.ringCounter.rectTransform.position;
            rect.z = 0.1f;
            Vector3 point = _camera.ScreenToWorldPoint(rect);
            point = _camera.transform.InverseTransformPoint(point);
            _targetPosition = point;

            float easedFactor = Easings.Get(Easing.OutCubic, _factor);
            transform.localPosition = Vector3.Lerp(_initialPosition, _targetPosition, easedFactor);

            Vector3 targetScale = Vector3.one * (0.02f * _camera.fieldOfView) / 60f;
            transform.localScale = Vector3.Lerp(_initialScale, targetScale, easedFactor);

            Matrix4x4 viewMatrix = _camera.worldToCameraMatrix;
            Vector3 cameraForward = viewMatrix.inverse.MultiplyVector(_camera.transform.right);
            _targetRotation = Quaternion.LookRotation(cameraForward, Vector3.up);
            
            transform.localRotation = Quaternion.Lerp(_initialRotation, _targetRotation, Easings.Get(Easing.OutCubic, _factor));
            
            _factor += Time.deltaTime / time;
            
            if (_factor >= 1f)
            {
                var context = ActorStageHUD.Context;
                context.ringCounterAnimator.Play("RingBump", 0);
                context.ringBumpEffect.Play("RingBump", 0);
                
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            
        }
    }
}