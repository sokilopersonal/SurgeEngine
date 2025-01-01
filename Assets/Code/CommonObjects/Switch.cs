using FMODUnity;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

namespace SurgeEngine.Code.CommonObjects
{
    public class Switch : ContactBase
    {
        [Header("Main")]
        [Space(10)]
        public bool toggleOnce = true;
        public Transform buttonTransform;
        public SkinnedMeshRenderer meshRenderer;

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
        private Material buttonMaterial;

        private void Start()
        {
            Material[] mats = meshRenderer.sharedMaterials;
            buttonMaterial = new Material(mats[2]);
            mats[2] = buttonMaterial;
            meshRenderer.sharedMaterials = mats;
        }

        public override void Contact(Collider msg)
        {
            base.Contact(msg);

            if (toggleOnce && hasBeenToggled)
                return;

            hasBeenToggled = true;
            toggled = !toggled;

            float startEmissive = toggled ? 1f : 0f;
            float endEmissive = toggled ? 0f : 1f;
            
            float currentEmissive = startEmissive;

            buttonMaterial.SetFloat("_EmissiveExposureWeight", startEmissive);
            
            DOTween.To(() => currentEmissive, x => currentEmissive = x, endEmissive, 0.25f).SetEase(Ease.OutQuart).OnUpdate(() =>
            {
                buttonMaterial.SetFloat("_EmissiveExposureWeight", currentEmissive);
            });

            buttonTransform.DOKill(true);
            buttonTransform.DOLocalMoveY(toggled ? -0.175f : -0.1f, 0.25f).SetEase(Ease.OutQuart);

            if (toggled)
                onActivated.Invoke();
            else
                onDeactivated.Invoke();

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
