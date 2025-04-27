using SurgeEngine.Code.UI;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.DI
{
    public class PauseInstaller : MonoInstaller
    {
        [SerializeField] private PauseHandler pauseHandlerPrefab;
        
        public override void InstallBindings()
        {
            var pauseHandler = Container.InstantiatePrefabForComponent<PauseHandler>(pauseHandlerPrefab);
            Container.Bind<PauseHandler>().FromInstance(pauseHandler).AsSingle().NonLazy();

            Container.Bind<PauseContext>().FromNew().AsSingle().NonLazy();
        }
    }
}