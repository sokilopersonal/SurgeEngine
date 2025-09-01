using System.Collections.Generic;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.Effects
{
    public class MultipleEffect : Effect
    {
        [SerializeField] private List<ParticleSystem> particles = new List<ParticleSystem>();
        public override void Toggle(bool value)
        {
            foreach (ParticleSystem particle in particles)
            {
                if (value)
                    particle.Play(true);
                else
                    particle.Stop(true);
            }
        }
    }
}