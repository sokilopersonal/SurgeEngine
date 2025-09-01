using SurgeEngine._Source.Code.Gameplay.Effects;
using UnityEngine;
using Zenject;

namespace SurgeEngine._Source.Code.Infrastructure.DI
{
    public class ParticlesContainerInstaller : MonoInstaller
    {
        [SerializeField] private ParticlesContainer container;
        
        public override void InstallBindings()
        {
            Container.Bind<ParticlesContainer>().FromInstance(container).AsSingle().NonLazy();
        }
    }
}