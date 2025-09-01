using SurgeEngine._Source.Code.UI;
using SurgeEngine._Source.Code.UI.Pages.Baseline;
using UnityEngine;
using Zenject;

namespace SurgeEngine._Source.Code.Infrastructure.DI
{
    public class PauseInstaller : MonoInstaller
    {
        [SerializeField] private PauseHandler pauseHandlerPrefab;
        
        public override void InstallBindings()
        {
            var pauseHandler = Container.InstantiatePrefabForComponent<PauseHandler>(pauseHandlerPrefab);
            Container.Bind<PageController>().FromComponentOn(pauseHandler.gameObject).AsSingle().NonLazy();
            Container.Bind<PauseHandler>().FromInstance(pauseHandler).AsSingle().NonLazy();
        }
    }
}