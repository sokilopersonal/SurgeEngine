using NaughtyAttributes;
using UnityEngine;

namespace SurgeEngine.Code.Config.SonicSpecific
{
    [CreateAssetMenu(fileName = "BoostConfig", menuName = "SurgeEngine/Configs/Sonic/Boost", order = 0)]
    public class BoostConfig : ScriptableObject
    {
        public float startSpeed = 45f;
        public float airBoostSpeed = 45f;
        
        public float startDrain = 5f;
        public float energyDrain = 5.25f;
        public float turnSpeedMultiplier = 0.8f;
        public float maxSpeedMultiplier = 1.2f;
        public float magnetRadius = 2.75f;
        public float acceleration = 45f;

        [Foldout("Energy Additions")] public float ringEnergyAddition = 1.5f;
        [Foldout("Energy Additions")] public float driftEnergyAddition = 6.75f;
    }
}