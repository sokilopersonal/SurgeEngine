using System;
using SurgeEngine.Code.Core.StateMachine.Base;
using SurgeEngine.Code.Core.StateMachine.Components;
using SurgeEngine.Code.Gameplay.Enemy.EggFighter.States;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.Enemy.Base
{
    [RequireComponent(typeof(StateAnimator))]
    public class EnemyAnimation : EnemyComponent
    {
        protected StateAnimator _stateAnimator;
        
        public Animator Animator => _stateAnimator.Animator;

        private void Awake()
        {
            _stateAnimator = GetComponent<StateAnimator>();
        }

        public override void Initialize(EnemyBase enemyBase)
        {
            base.Initialize(enemyBase);
            enemyBase.StateMachine.OnStateAssign += ChangeStateAnimation;
        }

        protected virtual void ChangeStateAnimation(FState obj)
        {
            _stateAnimator.StopAllCoroutines();
        }
    }
}