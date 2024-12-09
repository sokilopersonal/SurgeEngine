using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorSystem.Actors;
using SurgeEngine.Code.Config.SonicSpecific;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateAirBoost : FStateMove
    {
        private float _timer;
        private BoostConfig _config;
        
        public FStateAirBoost(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            _config = (owner as Sonic).boostConfig;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0.25f;
            
            FBoost boost = StateMachine.GetSubState<FBoost>();
            if (boost.canAirBoost)
            {
                Vector3 force = _rigidbody.transform.forward * _config.airBoostSpeed;

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