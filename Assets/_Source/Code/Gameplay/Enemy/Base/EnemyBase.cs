using System;
using NaughtyAttributes;
using SurgeEngine.Code.Core.StateMachine;
using SurgeEngine.Code.Gameplay.CommonObjects.System;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.Enemy.Base
{
    public class EnemyBase : MonoBehaviour
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
            StateMachine.LateTick(Time.deltaTime);
        }

        private void OnDeath()
        {
            IsDead = true;
            
            Stage.Instance.Data.AddScore(300);
        }
    }
}