using SurgeEngine.Code.Core.Actor.HUD;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.DI
{
    public class HUDSpawner : MonoInstaller
    {
        [SerializeField] private ActorStageHUD hudPrefab;

        public override void InstallBindings()
        {
            Container.InstantiatePrefabForComponent<ActorStageHUD>(hudPrefab);
        }
    }
}