using SurgeEngine._Source.Code.Infrastructure.Tools.Managers;
using Zenject;

namespace SurgeEngine._Source.Code.Infrastructure.DI
{
    public class DisplayInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UserDisplay>().FromNew().AsSingle().NonLazy();
        }
    }
}