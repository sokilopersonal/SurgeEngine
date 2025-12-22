using System;
using System.Collections;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using SurgeEngine.Source.Code.Core.StateMachine.Components;
using SurgeEngine.Source.Code.Gameplay.Enemy.Base;
using SurgeEngine.Source.Code.Gameplay.Enemy.EggFighter.States;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.Enemy.EggFighter
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
                stateAnimator.TransitionToState("Idle", 0.4f);
            }
            
            if (obj is EGStateChase)
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