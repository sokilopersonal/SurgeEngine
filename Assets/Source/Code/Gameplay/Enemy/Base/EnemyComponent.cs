using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.Base
{
    public class EnemyComponent : MonoBehaviour
    {
        protected EnemyBase enemyBase;

        public virtual void Initialize(EnemyBase enemyBase) => this.enemyBase = enemyBase;
    }
}