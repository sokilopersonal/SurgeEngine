using SurgeEngine.Code.Tools;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.DI
{
    public class VolumeInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<VolumeManager>().FromNew().AsSingle().NonLazy();
        }
    }
}