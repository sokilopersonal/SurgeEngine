using System.Collections.Generic;
using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Infrastructure.Config.Sonic
{
    [CreateAssetMenu(fileName = "SweepKickConfig", menuName = "Surge Engine/Configs/Sonic/SweepKick", order = 0)]
    public class SweepConfig : ScriptableObject
    {
        [SerializeField] private ButtonType button = ButtonType.B;
        [SerializeField] private float deceleration = 6f;
        [SerializeField] private List<string> eligibleAnimationStates = new List<string>() {
            "SitEnter",
            "SitExit",
            "Sliding",
            "SlideToSit",
            "CrawlEnter",
            "CrawlExit",
            "StompSquat"
        };
        
        public ButtonType Button => button;
        public float Deceleration => deceleration;
        public List<string> EligibleAnimationStates => eligibleAnimationStates;
    }
}