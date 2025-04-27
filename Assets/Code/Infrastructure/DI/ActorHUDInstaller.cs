using SurgeEngine.Code.Core.Actor.HUD;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.DI
{
    public class ActorHUDInstaller : MonoInstaller
    {
        [SerializeField] private ActorStageHUD hudPrefab;

        public override void InstallBindings()
        {
            Container.Bind<ActorStageHUD>().FromComponentInNewPrefab(hudPrefab).AsSingle().NonLazy();
            Container.Bind<ActorHUDContext>().FromNew().AsSingle().NonLazy();
        }
    }
}