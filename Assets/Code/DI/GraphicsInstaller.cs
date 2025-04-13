using SurgeEngine.Code.Tools;
using UnityEngine;
using UnityEngine.Rendering;
using Zenject;

namespace SurgeEngine.Code.DI
{
    public class GraphicsInstaller : MonoInstaller
    {
        [SerializeField] private VolumeProfile volumeProfile;

        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<UserGraphics>().FromNew().AsSingle().WithArguments(volumeProfile).NonLazy();
        }
    }
}