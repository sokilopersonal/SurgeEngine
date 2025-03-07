using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorSystem;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Code.DI
{
    public class ActorInstaller : MonoInstaller
    {
        [SerializeField] private Transform actorPrefab;
        [SerializeField] private StartData data;

        public override void InstallBindings()
        {
            var instance = Container.InstantiatePrefabForComponent<Actor>(actorPrefab, data.startTransform.position, Quaternion.identity, null);
            instance.transform.rotation = data.startTransform.rotation;
            Container.Bind<Actor>().FromInstance(instance).AsSingle().NonLazy();
            Container.Bind<ActorContext>().FromNew().AsSingle().NonLazy();
            
            instance.SetStart(data);
        }
    }
}