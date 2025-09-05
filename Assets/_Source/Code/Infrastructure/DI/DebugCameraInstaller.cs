using SurgeEngine._Source.Code.Infrastructure.Tools;
using UnityEngine;
using Zenject;

namespace SurgeEngine._Source.Code.Infrastructure.DI
{
    public class DebugCameraInstaller : MonoInstaller
    {
        [SerializeField] private DebugCamera debugCameraPrefab;
        
        public override void InstallBindings()
        {
            var instance = Container.InstantiatePrefabForComponent<DebugCamera>(debugCameraPrefab);
            Container.BindInstance(instance).AsSingle().NonLazy();
        }
    }
}