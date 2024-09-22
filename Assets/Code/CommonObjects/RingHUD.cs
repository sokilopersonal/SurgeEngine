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

        private Quaternion _initialRotation;
        private Quaternion _targetRotation;

        public void Initialize(float time)
        {
            transform.localScale = Vector3.one * 1.25f;
            
            _initialRotation = transform.rotation;

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
            Matrix4x4 viewMatrix = _camera.worldToCameraMatrix;
            Vector3 cameraForward = viewMatrix.inverse.MultiplyVector(Vector3.right);
            _targetRotation = Quaternion.LookRotation(cameraForward, _camera.transform.up);
            transform.rotation = Quaternion.Lerp(_initialRotation, _targetRotation, 
                Easings.Get(Easing.OutCubic, _factor));
        }
    }
}
