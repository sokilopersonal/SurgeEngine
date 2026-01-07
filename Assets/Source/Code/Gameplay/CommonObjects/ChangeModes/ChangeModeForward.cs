using SurgeEngine.Source.Code.Core.Character.System;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes
{
    public class ChangeModeForward : ChangeMode3D
    {
        protected override SplineTag SplineTagFilter => SplineTag.SideView | SplineTag.Quickstep;

        protected override void SetMode(CharacterBase ctx)
        {
            ctx.Kinematics.SetForwardPath(new ChangeMode3DData(new SplineData(container, ctx.transform.position), isChangeCamera, isLimitEdge, pathCorrectionForce));
        }

        protected override void RemoveMode(CharacterBase ctx)
        {
            ctx.Kinematics.SetForwardPath(null);
        }
    }
}