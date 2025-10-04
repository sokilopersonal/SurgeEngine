using SurgeEngine._Source.Code.Core.Character.System;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.ChangeModes
{
    public class ChangeModeDash : ChangeMode3D
    {
        protected override void SetMode(CharacterBase ctx)
        {
            var kinematics = ctx.Kinematics;
            kinematics.SetDashPath(kinematics.PathDash == null
                ? new ChangeMode3DData(new SplineData(path, ctx.transform.position), isChangeCamera, isLimitEdge,
                    pathCorrectionForce)
                : null);
        }
    }
}