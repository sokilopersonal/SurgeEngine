using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.ChangeModes
{
    public class ChangeMode2D : ModeCollision
    {
        [SerializeField] private float pathEaseTime = 1f;
        [SerializeField] private DominantSpline dominantSpline = DominantSpline.Left;

        protected override void SetMode(CharacterBase ctx)
        {
            var kinematics = ctx.Kinematics;
            kinematics.Set2DPath(kinematics.Path2D == null
                ? new ChangeMode2DData(new SplineData(path, ctx.transform.position, dominantSpline), ctx.transform.position, isChangeCamera, pathEaseTime)
                : null);
        }
    }

    public class ChangeMode2DData : ChangeModeData
    {
        public Vector3 StartPosition { get; set; }
        public float PathEaseTime { get; private set; }
        public float CurrentEaseTime { get; set; }

        public ChangeMode2DData(SplineData spline, Vector3 startPos, bool isCameraChange, float pathEaseTime) : base(spline, isCameraChange)
        {
            StartPosition = startPos;
            PathEaseTime = pathEaseTime;
        }
    }

    public enum DominantSpline
    {
        Left,
        Right
    }
}