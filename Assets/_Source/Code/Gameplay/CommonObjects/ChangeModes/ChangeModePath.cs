using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Gameplay.CommonObjects.ChangeModes
{
    public class ChangeModePath : ModeCollision
    {
        [SerializeField] private SplineContainer path;

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
    }
}