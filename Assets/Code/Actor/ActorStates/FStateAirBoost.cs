using SurgeEngine.Code.Parameters.SonicSubStates;

namespace SurgeEngine.Code.Parameters
{
    public class FStateAirBoost : FStateMove
    {
        public override void OnEnter()
        {
            base.OnEnter();

            FBoost boost = stateMachine.GetSubState<FBoost>();
            if (boost.canAirBoost)
            {
                if (boost.ApplyAirForce(_rigidbody, _rigidbody.transform.forward * boost.airStartForce))
                {
                    animation.TransitionToState("Air Boost", 1f, true);
                    boost.canAirBoost = false;
                }
            }
            
            stateMachine.SetState<FStateAir>();
        }

        public override void OnExit()
        {
            base.OnExit();
            
            animation.SetAction(false);
        }
    }
}