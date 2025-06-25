using SurgeEngine.Code.Infrastructure.Tools.Managers;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.DI
{
    public class GraphicsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UserGraphics>().FromNew().AsSingle().NonLazy();
        }
    }
}