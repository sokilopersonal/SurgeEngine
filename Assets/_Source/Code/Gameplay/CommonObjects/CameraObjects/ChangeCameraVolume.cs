using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.CameraObjects
{
    public class ChangeCameraVolume : ContactBase
    {
        [SerializeField] private ObjCameraBase target;
        
        private BoxCollider _boxCollider;

        protected override void Awake()
        {
            _boxCollider = GetComponent<BoxCollider>();
            
            if (target == null) // If we don't have a target, search it in the children
            {
                target = GetComponentInChildren<ObjCameraBase>();
            }
        }

        public override void Contact(Collider msg, ActorBase context)
        {
            base.Contact(msg, context);
            
            target.SetPan();
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (other.transform.TryGetComponent(out ActorBase actor))
            {
                target.RemovePan();
            }
        }

        public void ResetPan()
        {
            target.RemovePan();
        }

        public void DisablePan()
        {
            _boxCollider.enabled = false;
            target.RemovePan();
        }

        protected override void OnDrawGizmos()
        {
            if (_boxCollider == null)
                _boxCollider = GetComponent<BoxCollider>();
            
            Gizmos.color = new Color(0.15f, 1f, 0f, 0.1f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(_boxCollider.center, _boxCollider.size);
        }
    }
}