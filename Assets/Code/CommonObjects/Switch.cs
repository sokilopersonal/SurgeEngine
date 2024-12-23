using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorEffects;
using UnityEngine;
using UnityEngine.Events;

namespace SurgeEngine.Code.CommonObjects
{
    public class Switch : ContactBase
    {
        public SkinnedMeshRenderer meshRenderer;
        public bool toggleOnce = true;
        public UnityEvent onActivated;
        public UnityEvent onDeactivated;
        [HideInInspector] public Material active;
        [HideInInspector] public Material inactive;

        private bool toggled = false;
        private bool hasBeenToggled = false;
        private BoxCollider _collider;

        public override void Contact(Collider msg)
        {
            base.Contact(msg);

            if (toggleOnce && hasBeenToggled)
                return;

            hasBeenToggled = true;
            toggled = !toggled;

            if (toggled)
                onActivated.Invoke();
            else
                onDeactivated.Invoke();

            Material[] mats = meshRenderer.sharedMaterials;
            mats[2] = toggled ? active : inactive;
            meshRenderer.sharedMaterials = mats;
        }

        protected override void OnDrawGizmos()
        {
            if (_collider == null)
                _collider = GetComponent<BoxCollider>();

            Gizmos.color = new Color(0f, 1f, 1f, 0.1f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(_collider.center, _collider.size);
        }
    }
}
