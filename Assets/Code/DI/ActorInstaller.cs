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
            var instanceObject = Instantiate(actorPrefab, data.startTransform.position, data.startTransform.rotation);
            var instance = instanceObject.GetComponentInChildren<Actor>();
            
            Container.Bind<Actor>().FromInstance(instance).AsSingle().NonLazy();
            Container.Bind<ActorContext>().FromNew().AsSingle().NonLazy();

            Container.InjectGameObject(instanceObject.gameObject);
            
            instance.SetStart(data);
            
            Quaternion par = instance.transform.parent.rotation;
            instance.transform.parent.rotation = Quaternion.identity;
            instance.transform.rotation = par;
        }
    }
}