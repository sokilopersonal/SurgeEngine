using System;
using SurgeEngine.Source.Code.Core.Character.HUD;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Source.Code.UI;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Source.Code.Infrastructure.DI
{
    public class GameplaySceneContext : MonoInstaller
    {
        [Header("Stage")]
        [SerializeField] private Stage stage;
        
        [Header("Character")]
        [SerializeField] private CharacterBase characterPrefab;
        [SerializeField] private CharacterSpawn spawnPoint;
        [SerializeField] private Camera gameCameraPrefab;
        
        [Header("HUD")]
        [SerializeField] private CharacterStageHUD hudPrefab;
        
        [Header("Point Marker")]
        [SerializeField] private PointMarkerLoadingScreen pointMarkerLoadingScreenPrefab;
        
        public override void InstallBindings()
        {
            SetupStage();
            SetupGameCamera();
            SetupCharacter();
            SetupHUD();
            SetupPointMarkerScreen();
        }

        private void SetupStage()
        {
            Container.BindInstance(stage).AsSingle().NonLazy();
        }

        private void SetupGameCamera()
        {
            Container.InstantiatePrefabForComponent<Camera>(gameCameraPrefab);
        }

        private void SetupCharacter()
        {
            if (!spawnPoint)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                throw new NullReferenceException("Spawn Point is not assigned, please do it. Stopping play mode...");
#endif
            }

            var data = spawnPoint.StartData;
            var spawn = spawnPoint.transform;

            var instance = Container.InstantiatePrefabForComponent<CharacterBase>(characterPrefab, spawn.position + spawn.transform.up, spawn.rotation, null);
            Container.BindInstance(instance).AsSingle();
            
            Container.Bind<CharacterContext>().FromNew().AsSingle().NonLazy();
            
            instance.SetStart(data);
        }

        private void SetupHUD()
        {
            var instance = Container.InstantiatePrefabForComponent<CharacterStageHUD>(hudPrefab);
            Container.BindInstance(instance).AsSingle().NonLazy();
        }

        private void SetupPointMarkerScreen()
        {
            var instance = Container.InstantiatePrefabForComponent<PointMarkerLoadingScreen>(pointMarkerLoadingScreenPrefab);
            Container.BindInstance(instance).AsSingle().NonLazy();
        }
    }
}