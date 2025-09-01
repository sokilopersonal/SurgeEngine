using SurgeEngine._Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine._Source.Code.Gameplay.Enemy.Base;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Events
{
    public class EnemyDeathEvent : BaseStageEvent, IPointMarkerLoader
    {
        [SerializeField] private EnemyBase[] enemies;

        private int _enemiesCount => enemies.Length;
        private int _enemiesDied;

        private void Awake()
        {
            _enemiesDied = 0;
        }

        private void OnEnable()
        {
            foreach (var enemy in enemies)
            {
                enemy.OnDied += OnEnemyDied;
            }
        }
        
        private void OnDisable()
        {
            foreach (var enemy in enemies)
            {
                enemy.OnDied -= OnEnemyDied;
            }
        }

        private void OnEnemyDied()
        {
            _enemiesDied++;
            
            if (_enemiesDied == _enemiesCount)
            {
                OnEventInvoked?.Invoke();
            }
        }

        public void Load()
        {
            _enemiesDied = 0;
        }
    }
}