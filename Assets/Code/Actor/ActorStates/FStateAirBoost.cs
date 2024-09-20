using SurgeEngine.Code.Parameters.SonicSubStates;
using UnityEngine;

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
                animation.TransitionToState("Air Boost", 1f, true);
                _rigidbody.linearVelocity = _rigidbody.transform.forward * boost.airStartForce;
                boost.canAirBoost = false;
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