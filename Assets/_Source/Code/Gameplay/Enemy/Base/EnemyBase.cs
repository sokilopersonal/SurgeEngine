using System;
using SurgeEngine.Code.Core.StateMachine;
using SurgeEngine.Code.Gameplay.CommonObjects.System;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.Enemy.Base
{
    public class EnemyBase : MonoBehaviour
    {
        public EnemyView view;
        public new EnemyAnimation animation;

        private FStateMachine _stateMachine;
        public FStateMachine StateMachine => _stateMachine;

        public Action OnDied;
        public bool IsDead { get; protected set; }

        protected virtual void Awake()
        {
            _stateMachine = new FStateMachine();
            
            view?.Initialize(this);
            animation?.Initialize(this);
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
            _stateMachine.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            _stateMachine.FixedTick(Time.fixedDeltaTime);
        }

        private void LateUpdate()
        {
            _stateMachine.LateTick(Time.deltaTime);
        }

        private void OnDeath()
        {
            IsDead = true;
            
            Stage.Instance.Data.AddScore(300);
        }
    }
}