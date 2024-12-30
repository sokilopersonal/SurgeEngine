using FMODUnity;
using UnityEngine;
using UnityEngine.Events;

namespace SurgeEngine.Code.CommonObjects
{
    public class Switch : ContactBase
    {
        [Header("Main")]
        [Space(10)]
        public bool toggleOnce = true;
        public SkinnedMeshRenderer meshRenderer;
        [Space(5)]
        public Material active;
        public Material inactive;

        [Header("Switch Events")]
        [Space(10)]
        public UnityEvent onActivated;
        public UnityEvent onDeactivated;

        [Header("Sounds")]
        [Space(10)]
        public EventReference onReference;
        public EventReference offReference;

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

            RuntimeManager.PlayOneShot(toggled ? onReference : offReference, transform.position + Vector3.up);
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
