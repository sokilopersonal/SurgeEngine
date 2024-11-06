using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.GameDocuments;
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
                Vector3 force = _rigidbody.transform.forward * boost.GetBoostEnergyGroup()
                    .GetParameter<float>(SonicGameDocumentParams.BoostEnergy_AirBoostSpeed);

                _rigidbody.linearVelocity = force;
                boost.canAirBoost = false;
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