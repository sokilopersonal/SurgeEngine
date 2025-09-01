using SurgeEngine._Source.Code.Gameplay.Enemy.Base;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.Enemy.EggFighter
{
    public class EGEffects : EnemyComponent
    {
        [SerializeField] private ParticleSystem hitPrefab;
        
        public override void Initialize(EnemyBase enemyBase)
        {
            base.Initialize(enemyBase);
            enemyBase.OnDied += OnDied;
        }

        private void OnDied()
        {
            var instance = Instantiate(hitPrefab, transform.position + transform.up, Quaternion.identity);
            Destroy(instance.gameObject, 1f);
        }
    }
}