using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Config;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateBrake : FStateMove, IDamageableState
    {
        private readonly BaseActorConfig _config;
        
        public FStateBrake(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            owner.TryGetConfig(out _config);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (Kinematics.MoveDot > 0)
            {
                StateMachine.SetState<FStateGround>();
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            if (Common.CheckForGround(out var hit))
            {
                Vector3 normal = hit.normal;
                Kinematics.Normal = normal;
                
                Kinematics.Snap(hit.point, normal);
                
                float f = Mathf.Lerp(_config.maxSkiddingRate, _config.minSkiddingRate, Kinematics.Speed / _config.topSpeed);
                _rigidbody.linearVelocity = Vector3.MoveTowards(_rigidbody.linearVelocity, Vector3.zero,
                    Time.fixedDeltaTime * f);
                Kinematics.Project();
                
                Model.RotateBody(normal);
                Kinematics.SlopePhysics();

                if (_rigidbody.linearVelocity.magnitude < 0.2f)
                {
                    if (Kinematics.Skidding) StateMachine.SetState<FStateBrakeTurn>();
                    else
                    {
                        StateMachine.SetState<FStateGround>();
                    }
                }
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }
    }
}