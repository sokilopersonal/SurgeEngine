using System;
using NaughtyAttributes;
using SurgeEngine.Source.Code.Core.StateMachine;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.Base
{
    public class EnemyBase : StageObject
    {
        [SerializeField, Required] private EnemyView view;
        public EnemyView View => view;
        public FStateMachine StateMachine { get; private set; }

        public Action OnDied;
        protected bool IsDead { get; set; }

        protected virtual void Awake()
        {
            StateMachine = new FStateMachine();
            
            View.Initialize(this);
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

        private void LateUpdate()
        {
        }

        private void OnDeath()
        {
            IsDead = true;
            
            Stage.Instance.Data.AddScore(300);
        }
    }
}