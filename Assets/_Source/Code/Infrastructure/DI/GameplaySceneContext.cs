using System;
using SurgeEngine._Source.Code.Core.Character.HUD;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Gameplay.CommonObjects;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine._Source.Code.UI;
using UnityEngine;
using Zenject;

namespace SurgeEngine._Source.Code.Infrastructure.DI
{
    public class GameplaySceneContext : MonoInstaller
    {
        [Header("Stage")]
        [SerializeField] private Stage stage;
        
        [Header("Character")]
        [SerializeField] private Transform characterPrefab;
        [SerializeField] private CharacterSpawn spawnPoint;
        
        [Header("HUD")]
        [SerializeField] private CharacterStageHUD hudPrefab;
        
        [Header("Point Marker")]
        [SerializeField] private PointMarkerLoadingScreen pointMarkerLoadingScreenPrefab;
        
        public override void InstallBindings()
        {
            SetupStage();
            SetupCharacter();
            SetupHUD();
            SetupPointMarkerScreen();
        }

        private void SetupStage()
        {
            Container.BindInstance(stage).AsSingle().NonLazy();
        }

        private void SetupCharacter()
        {
            if (!spawnPoint)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                throw new NullReferenceException("Spawn Point is not assigned, please do it in ActorInstaller under GameplaySceneContext. Stopping play mode...");
#endif
            }

            var data = spawnPoint.StartData;
            var spawn = spawnPoint.transform;

            var instance = Container.InstantiatePrefabForComponent<CharacterBase>(characterPrefab, spawn.position, spawn.rotation, null);
            Container.BindInstance(instance).AsSingle().NonLazy();
            var parent = instance.transform.parent;
            Quaternion par = parent.rotation;
            parent.rotation = Quaternion.identity;
            instance.transform.rotation = par;
            
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