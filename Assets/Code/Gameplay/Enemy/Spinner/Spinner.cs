using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.Interfaces;
using SurgeEngine.Code.Gameplay.Enemy.Base;

namespace SurgeEngine.Code.Gameplay.Enemy.Spinner
{
    public class Spinner : EnemyBase, IDamageable
    {
        public void TakeDamage(Entity sender, float damage)
        {
            view.Destroy();
        }
    }
}