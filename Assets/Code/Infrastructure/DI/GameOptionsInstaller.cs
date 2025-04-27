using SurgeEngine.Code.Infrastructure.Tools.Managers;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.DI
{
    public class GameOptionsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameSettings>().FromNew().AsSingle().NonLazy();
        }
    }
}