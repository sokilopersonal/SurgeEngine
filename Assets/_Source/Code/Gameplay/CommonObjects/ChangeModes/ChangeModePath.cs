using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Gameplay.CommonObjects.ChangeModes
{
    public class ChangeModePath : ContactBase
    {
        [SerializeField] private SplineContainer path;
        
        private BoxCollider _collider;

        public override void Contact(Collider msg, ActorBase context)
        {
            base.Contact(msg, context);

            TogglePath();
        }
        
        private void TogglePath()
        {
            ActorBase context = ActorContext.Context;
            ActorKinematics kinematics = context.kinematics;
            if (!kinematics.IsPathValid())
            {
                kinematics.SetPath(path, KinematicsMode.Forward);
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