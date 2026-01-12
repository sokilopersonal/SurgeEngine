
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    public class RainbowRing : DashRing, IPointMarkerLoader
    {
        [SerializeField, Space(10)] private bool onlyTriggerOnce;

        private bool _triggered;

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);

            int score = 1000;
            if (!onlyTriggerOnce)
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