using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.Misc
{
    public class MouthDubleSwitcher : MonoBehaviour
    {
        [SerializeField] Transform mouthReference;

        private Transform _cameraTransform;
        private Actor _actor;

        private void Start()
        {
            _actor = ActorContext.Context;
            _cameraTransform = _actor.camera.GetCamera().transform;
        }

        private void LateUpdate()
        {
            float dot = Vector3.Dot(-_cameraTransform.forward, _actor.transform.right);

            if (dot < -0.1f)
                mouthReference.localScale = new Vector3(-1, 1, 1);
            else if (dot > 0.1f)
                mouthReference.localScale = new Vector3(1, 1, 1);
        }
    }
}