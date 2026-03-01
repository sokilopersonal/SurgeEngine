using SurgeEngine.Source.Code.Core.Character.System;
using Zenject;

namespace SurgeEngine.Source.Code.Infrastructure.DI
{
    public class CharacterInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInstance(GetComponent<CharacterBase>()).AsSingle();
        }
    }
}