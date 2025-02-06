using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;

namespace SurgeEngine.Code.Enemy.Spinner
{
    public class Spinner : EnemyBase, IDamageable
    {
        public void TakeDamage(Entity sender, float damage)
        {
            view.Destroy();
        }
    }
}