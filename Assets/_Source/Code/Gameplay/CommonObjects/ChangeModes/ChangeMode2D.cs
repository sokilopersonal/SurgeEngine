using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Gameplay.CommonObjects.ChangeModes
{
    public class ChangeMode2D : ModeCollision
    {
        [SerializeField] private SplineContainer path;
        [SerializeField] private float pathEaseTime = 1f;

        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);

            if (!CheckFacing(context.transform.forward))
                return;
            
            var kinematics = context.Kinematics;
            kinematics.Set2DPath(kinematics.Path2D == null
                ? new ChangeMode2DData(new SplineData(path, context.transform.position), isChangeCamera, pathEaseTime)
                : null);
        }
    }

    public class ChangeMode2DData : ChangeModeData
    {
        public float PathEaseTime { get; private set; }
        public float CurrentEaseTime { get; set; }

        public ChangeMode2DData(SplineData spline, bool isCameraChange, float pathEaseTime) : base(spline, isCameraChange)
        {
            PathEaseTime = pathEaseTime;
        }
    }
}