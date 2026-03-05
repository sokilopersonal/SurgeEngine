using UnityEngine;
using Zenject;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.Base
{
    public class EnemyInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<EnemyBase>().FromComponentOnRoot().AsSingle().NonLazy();
        }
    }
}