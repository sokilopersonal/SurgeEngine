using System.Collections.Generic;
using UnityEngine;

namespace SurgeEngine.Code.Config.SonicSpecific
{
    [CreateAssetMenu(fileName = "SweepKickConfig", menuName = "SurgeEngine/Configs/Sonic/SweepKick", order = 0)]
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