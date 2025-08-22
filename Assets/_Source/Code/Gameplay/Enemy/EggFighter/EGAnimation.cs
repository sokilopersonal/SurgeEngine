using System;
using SurgeEngine.Code.Core.StateMachine.Base;
using SurgeEngine.Code.Core.StateMachine.Components;
using SurgeEngine.Code.Gameplay.Enemy.Base;
using SurgeEngine.Code.Gameplay.Enemy.EggFighter.States;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.Enemy.EggFighter
{
    [RequireComponent(typeof(StateAnimator))]
    public class EGAnimation : EnemyAnimation
    {
        public event Action<Animator> OnAnimatorMoveEvent; 
        
        protected override void ChangeStateAnimation(FState obj)
        {
            base.ChangeStateAnimation(obj);
            
            if (obj is EGStateIdle)
            {
                stateAnimator.TransitionToState("Idle");
            }
            
            if (obj is EGStatePatrol or EGStateChase)
            {
                stateAnimator.TransitionToState("Walk", 0.1f);
            }
        
            if (obj is EGStatePunch)
            {
                stateAnimator.TransitionToState("Punch");
            }
        }

        private void OnAnimatorMove()
        {
            OnAnimatorMoveEvent?.Invoke(Animator);
        }
    }
}