using SurgeEngine._Source.Code.Core.StateMachine.Base;
using SurgeEngine._Source.Code.Gameplay.Enemy.AeroCannon.States;
using SurgeEngine._Source.Code.Gameplay.Enemy.Base;

namespace SurgeEngine._Source.Code.Gameplay.Enemy.AeroCannon
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