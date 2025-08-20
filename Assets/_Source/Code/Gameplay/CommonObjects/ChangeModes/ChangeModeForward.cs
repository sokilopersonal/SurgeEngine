using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Gameplay.CommonObjects.ChangeModes
{
    public class ChangeModeForward : ChangeMode3D
    {
        [SerializeField] private SplineContainer path;
        
        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);

            if (!CheckFacing(context.transform.forward))
                return;
            
            var kinematics = context.Kinematics;
            kinematics.SetForwardPath(kinematics.PathForward == null
                ? new ChangeMode3DData(new SplineData(path, context.transform.position), isChangeCamera, isLimitEdge,
                    pathCorrectionForce)
                : null);
        }
    }
}