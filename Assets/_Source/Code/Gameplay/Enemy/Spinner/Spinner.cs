using SurgeEngine._Source.Code.Gameplay.CommonObjects.Interfaces;
using SurgeEngine._Source.Code.Gameplay.Enemy.Base;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.Enemy.Spinner
{
    public class Spinner : EnemyBase, IDamageable
    {
        public void TakeDamage(Component sender)
        {
            View.Destroy();
        }
    }
}