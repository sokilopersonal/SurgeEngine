using SurgeEngine.Code.Core.Actor.HUD;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.Gameplay.CommonObjects.HUD
{
    public class RingHUD : MonoBehaviour
    {
        [SerializeField] private AnimationCurve easingCurve;
        
        private float _factor;

        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private Vector3 _startScale;

        private float _distance;

        [Inject] private CharacterBase _character;
        private Camera _camera => _character.Camera.GetCamera();
        private CharacterStageHUD _characterStageHUD;

        public void Initialize(CharacterStageHUD hud)
        {
            _characterStageHUD = hud;
            
            transform.SetParent(_camera.transform, true);
            _startPosition = transform.localPosition;
            _startRotation = transform.localRotation;
            _startScale = transform.localScale;

            var context = CharacterContext.Context;
            float t = context.Kinematics.Speed / context.Config.topSpeed;
            _startScale *= Mathf.Lerp(1f, Random.Range(1.2f, 1.6f), t);
            _startPosition += Vector3.up * (Random.Range(-0.175f, 0.175f) * t);
            _startPosition += Vector3.right * (Random.Range(-0.175f, 0.175f) * t);
            _distance = Vector3.Distance(_camera.transform.TransformPoint(_startPosition), _camera.transform.position) / 2;
            
            const float distanceThreshold = 6f;
            if (_distance <= distanceThreshold)
            {
                _startPosition += Vector3.forward * (Random.Range(0.3f, 0.4f) * t);
            }
            else
            {
                _startPosition += Vector3.back * (_distance * (Random.Range(0.3f, 0.6f) * t));
            }
        }

        private void FixedUpdate()
        {
            Align();
            
            _factor += Time.deltaTime / 0.45f;
            if (_factor >= 1f)
            {
                _characterStageHUD.RingCounterAnimator.Play("RingBump", 0);
                _characterStageHUD.RingBumpAnimator.Play("RingBump", 0);
                
                Destroy(gameObject);
            }
        }
        
        private void Align()
        {
            Vector3 worldPos =
                MatrixHelper.GetMatrixRectTransformPosition(_characterStageHUD.RingCounterRect.rectTransform, _camera,
                    _distance);

            float easedFactor = easingCurve.Evaluate(_factor);
            var context = _character;
            float speedT = context.Kinematics.Speed / context.Config.topSpeed;
            
            Vector3 targetLocalPos = transform.parent.InverseTransformPoint(worldPos);
            transform.localPosition = Vector3.Lerp(_startPosition, targetLocalPos, easedFactor);
            
            Vector3 cameraForward = _camera.cameraToWorldMatrix.inverse.MultiplyVector(_camera.transform.right);
            transform.localRotation = Quaternion.Lerp(_startRotation, Quaternion.LookRotation(cameraForward, Vector3.up), Easings.Get(Easing.OutCubic, _factor *
                Mathf.Lerp(1f, Random.Range(1.25f, 1.5f), speedT)));

            const float maxScale = 0.003f;
            Vector3 targetScale = Vector3.one * (maxScale * _distance * _camera.fieldOfView);
            transform.localScale = Vector3.Lerp(_startScale, targetScale, easedFactor);
        }
    }
}