using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes
{
    public class ChangeMode2D : ModeCollision
    {
        [SerializeField] private float pathEaseTime = 1f;
        [SerializeField] private DominantSide dominantSide = DominantSide.Left;

        protected override SplineTag SplineTagFilter => SplineTag.SideView;

        protected override void SetMode(CharacterBase ctx)
        {
            ctx.Kinematics.Mode.Set2DPath(new ChangeMode2DData(new SplineData(Container, ctx.transform.position, dominantSide), ctx.transform.position, isChangeCamera, pathEaseTime));
        }

        protected override void RemoveMode(CharacterBase ctx)
        {
            ctx.Kinematics.Mode.Set2DPath(null);
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

    public enum DominantSide
    {
        Left,
        Right
    }
}