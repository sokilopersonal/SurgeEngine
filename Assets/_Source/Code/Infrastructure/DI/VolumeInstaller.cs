using SurgeEngine._Source.Code.Infrastructure.Tools.Managers;
using Zenject;

namespace SurgeEngine._Source.Code.Infrastructure.DI
{
    public class VolumeInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<VolumeManager>().FromNew().AsSingle().NonLazy();
        }
    }
}