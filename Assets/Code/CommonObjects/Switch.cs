using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorEffects;
using UnityEngine;
using DG.Tweening;

namespace SurgeEngine.Code.CommonObjects
{
    public class Switch : ContactBase
    {
        public Transform button;
        public SkinnedMeshRenderer meshRenderer;

        [HideInInspector] public Material active;
        [HideInInspector] public Material inactive;

        private bool toggled = false;
        private BoxCollider _collider;

        public override void Contact(Collider msg)
        {
            base.Contact(msg);

            Actor context = ActorContext.Context;

            toggled = !toggled;

            Material[] mats = meshRenderer.sharedMaterials;
            mats[2] = toggled ? active : inactive;
            meshRenderer.sharedMaterials = mats;

            button.DOKill(true);

            button.DOLocalMoveY(toggled ? -0.175f : 0f, 0.2f).SetEase(Ease.OutCirc);
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
