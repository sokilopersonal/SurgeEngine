using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Gameplay.CommonObjects.ChangeModes
{
    public class ChangeModePath : ModeCollision
    {
        [SerializeField] private SplineContainer path;
        
        private BoxCollider _collider;

        public override void Contact(Collider msg, ActorBase context)
        {
            base.Contact(msg, context);

            if (!CheckFacing(context.transform.forward))
                return;
            
            TogglePath(context);
        }
        
        private void TogglePath(ActorBase context)
        {
            ActorKinematics kinematics = context.Kinematics;
            if (!kinematics.IsPathValid())
            {
                kinematics.SetPath(path, KinematicsMode.Forward);
            }
            else
            {
                kinematics.SetPath(null);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (_collider == null)
                _collider = GetComponent<BoxCollider>();
            
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = new Color(0f, 0.35f, 1f, 0.3f);
            Gizmos.DrawCube(_collider.center, _collider.size);
        }
    }
}