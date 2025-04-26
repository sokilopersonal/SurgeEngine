using SurgeEngine.Code.Tools;
using Zenject;

namespace SurgeEngine.Code.DI
{
    public class GameOptionsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GameSettings>().FromNew().AsSingle().NonLazy();
        }
    }
}