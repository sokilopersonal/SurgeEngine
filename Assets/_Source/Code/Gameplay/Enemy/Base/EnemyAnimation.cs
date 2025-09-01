using SurgeEngine._Source.Code.Core.StateMachine.Base;
using SurgeEngine._Source.Code.Core.StateMachine.Components;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.Enemy.Base
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