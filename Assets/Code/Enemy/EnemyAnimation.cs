using SurgeEngine.Code.Enemy.States;
using SurgeEngine.Code.StateMachine.Components;

namespace SurgeEngine.Code.StateMachine
{
    public class EnemyAnimation : StateAnimator, IEnemyComponent
    {
        public EnemyBase enemyBase { get; set; }

        private void OnEnable()
        {
            enemyBase.stateMachine.OnStateAssign += OnStateAssign;
        }

        private void OnDisable()
        {
            enemyBase.stateMachine.OnStateAssign -= OnStateAssign;
        }

        protected override void AnimationTick()
        {
            
        }

        private void OnStateAssign(FState obj)
        {
            var prev = enemyBase.stateMachine.PreviousState;
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
                TransitionToState("PunchCharge", 0.25f, true);
            }

            if (obj is EGStateTurn)
            {
                TransitionToState("Turn", 0f, true);
            }

            if (obj is EGStateDead)
            {
                TransitionToState("IdleDead", 0f, true);
            }
        }
    }
}