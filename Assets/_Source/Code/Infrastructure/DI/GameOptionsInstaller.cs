using SurgeEngine._Source.Code.Infrastructure.Tools.Managers;
using Zenject;

namespace SurgeEngine._Source.Code.Infrastructure.DI
{
    public class GameOptionsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameSettings>().FromNew().AsSingle().NonLazy();
        }
    }
}