using SurgeEngine.Source.Code.Core.Character.System;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes
{
    public class ChangeModeDash : ChangeMode3D
    {
        protected override void SetMode(CharacterBase ctx)
        {
            ctx.Kinematics.Mode.SetDashPath(new ChangeMode3DData(isChangeCamera, isLimitEdge, pathCorrectionForce));
        }

        protected override void RemoveMode(CharacterBase ctx)
        {
            ctx.Kinematics.Mode.SetDashPath(null);
        }
    }
}