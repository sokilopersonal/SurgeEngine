using SurgeEngine.Code.Infrastructure.Tools.Managers;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.DI
{
    public class VolumeInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<VolumeManager>().FromNew().AsSingle().NonLazy();
        }
    }
}