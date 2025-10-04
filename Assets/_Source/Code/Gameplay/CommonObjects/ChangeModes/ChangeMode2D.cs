using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.ChangeModes
{
    public class ChangeMode2D : ModeCollision
    {
        [SerializeField] private float pathEaseTime = 1f;

        protected override void SetMode(CharacterBase ctx)
        {
            var kinematics = ctx.Kinematics;
            kinematics.Set2DPath(kinematics.Path2D == null
                ? new ChangeMode2DData(new SplineData(path, ctx.transform.position), isChangeCamera, pathEaseTime)
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