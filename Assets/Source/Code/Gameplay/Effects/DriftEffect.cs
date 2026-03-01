using System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.Effects
{
    public class DriftEffect : Effect
    {
        public Rigidbody Rigidbody { private get; set; }

        private void Update()
        {
            if (particle.isPlaying)
            {
                // Do something... I don't know
            }
        }
    }
}