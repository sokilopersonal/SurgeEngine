using SurgeEngine.Code.Core.StateMachine.Base;
using SurgeEngine.Code.Gameplay.Enemy.AeroCannon.States;
using SurgeEngine.Code.Gameplay.Enemy.Base;

namespace SurgeEngine.Code.Gameplay.Enemy.AeroCannon
{
    public class AeroCannonAnimation : EnemyAnimation
    {
        protected override void ChangeStateAnimation(FState obj)
        {
            base.ChangeStateAnimation(obj);
            
            if (obj is ACStateIdle)
            {
                stateAnimator.TransitionToState("Idle");
            }
        
            if (obj is ACStateShoot)
            {
                stateAnimator.TransitionToState("Shoot");
            }
        }
    }
}