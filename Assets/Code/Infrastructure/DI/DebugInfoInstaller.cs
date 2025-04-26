using SurgeEngine.Code.CustomDebug;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.DI
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