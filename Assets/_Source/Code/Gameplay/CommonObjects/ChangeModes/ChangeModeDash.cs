using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Gameplay.CommonObjects.ChangeModes
{
    public class ChangeModeDash : ContactBase
    {
        [SerializeField] private SplineContainer path;
        
        private BoxCollider _boxCollider;

        public override void Contact(Collider msg, ActorBase context)
        {
            base.Contact(msg, context);
            
            ActorBase actor = ActorContext.Context;
            ActorKinematics kinematics = actor.Kinematics;
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