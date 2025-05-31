using NaughtyAttributes;
using UnityEngine;

namespace SurgeEngine.Code.Infrastructure.Config.SonicSpecific
{
    [CreateAssetMenu(fileName = "BoostConfig", menuName = "SurgeEngine/Configs/Sonic/Boost", order = 0)]
    public class BoostConfig : ScriptableObject
    {
        [SerializeField] private float startSpeed = 45f;
        [SerializeField] private float airBoostSpeed = 45f;
        [SerializeField] private float startDrain = 5f;
        [SerializeField] private float energyDrain = 5.25f;
        [SerializeField] private float turnSpeedMultiplier = 0.8f;
        [SerializeField] private float maxSpeedMultiplier = 1.2f;
        [SerializeField] private float magnetRadius = 2.75f;
        [SerializeField] private float acceleration = 45f;
        [SerializeField, Range(25, 100)] private float boostCapacity = 100;
        [SerializeField] private float ringEnergyAddition = 1.5f;
        [SerializeField] private float driftEnergyAddition = 6.75f;
        [SerializeField] private float inAirTime = 0.3f;

        public float StartSpeed => startSpeed;
        public float AirBoostSpeed => airBoostSpeed;
        public float StartDrain => startDrain;
        public float EnergyDrain => energyDrain;
        public float TurnSpeedMultiplier => turnSpeedMultiplier;
        public float MaxSpeedMultiplier => maxSpeedMultiplier;
        public float MagnetRadius => magnetRadius;
        public float Acceleration => acceleration;
        public float BoostCapacity => boostCapacity;
        public float RingEnergyAddition => ringEnergyAddition;
        public float DriftEnergyAddition => driftEnergyAddition;
        public float InAirTime => inAirTime;
    }
}