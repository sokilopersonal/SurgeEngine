using System;
using NaughtyAttributes;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;
using Zenject;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SurgeEngine.Code.Infrastructure.DI
{
    public class ActorInstaller : MonoInstaller
    {
        [SerializeField, Required] private Transform actorPrefab;
        [SerializeField, Required] private Transform spawnPoint;
        [SerializeField] private StartData data;

        public override void InstallBindings()
        {
            if (!spawnPoint)
            {
#if UNITY_EDITOR
                EditorApplication.isPlaying = false;
                throw new NullReferenceException("Spawn Point is not assigned, please do it in ActorInstaller under GameplaySceneContext. Stopping play mode...");
#endif
            }
            
            data.StartTransform = spawnPoint;

            var instance = Container.InstantiatePrefabForComponent<ActorBase>(actorPrefab, data.StartTransform.position, data.StartTransform.rotation, null);

            Container.Bind<ActorBase>().FromInstance(instance).AsSingle().NonLazy();
            Container.Bind<ActorContext>().FromNew().AsSingle().NonLazy();
            
            instance.SetStart(data);
            
            Quaternion par = instance.transform.parent.rotation;
            instance.transform.parent.rotation = Quaternion.identity;
            instance.transform.rotation = par;
        }
    }
}