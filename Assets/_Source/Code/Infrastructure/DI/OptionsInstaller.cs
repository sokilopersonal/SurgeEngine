using SurgeEngine._Source.Code.Infrastructure.Tools.Managers;
using UnityEngine;
using Zenject;

namespace SurgeEngine._Source.Code.Infrastructure.DI
{
    public class OptionsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UserGraphics>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<GameSettings>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<UserDisplay>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<VolumeManager>().FromNew().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<UserInput>().FromNew().AsSingle().NonLazy();
        }
    }
}