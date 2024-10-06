using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class ChangeCameraVolume : ContactBase
    {
        [SerializeField] private ObjCameraBase target;
        
        private BoxCollider collider;

        private void Awake()
        {
            if (target == null) // If we don't have a target, search it in the children
            {
                target = GetComponentInChildren<ObjCameraBase>();
            }
        }

        public override void OnTriggerContact(Collider msg)
        {
            base.OnTriggerContact(msg);
            
            target.SetPan();
        }
        
        private void OnTriggerExit(Collider other)
        {
            if (ActorContext.Context.gameObject == other.transform.parent.gameObject)
            {
                target.RemovePan();
            }
        }

        private void OnDrawGizmos()
        {
            if (collider == null)
                collider = GetComponent<BoxCollider>();
            
            Gizmos.color = new Color(0.15f, 1f, 0f, 0.135f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(collider.center, collider.size);
        }
    }
}