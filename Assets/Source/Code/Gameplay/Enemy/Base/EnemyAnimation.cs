using SurgeEngine.Source.Code.Core.StateMachine.Base;
using SurgeEngine.Source.Code.Core.StateMachine.Components;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.Base
{
    [RequireComponent(typeof(StateAnimator))]
    public class EnemyAnimation : EnemyComponent
    {
        [SerializeField] protected StateAnimator stateAnimator;
        public Animator Animator => stateAnimator.Animator;

        private void Awake()
        {
            EnemyBase.StateMachine.OnStateAssign += ChangeStateAnimation;
        }

        protected virtual void ChangeStateAnimation(FState obj)
        {
            stateAnimator.StopAllCoroutines();
        }
    }
}