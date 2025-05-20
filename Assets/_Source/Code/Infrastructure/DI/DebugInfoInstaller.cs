using SurgeEngine.Code.Infrastructure.Custom.Drawers;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.DI
{
    public class DebugInfoInstaller : MonoInstaller
    {
        [SerializeField] private DebugWindow windowPrefab;

        public override void InstallBindings()
        {
            var window = Container.InstantiatePrefabForComponent<DebugWindow>(windowPrefab);
            Container.Bind<DebugWindow>().FromInstance(window).AsSingle().NonLazy();
        }
    }
}