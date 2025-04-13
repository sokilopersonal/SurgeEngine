using SurgeEngine.Code.Tools;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace SurgeEngine.Code.DI
{
    public class GraphicsInstaller : MonoInstaller
    {
        [SerializeField] private Volume volumePrefab;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UserGraphics>().FromNew().AsSingle().WithArguments(volumePrefab).NonLazy();
        }
    }
}