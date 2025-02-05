using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.StateMachine;
using SurgeEngine.Code.StateMachine.Components;
using UnityEngine;

namespace SurgeEngine.Code.Enemy
{
    [DefaultExecutionOrder(-9888)]
    public class EnemyBase : Entity
    {
        public EnemyView view;
        public new StateAnimator animation;

        protected override void Awake()
        {
            base.Awake();
            
            foreach (var component in new IEnemyComponent[] { view })
            {
                component?.SetOwner(this);
            }
            
            animation?.Initialize(stateMachine);
        }
        
        private void Update()
        {
            stateMachine.Tick(Time.deltaTime);
        }

        private void FixedUpdate()
        {
            stateMachine.FixedTick(Time.fixedDeltaTime);
        }

        private void LateUpdate()
        {
            stateMachine.LateTick(Time.deltaTime);
        }
    }
}