using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes
{
    public class ChangeMode2D : ModeCollision
    {
        [SerializeField] private float pathEaseTime = 1f;
        [SerializeField] private DominantSpline dominantSpline = DominantSpline.Left;

        protected override SplineTag SplineTagFilter => SplineTag.SideView;

        protected override void SetMode(CharacterBase ctx)
        {
            ctx.Kinematics.Set2DPath(new ChangeMode2DData(new SplineData(container, ctx.transform.position, dominantSpline), ctx.transform.position, isChangeCamera, pathEaseTime));
        }

        protected override void RemoveMode(CharacterBase ctx)
        {
            ctx.Kinematics.Set2DPath(null);
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