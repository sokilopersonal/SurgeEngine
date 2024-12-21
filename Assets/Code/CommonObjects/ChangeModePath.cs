using SurgeEngine.Code.ActorSystem;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.CommonObjects
{
    public class ChangeModePath : ContactBase
    {
        [SerializeField] private SplineContainer path;
        
        private BoxCollider _collider;

        public override void Contact(Collider msg)
        {
            base.Contact(msg);

            TogglePath();
        }
        
        private void TogglePath()
        {
            Actor context = ActorContext.Context;
            ActorKinematics kinematics = context.kinematics;
            if (!kinematics.IsPathValid())
            {
                kinematics.SetPath(path);
            }
            else
            {
                kinematics.SetPath(null);
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            
            if (_collider == null)
                _collider = GetComponent<BoxCollider>();
            
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = new Color(0f, 0.35f, 1f, 0.1f);
            Gizmos.DrawCube(_collider.center, _collider.size);
        }
    }
}