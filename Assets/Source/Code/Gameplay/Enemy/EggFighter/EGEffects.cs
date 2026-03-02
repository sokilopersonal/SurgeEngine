using System;
using SurgeEngine.Source.Code.Gameplay.Enemy.Base;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.EggFighter
{
    public class EGEffects : EnemyComponent
    {
        [SerializeField] private ParticleSystem hitPrefab;

        private void Awake()
        {
            EnemyBase.OnDied += OnDied;
        }

        private void OnDied()
        {
            var instance = Instantiate(hitPrefab, transform.position + transform.up, Quaternion.identity);
            Destroy(instance.gameObject, 1f);
        }
    }
}