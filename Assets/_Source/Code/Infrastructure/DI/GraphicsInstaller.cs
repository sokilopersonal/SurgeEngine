using SurgeEngine.Code.Infrastructure.Tools.Managers;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.DI
{
    public class GraphicsInstaller : MonoInstaller
    {
        [SerializeField] private VolumeProfile volumeProfile;

        public override void InstallBindings()
        {
            SceneManager.LoadScene("AdditiveGraphics", LoadSceneMode.Additive);
            Container.BindInterfacesAndSelfTo<UserGraphics>().FromNew().AsSingle().WithArguments(volumeProfile).NonLazy();
        }
    }
}