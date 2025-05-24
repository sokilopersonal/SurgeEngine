using SurgeEngine.Code.Gameplay.CommonObjects.System;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.DI
{
    public class StageInstaller : MonoInstaller
    {
        [SerializeField] private Stage stage;
        
        public override void InstallBindings()
        {
            Container.Bind<Stage>().FromInstance(stage).AsSingle().NonLazy();
        }
    }
}