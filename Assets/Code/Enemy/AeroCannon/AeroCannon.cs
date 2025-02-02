using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Enemy.AeroCannon
{
    public class AeroCannon : EnemyBase, IDamageable
    {
        public void TakeDamage(object sender, float damage)
        {
            view.Destroy();
        }
    }
}