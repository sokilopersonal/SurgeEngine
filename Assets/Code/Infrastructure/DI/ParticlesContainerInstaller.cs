using SurgeEngine.Code.Effects;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.DI
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