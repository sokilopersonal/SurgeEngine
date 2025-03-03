using SurgeEngine.Code.Enemy.EggFighter.States;
using SurgeEngine.Code.StateMachine;
using SurgeEngine.Code.StateMachine.Components;

namespace SurgeEngine.Code.Enemy
{
    public class EGAnimation : StateAnimator, IEnemyComponent
    {
        public EnemyBase enemyBase { get; set; }

        protected override void AnimationTick()
        {
            
        }

        protected override void ChangeStateAnimation(FState obj)
        {
            base.ChangeStateAnimation(obj);

            if (obj is EGStateIdle)
            {
                TransitionToState("Idle");
            }
            
            if (obj is EGStatePatrol)
            {
                TransitionToState("Walk", 0.1f);
            }
            
            if (obj is EGStateChase)
            {
                TransitionToState("Run");
            }

            if (obj is EGStatePunch)
            {
                TransitionToState("PunchCharge", 0.25f);
            }

            if (obj is EGStateTurn)
            {
                TransitionToState("Turn", 0f);
            }

            if (obj is EGStateDead)
            {
                TransitionToState("IdleDead", 0f);
            }
        }
    }
}