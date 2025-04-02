using SurgeEngine.Code.Actor.System;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.CommonObjects
{
    public class ChangeModeDash : ContactBase
    {
        [SerializeField] private SplineContainer path;
        
        private BoxCollider _boxCollider;

        public override void Contact(Collider msg)
        {
            base.Contact(msg);
            
            ActorBase actor = ActorContext.Context;
            ActorKinematics kinematics = actor.kinematics;
            if (!kinematics.IsPathValid())
            {
                kinematics.SetPath(path, KinematicsMode.Dash);
            }
            else
            {
                kinematics.SetPath(null);
            }
        }

        protected override void OnDrawGizmos()
        {
            if (_boxCollider == null)
                _boxCollider = GetComponent<BoxCollider>();
            
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.1f);
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawCube(_boxCollider.center, _boxCollider.size);
        }
    }
}