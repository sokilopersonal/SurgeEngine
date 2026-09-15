using UnityEngine;
using Zenject;

namespace SurgeEngine.Source.Code.Input
{
    public class InputIconInstaller : MonoInstaller
    {
        [SerializeField] private InputIconDatabase database;

        public override void InstallBindings()
        {
            Container.Bind<InputIconResolver>()
                .AsSingle()
                .WithArguments(database)
                .NonLazy();
        }
    }
}