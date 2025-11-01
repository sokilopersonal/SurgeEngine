using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Source.Code.Gameplay.Effects
{
    public class ParticlesContainer : MonoBehaviour
    {
        private static ParticlesContainer _instance;
        
        [SerializeField] private List<ParticleEffect> particles = new List<ParticleEffect>();

        [Inject] private void Init(ParticlesContainer instance) => _instance = instance;
        
        public static void Spawn(string name, Vector3 position, float destroyTime)
        {
            var particle = GetParticle(name);
            var instance = Instantiate(particle.Particle);
            instance.transform.position = position;
            particle.Particle.Play(true);
            
            Destroy(instance.gameObject, destroyTime);
        }

        public static void Spawn(string name, Vector3 position, Quaternion rotation, float destroyTime)
        {
            var particle = GetParticle(name);
            var instance = Instantiate(particle.Particle, position, rotation);
            particle.Particle.Play(true);
            
            Destroy(instance.gameObject, destroyTime);
        }

        public static void Spawn(string name, Vector3 position)
        {
            var particle = GetParticle(name);
            var instance = Instantiate(particle.Particle);
            instance.transform.position = position;
            particle.Particle.Play(true);
            
            Destroy(instance.gameObject, particle.Particle.main.duration);
        }

        public static void Spawn(string name, Vector3 position, Quaternion rotation)
        {
            var particle = GetParticle(name);
            var instance = Instantiate(particle.Particle, position, rotation);
            particle.Particle.Play(true);
            
            Destroy(instance.gameObject, particle.Particle.main.duration);
        }
        
        private static ParticleEffect GetParticle(string name) => _instance.particles.Find(particle => particle.Name == name);
    }

    [Serializable]
    public class ParticleEffect
    {
        [SerializeField] private string name;
        [SerializeField] private ParticleSystem particle;
        
        public string Name => name;
        public ParticleSystem Particle => particle;
    }
}