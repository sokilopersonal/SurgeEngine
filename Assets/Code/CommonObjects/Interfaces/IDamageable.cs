using SurgeEngine.Code.ActorSystem;

namespace SurgeEngine.Code.CommonObjects
{
    public interface IDamageable
    {
        void TakeDamage(Entity sender, float damage);
    }
}