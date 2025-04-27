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
        protected override void ChangeStateAnimation(FState obj)
        {
            base.ChangeStateAnimation(obj);
            
            if (obj is EGStateIdle)
            {
                _stateAnimator.TransitionToState("Idle");
            }
            
            if (obj is EGStatePatrol)
            {
                _stateAnimator.TransitionToState("Walk", 0.1f);
            }
            
            if (obj is EGStateChase)
            {
                _stateAnimator.TransitionToState("Run");
            }
        
            if (obj is EGStatePunch)
            {
                _stateAnimator.TransitionToState("PunchCharge", 0.25f);
            }
        
            if (obj is EGStateTurn)
            {
                _stateAnimator.TransitionToState("Turn", 0f);
            }
        
            if (obj is EGStateDead)
            {
                _stateAnimator.TransitionToState("IdleDead", 0f);
            }
        }
    }
}