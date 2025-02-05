using SurgeEngine.Code.CommonObjects;

namespace SurgeEngine.Code.Enemy.Spinner
{
    public class Spinner : EnemyBase, IDamageable
    {
        public void TakeDamage(object sender, float damage)
        {
            view.Destroy();
        }
    }
}