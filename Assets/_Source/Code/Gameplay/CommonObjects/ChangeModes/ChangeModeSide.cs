using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Gameplay.CommonObjects.ChangeModes
{
    public class ChangeModeSide : ModeCollision
    {
        [SerializeField] private SplineContainer path;
        
        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);

            if (!CheckFacing(context.transform.forward))
                return;
            
            CharacterKinematics kinematics = context.Kinematics;
            if (!kinematics.IsPathValid())
            {
                kinematics.SetPath(path, KinematicsMode.Side);
            }
            else
            {
                kinematics.SetPath(null);
            }
        }
    }
}