using SurgeEngine.Code.Actor.System;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.CommonObjects
{
    public class ChangeModeSide : ContactBase
    {
        [SerializeField] private SplineContainer path;
        
        private BoxCollider _collider;
        
        public override void Contact(Collider msg)
        {
            base.Contact(msg);
            
            ActorBase context = ActorContext.Context;
            ActorKinematics kinematics = context.kinematics;
            if (!kinematics.IsPathValid())
            {
                kinematics.SetPath(path, KinematicsMode.Side);
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
            Gizmos.color = new Color(0.61f, 1f, 0.18f, 0.1f);
            Gizmos.DrawCube(_collider.center, _collider.size);
        }
    }
}