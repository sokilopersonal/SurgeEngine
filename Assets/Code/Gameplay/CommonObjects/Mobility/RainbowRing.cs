using NaughtyAttributes;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
{
    public class RainbowRing : DashRing
    {
        [SerializeField, Space(10), InfoBox("If true then score will be added on every contact, otherwise only once")] 
        private bool allowDoubleScore;

        private bool _triggered;

        public override void Contact(Collider msg)
        {
            base.Contact(msg);

            int score = 1000;
            if (!allowDoubleScore)
            {
                if (!_triggered)
                {
                    Common.AddScore(score);
                    _triggered = true;
                }
            }
            else
                Common.AddScore(score);
        }
    }
}