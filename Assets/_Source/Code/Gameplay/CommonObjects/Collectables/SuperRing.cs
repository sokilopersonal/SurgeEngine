using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Collectables
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