using SurgeEngine.Code.ActorHUD;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.DI
{
    public class ActorHUDInstaller : MonoInstaller
    {
        [SerializeField] private ActorStageHUD hudPrefab;

        public override void InstallBindings()
        {
            Container.Bind<ActorStageHUD>().FromComponentInNewPrefab(hudPrefab).AsSingle().NonLazy();
        }
    }
}