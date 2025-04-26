using SurgeEngine.Code.Actor.System;

namespace SurgeEngine.Code.CommonObjects
{
    public interface IDamageable
    {
        void TakeDamage(Entity sender, float damage);
    }
}