using System;
using SurgeEngine.Code.Core.StateMachine.Base;
using SurgeEngine.Code.Core.StateMachine.Components;
using SurgeEngine.Code.Gameplay.Enemy.EggFighter.States;
using UnityEngine;
using UnityEngine.Serialization;

namespace SurgeEngine.Code.Gameplay.Enemy.Base
{
    [RequireComponent(typeof(StateAnimator))]
    public class EnemyAnimation : EnemyComponent
    {
        [SerializeField] protected StateAnimator stateAnimator;
        
        public Animator Animator => stateAnimator.Animator;

        public override void Initialize(EnemyBase enemyBase)
        {
            base.Initialize(enemyBase);
            enemyBase.StateMachine.OnStateAssign += ChangeStateAnimation;
        }

        protected virtual void ChangeStateAnimation(FState obj)
        {
            stateAnimator.StopAllCoroutines();
        }
    }
}