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
            var volume = Container.InstantiatePrefabForComponent<Volume>(volumePrefab);
            DontDestroyOnLoad(volume.gameObject);

            Container.BindInterfacesAndSelfTo<UserGraphics>().FromNew().AsSingle().WithArguments(volume.profile).NonLazy();
        }
    }
}