using SurgeEngine.Code.Actor.States.BaseStates;
using SurgeEngine.Code.Actor.States.SonicSubStates;
using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.Config.SonicSpecific;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Actor.States.SonicSpecific
{
    public class FStateAirBoost : FStateMove
    {
        private float _timer;
        private readonly BoostConfig _config;
        
        public FStateAirBoost(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
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
            boost.CanAirBoost = false;
            boost.Active = false;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            FBoost boost = StateMachine.GetSubState<FBoost>();
            if (boost.CanAirBoost)
            {
                Vector3 direction = Vector3.Cross(_rigidbody.transform.right, Vector3.up);
                Vector3 force = direction * _config.airBoostSpeed;

                _rigidbody.linearVelocity = force;
                Model.RotateBody(Vector3.up);
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