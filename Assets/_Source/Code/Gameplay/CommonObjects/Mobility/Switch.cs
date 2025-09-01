using DG.Tweening;
using FMODUnity;
using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;
using UnityEngine.Events;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility
{
    [SelectionBase]
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

        private bool _toggled = false;
        private bool _hasBeenToggled = false;
        private BoxCollider _collider;
        private Material _buttonMaterial;

        private void Start()
        {
            Material[] mats = meshRenderer.sharedMaterials;
            _buttonMaterial = new Material(mats[2]);
            mats[2] = _buttonMaterial;
            meshRenderer.sharedMaterials = mats;
        }

        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);

            if (toggleOnce && _hasBeenToggled)
                return;

            _hasBeenToggled = true;
            _toggled = !_toggled;

            float startEmissive = _toggled ? 1f : 0f;
            float endEmissive = _toggled ? 0f : 1f;
            
            float currentEmissive = startEmissive;

            _buttonMaterial.SetFloat("_EmissiveExposureWeight", startEmissive);
            
            DOTween.To(() => currentEmissive, x => currentEmissive = x, endEmissive, 0.25f).SetEase(Ease.OutQuart).OnUpdate(() =>
            {
                _buttonMaterial.SetFloat("_EmissiveExposureWeight", currentEmissive);
            });

            buttonTransform.DOKill(true);
            buttonTransform.DOLocalMoveY(_toggled ? -0.175f : -0.1f, 0.25f).SetEase(Ease.OutQuart);

            if (_toggled)
                onActivated.Invoke();
            else
                onDeactivated.Invoke();

            RuntimeManager.PlayOneShot(_toggled ? onReference : offReference, transform.position + Vector3.up);
        }

        private void OnDrawGizmosSelected()
        {
            if (_collider == null)
                _collider = GetComponent<BoxCollider>();

            Gizmos.color = new Color(0f, 1f, 1f, 0.1f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(_collider.center, _collider.size);
        }
    }
}
