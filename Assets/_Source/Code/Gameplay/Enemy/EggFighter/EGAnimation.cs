using System;
using SurgeEngine._Source.Code.Core.StateMachine.Base;
using SurgeEngine._Source.Code.Core.StateMachine.Components;
using SurgeEngine._Source.Code.Gameplay.Enemy.Base;
using SurgeEngine._Source.Code.Gameplay.Enemy.EggFighter.States;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.Enemy.EggFighter
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