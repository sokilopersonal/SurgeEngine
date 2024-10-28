using SurgeEngine.Code.Custom;

namespace SurgeEngine.Code.Parameters
{
    public class FStateSit : FStateMove
    {
        public override void OnEnter()
        {
            base.OnEnter();
            
            Common.ResetVelocity(ResetVelocityType.Both);
        }

        public override void OnExit()
        {
            base.OnExit();
            
            Animation.ResetAction();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (!Input.BHeld)
            {
                StateMachine.SetState<FStateIdle>(0.1f);
            }
        }
    }
}