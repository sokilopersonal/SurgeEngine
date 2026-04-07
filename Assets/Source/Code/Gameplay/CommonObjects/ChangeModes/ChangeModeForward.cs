using SurgeEngine.Source.Code.Core.Character.System;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes
{
    public class ChangeModeForward : ChangeMode3D
    {
        protected override void SetMode(CharacterBase ctx)
        {
            ctx.Kinematics.Mode.SetForwardMode(new ChangeMode3DData(isChangeCamera, isLimitEdge, pathCorrectionForce));
        }

        protected override void RemoveMode(CharacterBase ctx)
        {
            ctx.Kinematics.Mode.SetForwardMode(null);
        }
    }
}