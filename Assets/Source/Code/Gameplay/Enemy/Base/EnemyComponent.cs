using UnityEngine;
using Zenject;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.Base
{
    public class EnemyComponent : MonoBehaviour
    {
        protected EnemyBase EnemyBase;

        [Inject]
        private void Initialize(EnemyBase enemyBase)
        {
            EnemyBase = enemyBase;
        }
    }
}