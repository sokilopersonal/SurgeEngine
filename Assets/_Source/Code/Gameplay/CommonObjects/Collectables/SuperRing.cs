using SurgeEngine._Source.Code.Core.Character.System;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Collectables
{
    public class SuperRing : Ring
    {
        protected override void Collect(int count)
        {
            base.Collect(10);
        }

        public override void StartMagnet(CharacterBase character) { }
        public override bool IsSuperRing() => true;
    }
}