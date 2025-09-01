using SurgeEngine._Source.Code.Infrastructure.Tools.Managers;
using UnityEngine;
using Zenject;

namespace SurgeEngine._Source.Code.Infrastructure.DI
{
    public class SceneLoaderInstaller : MonoInstaller
    {
        [SerializeField] private SceneLoader prefab;

        public override void InstallBindings()
        {
            Container.Bind<SceneLoader>().FromComponentInNewPrefab(prefab).AsSingle().NonLazy();
        }
    }
}