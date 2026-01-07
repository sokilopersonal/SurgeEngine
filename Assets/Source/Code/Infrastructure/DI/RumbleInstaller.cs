using SurgeEngine.Source.Code.Gameplay.Inputs;
using Zenject;

namespace SurgeEngine.Source.Code.Infrastructure.DI
{
    public class RumbleInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<Rumble>().FromNew().AsSingle().NonLazy();
        }
    }
}