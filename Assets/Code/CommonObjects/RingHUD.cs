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
        
        private Vector3 _initialPosition;
        private Quaternion _initialRotation;
        private Vector3 _initialScale;
        
        public void Initialize(float time)
        {
            transform.localScale = Vector3.one * 1.3f;
            transform.parent = _camera.transform;
            
            _initialPosition = transform.position;
            _initialRotation = transform.rotation;
            _initialScale = transform.localScale;
            
            StartCoroutine(MoveToHUD(time));
        }

        private IEnumerator MoveToHUD(float time)
        {
            _factor = 0;
            
            while (_factor < 1f)
            {
                Move();

                _factor += Time.deltaTime / time;
                yield return null;
            }
            
            Destroy(gameObject);
        }

        private void Move()
        {
            Vector3 targetWorldPosition = SurgeMath.GetCameraMatrixPosition(_camera, 100, 100);
            transform.position = Vector3.Lerp(_initialPosition, targetWorldPosition, Easings.Get(Easing.OutCubic, _factor));
            transform.rotation = Quaternion.Slerp(_initialRotation, 
                Quaternion.LookRotation(transform.position - _camera.transform.position, _camera.transform.up) * Quaternion.Euler(0, 90, 0), 
                _factor);
            transform.localScale = Vector3.Lerp(_initialScale, Vector3.one * 0.13f, _factor);
        }
    }
}