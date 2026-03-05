using System;
using Alchemy.Inspector;
using SurgeEngine.Source.Code.Core.StateMachine;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.Base
{
    [RequireComponent(typeof(GameObjectContext), typeof(EnemyInstaller))]
    public class EnemyBase : StageObject
    {
        [SerializeField] private EnemyView view;
        public EnemyView View => view;
        public FStateMachine StateMachine { get; private set; }

        public Action OnDied;
        protected bool IsDead { get; set; }

        [Inject]
        private void Initialize()
        {
            StateMachine = new FStateMachine();
        }

        private void OnEnable()
        {
            OnDied += OnDeath;
        }
        
        private void OnDisable()
        {
            OnDied -= OnDeath;
        }

        private void Update()
        {
            StateMachine.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            StateMachine.FixedTick(Time.fixedDeltaTime);
        }
        
        private void OnDeath()
        {
            IsDead = true;
            
            Stage.Instance.Data.AddScore(300);
        }
    }
}