using System;
using NaughtyAttributes;
using SurgeEngine.Code.Core.Actor.HUD;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects;
using SurgeEngine.Code.Gameplay.CommonObjects.System;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.Infrastructure.DI
{
    public class GameplaySceneContext : MonoInstaller
    {
        [Header("Stage")]
        [SerializeField] private Stage stage;
        
        [Header("Character")]
        [SerializeField] private Transform actorPrefab;
        [SerializeField] private CharacterSpawn spawnPoint;
        
        [Header("HUD")]
        [SerializeField] private CharacterStageHUD hudPrefab;
        
        public override void InstallBindings()
        {
            SetupStage();
            SetupActor();
            SetupHUD();
        }

        private void SetupStage()
        {
            Container.Bind<Stage>().FromInstance(stage).AsSingle().NonLazy();
        }

        private void SetupActor()
        {
            if (!spawnPoint)
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
                throw new NullReferenceException("Spawn Point is not assigned, please do it in ActorInstaller under GameplaySceneContext. Stopping play mode...");
#endif
            }

            var data = spawnPoint.StartData;
            data.StartTransform = spawnPoint.transform;

            var instance = Container.InstantiatePrefabForComponent<CharacterBase>(actorPrefab, data.StartTransform.position, data.StartTransform.rotation, null);

            Container.Bind<CharacterBase>().FromInstance(instance).AsSingle().NonLazy();
            var parent = instance.transform.parent;
            Quaternion par = parent.rotation;
            parent.rotation = Quaternion.identity;
            instance.transform.rotation = par;
            
            Container.Bind<CharacterContext>().FromNew().AsSingle().NonLazy();
            
            instance.SetStart(data);
        }

        private void SetupHUD()
        {
            Container.InstantiatePrefabForComponent<CharacterStageHUD>(hudPrefab);
        }
    }
}