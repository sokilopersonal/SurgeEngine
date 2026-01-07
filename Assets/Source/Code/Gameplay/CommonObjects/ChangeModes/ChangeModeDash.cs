using SurgeEngine.Source.Code.Core.Character.System;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes
{
    public class ChangeModeDash : ChangeMode3D
    {
        protected override SplineTag SplineTagFilter => SplineTag.DashPath | SplineTag.Quickstep;
        
        protected override void SetMode(CharacterBase ctx)
        {
            ctx.Kinematics.SetDashPath(new ChangeMode3DData(new SplineData(container, ctx.transform.position), isChangeCamera, isLimitEdge, pathCorrectionForce));
        }

        protected override void RemoveMode(CharacterBase ctx)
        {
            ctx.Kinematics.SetDashPath(null);
        }
    }
}