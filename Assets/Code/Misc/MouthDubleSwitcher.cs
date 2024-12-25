using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.Misc
{
    public class MouthDubleSwitcher : MonoBehaviour
    {
        [SerializeField] private Animator animator;

        private Transform _cameraTransform;
        private Actor _actor;

        private void Update()
        {
            _actor = ActorContext.Context;
            _cameraTransform = _actor.camera.GetCamera().transform;
            
            float dot = Vector3.Dot(-_cameraTransform.forward, _actor.transform.right);
            Debug.Log(dot);

            if (dot > 0)
            {
                animator.SetLayerWeight(1, 1);
            }
            else if (dot < 0)
            {
                animator.SetLayerWeight(1, 0);
            }
        }
    }
}