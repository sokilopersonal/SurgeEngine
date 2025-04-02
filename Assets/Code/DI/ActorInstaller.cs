using SurgeEngine.Code.Actor.States;
using SurgeEngine.Code.Actor.System;
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
            data.StartTransform = GameObject.FindGameObjectWithTag("SpawnPoint").transform;
            
            var instanceObject = Instantiate(actorPrefab, data.StartTransform.position, data.StartTransform.rotation);
            var instance = instanceObject.GetComponentInChildren<ActorBase>();
            
            Container.Bind<ActorBase>().FromInstance(instance).AsSingle().NonLazy();
            Container.Bind<ActorContext>().FromNew().AsSingle().NonLazy();

            Container.InjectGameObject(instanceObject.gameObject);
            
            instance.SetStart(data);
            
            Quaternion par = instance.transform.parent.rotation;
            instance.transform.parent.rotation = Quaternion.identity;
            instance.transform.rotation = par;

            var startRenderer = data.StartTransform.GetComponentInChildren<Renderer>();
            if (startRenderer)
            {
                startRenderer.enabled = false; // Disable model if exists after spawning
            }
        }
    }
}