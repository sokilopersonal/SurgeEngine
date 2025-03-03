using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorSystem.Actors;
using SurgeEngine.Code.Config.SonicSpecific;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates.SonicSpecific
{
    public class FStateAirBoost : FStateMove
    {
        private float _timer;
        private readonly BoostConfig _config;
        
        public FStateAirBoost(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            owner.TryGetConfig(out _config);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0.3f;
        }

        public override void OnExit()
        {
            base.OnExit();
            
            FBoost boost = StateMachine.GetSubState<FBoost>();
            boost.canAirBoost = false;
            boost.Active = false;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            FBoost boost = StateMachine.GetSubState<FBoost>();
            if (boost.canAirBoost)
            {
                Vector3 force = _rigidbody.transform.forward * _config.airBoostSpeed;

                _rigidbody.linearVelocity = force;
            }
            
            if (Common.TickTimer(ref _timer, 0.3f))
            {
                StateMachine.SetState<FStateAir>();
            }

            if (!Actor.flags.HasFlag(FlagType.OutOfControl))
            {
                if (Input.BPressed)
                {
                    StateMachine.SetState<FStateStomp>();
                }
            }
        }
    }
}