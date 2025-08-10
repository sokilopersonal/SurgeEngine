using NaughtyAttributes;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
{
    public class RainbowRing : DashRing
    {
        [SerializeField, Space(10), InfoBox("If true then score will be added on every contact, otherwise only once")] 
        private bool allowDoubleScore;

        private bool _triggered;

        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);

            int score = 1000;
            if (!allowDoubleScore)
            {
                if (!_triggered)
                {
                    Utility.AddScore(score);
                    _triggered = true;
                }
            }
            else
                Utility.AddScore(score);
        }
    }
}