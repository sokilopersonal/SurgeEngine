using SurgeEngine.Code.Actor.System;
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
        }

        protected override void Update()
        {
            stateMachine.Tick(Time.deltaTime);
        }

        protected override void FixedUpdate()
        {
            stateMachine.FixedTick(Time.fixedDeltaTime);
        }

        protected override void LateUpdate()
        {
            stateMachine.LateTick(Time.deltaTime);
        }
    }
}