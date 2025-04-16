using SurgeEngine.Code.Actor.HUD;
using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects.HUD
{
    public class RingHUD : MonoBehaviour
    {
        private float _factor;

        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private Vector3 _startScale;

        private float _distance;
        
        private Camera _camera => ActorContext.Context.camera.GetCamera();

        public void Initialize()
        {
            transform.SetParent(_camera.transform, true);
            _startPosition = transform.localPosition;
            _startRotation = transform.localRotation;
            _startScale = transform.localScale;

            var context = ActorContext.Context;
            float t = context.kinematics.Speed / context.config.topSpeed;
            _startScale *= Mathf.LerpUnclamped(1f, Random.Range(1.2f, 1.4f), t);
            _startPosition += Vector3.up * (Random.Range(-0.12f, 0.12f) * t);
            _startPosition += Vector3.right * (Random.Range(-0.12f, 0.12f) * t);
            _distance = Vector3.Distance(_camera.transform.TransformPoint(_startPosition), _camera.transform.position) / 2;
            _startPosition += Vector3.back * (_distance * (Random.Range(0.1f, 0.2f) * t));
        }

        private void Update()
        {
            _factor += Time.deltaTime / 0.38f;
            
            Align();
            
            if (_factor >= 1f)
            {
                ActorStageHUD context = ActorHUDContext.Context;
                context.ringCounterAnimator.Play("RingBump", 0);
                context.ringBumpEffect.Play("RingBump", 0);
                
                Destroy(gameObject);
            }
        }
        
        private void Align()
        {
            var rectTransform = ActorHUDContext.Context.ringCounter.rectTransform;

            Vector3 rect = rectTransform.position;
            Vector3 screenPos = rect;
            screenPos.z = _distance;

            Camera cam = _camera;
            float ndcX = screenPos.x / cam.pixelWidth * 2 - 1;
            float ndcY = screenPos.y / cam.pixelHeight * 2 - 1;

            Vector4 ndcNear = new Vector4(ndcX, ndcY, -1f, 1f);
            Matrix4x4 invProjectionMatrix = cam.projectionMatrix.inverse;
            Matrix4x4 cameraToWorldMatrix = cam.cameraToWorldMatrix;

            Vector4 worldNearH = cameraToWorldMatrix * invProjectionMatrix * ndcNear;
            Vector3 worldNear = worldNearH / worldNearH.w;

            Vector3 dir = (worldNear - cam.transform.position).normalized;
            Vector3 worldPos = cam.transform.position + dir * _distance;

            float easedFactor = Easings.Get(Easing.InCubic, Mathf.Clamp01(_factor));
            var context = ActorContext.Context;
            float speedT = context.kinematics.Speed / context.config.topSpeed;
            
            Vector3 targetLocalPos = transform.parent.InverseTransformPoint(worldPos);
            transform.localPosition = Vector3.Lerp(_startPosition, targetLocalPos, easedFactor);
            
            Vector3 cameraForward = cameraToWorldMatrix.inverse.MultiplyVector(_camera.transform.right);
            transform.localRotation = Quaternion.Lerp(_startRotation, Quaternion.LookRotation(cameraForward, Vector3.up), Easings.Get(Easing.OutCubic, _factor *
                Mathf.Lerp(1f, Random.Range(1.25f, 1.5f), speedT)));

            const float maxScale = 0.003f;
            Vector3 targetScale = Vector3.one * (maxScale * _distance * _camera.fieldOfView);
            transform.localScale = Vector3.Lerp(_startScale, targetScale, easedFactor);
        }
    }
}