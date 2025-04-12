using SurgeEngine.Code.Tools;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.DI
{
    public class SceneLoaderInstaller : MonoInstaller
    {
        [SerializeField] private SceneLoader prefab;

        public override void InstallBindings()
        {
            Debug.Log("INSTALLED SCENE LOADER BLA BLA BLA");
            Container.Bind<SceneLoader>().FromComponentInNewPrefab(prefab).AsSingle().NonLazy();
        }
    }
}