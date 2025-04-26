using UnityEngine;

namespace SurgeEngine.Code.Effects
{
    public class Step : MonoBehaviour
    {
        [SerializeField] private ParticleSystem smokeGrass;
        [SerializeField] private ParticleSystem smokeWater;
        [SerializeField] private ParticleSystem smokeIron;
        [SerializeField] private ParticleSystem smokeWood;
        
        public ParticleSystem SmokeGrass => smokeGrass;
        public ParticleSystem SmokeWater => smokeWater;
        public ParticleSystem SmokeIron => smokeIron;
        public ParticleSystem SmokeWood => smokeWood;
    }
}