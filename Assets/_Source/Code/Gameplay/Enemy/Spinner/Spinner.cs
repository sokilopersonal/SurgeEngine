using SurgeEngine.Code.Gameplay.CommonObjects.Interfaces;
using SurgeEngine.Code.Gameplay.Enemy.Base;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.Enemy.Spinner
{
    public class Spinner : EnemyBase, IDamageable
    {
        public void TakeDamage(Component sender)
        {
            view.Destroy();
        }
    }
}