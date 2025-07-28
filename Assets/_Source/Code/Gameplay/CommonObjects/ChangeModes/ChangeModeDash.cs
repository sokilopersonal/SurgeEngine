using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Gameplay.CommonObjects.ChangeModes
{
    public class ChangeModeDash : ModeCollision
    {
        [SerializeField] private SplineContainer path;
        
        public override void Contact(Collider msg, ActorBase context)
        {
            base.Contact(msg, context);

            if (!CheckFacing(context.transform.forward))
                return;
            
            ActorKinematics kinematics = context.Kinematics;
            if (!kinematics.IsPathValid())
            {
                kinematics.SetPath(path, KinematicsMode.Dash);
            }
            else
            {
                kinematics.SetPath(null);
            }
        }
    }
}