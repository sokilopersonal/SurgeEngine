using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.GameDocuments;
using SurgeEngine.Code.Parameters.SonicSubStates;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public class FStateAirBoost : FStateMove
    {
        private float _timer;
        
        public FStateAirBoost(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0.25f;
            
            FBoost boost = StateMachine.GetSubState<FBoost>();
            if (boost.canAirBoost)
            {
                Vector3 force = _rigidbody.transform.forward * boost.GetBoostEnergyGroup()
                    .GetParameter<float>(SonicGameDocumentParams.BoostEnergy_AirBoostSpeed);

                _rigidbody.linearVelocity = force;
                boost.canAirBoost = false;
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            
            Animation.ResetAction();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (_timer > 0)
            {
                _timer -= dt;
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }
    }
}