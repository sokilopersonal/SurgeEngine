using SurgeEngine.Code.Infrastructure.Tools.Managers;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.DI
{
    public class DisplayInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UserDisplay>().FromNew().AsSingle().NonLazy();
        }
    }
}