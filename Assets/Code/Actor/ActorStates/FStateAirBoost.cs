using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Parameters.SonicSubStates;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public class FStateAirBoost : FStateMove
    {
        public FStateAirBoost(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();

            FBoost boost = StateMachine.GetSubState<FBoost>();
            if (boost.canAirBoost)
            {
                if (boost.ApplyAirForce(_rigidbody, _rigidbody.transform.forward * boost.GetBoostEnergyGroup().GetParameter<float>("AirBoostSpeed")))
                {
                    boost.canAirBoost = false;
                }
            }
            
            StateMachine.SetState<FStateAir>();
        }

        public override void OnExit()
        {
            base.OnExit();
            
            Animation.ResetAction();
        }
    }
}