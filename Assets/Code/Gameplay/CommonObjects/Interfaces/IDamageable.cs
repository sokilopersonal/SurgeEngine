using SurgeEngine.Code.Core.Actor.System;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Interfaces
{
    public interface IDamageable
    {
        void TakeDamage(Entity sender, float damage);
    }
}