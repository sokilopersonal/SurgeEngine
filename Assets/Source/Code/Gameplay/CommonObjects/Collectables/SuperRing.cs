using SurgeEngine.Source.Code.Core.Character.System;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Collectables
{
    public class SuperRing : Ring
    {
        public override bool IsLightSpeedDashTarget => false;

        protected override void Collect(int count)
        {
            base.Collect(10);
        }

        public override void StartMagnet(CharacterBase character) { }
        public override bool IsSuperRing() => true;
    }
}