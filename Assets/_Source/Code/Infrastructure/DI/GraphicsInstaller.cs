using SurgeEngine._Source.Code.Infrastructure.Tools.Managers;
using Zenject;

namespace SurgeEngine._Source.Code.Infrastructure.DI
{
    public class GraphicsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UserGraphics>().FromNew().AsSingle().NonLazy();
        }
    }
}