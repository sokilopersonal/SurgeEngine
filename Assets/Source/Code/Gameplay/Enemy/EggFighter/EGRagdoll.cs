using System;
using SurgeEngine.Source.Code.Gameplay.Enemy.Base;
using SurgeEngine.Source.Code.Gameplay.Enemy.RagdollPhysics;
using Zenject;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.EggFighter
{
    public class EGRagdoll : EnemyRagdoll
    {
        private EggFighter _eggFighter;
        
        [Inject]
        private void Initialize(EnemyBase enemy)
        {
            if (enemy is EggFighter eggFighter)
                _eggFighter = eggFighter;
            else throw new NullReferenceException("Enemy is not an EggFighter");
        }

        protected override void OnExplode()
        {
            base.OnExplode();
            
            _eggFighter.View.Destroy();
        }
    }
}