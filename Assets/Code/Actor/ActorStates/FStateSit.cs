using SurgeEngine.Code.Custom;

namespace SurgeEngine.Code.Parameters
{
    public class FStateSit : FStateMove
    {
        public override void OnEnter()
        {
            base.OnEnter();
            
            animation.TransitionToState("Sit", 0.15f, true);
            
            Common.ResetVelocity(ResetVelocityType.Both);
        }

        public override void OnExit()
        {
            base.OnExit();
            
            animation.ResetAction();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (input.BReleased)
            {
                stateMachine.SetState<FStateIdle>(0.1f);
            }
        }
    }
}