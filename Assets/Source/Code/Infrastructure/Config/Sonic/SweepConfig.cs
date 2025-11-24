using System.Collections.Generic;
using UnityEngine;

namespace SurgeEngine.Source.Code.Infrastructure.Config.Sonic
{
    [CreateAssetMenu(fileName = "SweepKickConfig", menuName = "Surge Engine/Configs/Sonic/SweepKick", order = 0)]
    public class SweepConfig : ScriptableObject
    {
        public float deceleration = 6f;
        public List<string> eligibleAnimationStates = new List<string>() {
            "SitEnter",
            "SitExit",
            "Sliding",
            "SlideToSit",
            "CrawlEnter",
            "CrawlExit",
            "StompSquat"
        };
    }
}