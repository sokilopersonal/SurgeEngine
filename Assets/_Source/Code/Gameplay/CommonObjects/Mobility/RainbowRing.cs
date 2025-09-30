using NaughtyAttributes;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine._Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility
{
    public class RainbowRing : DashRing, IPointMarkerLoader
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

        public void Load()
        {
            _triggered = false;
        }
    }
}