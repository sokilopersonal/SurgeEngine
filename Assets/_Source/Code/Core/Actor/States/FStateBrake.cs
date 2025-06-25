using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Config;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateBrake : FActorState, IDamageableState
    {
        private readonly PhysicsConfig _config;
        
        public FStateBrake(ActorBase owner) : base(owner)
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

            var config = Actor.Config;
            var curve = config.castDistanceCurve;
            var curveDistance = curve.Evaluate(Kinematics.Speed / _config.topSpeed);
            float distance = config.castDistance * curveDistance;
            
            if (Kinematics.CheckForGround(out var hit, castDistance: distance))
            {
                Kinematics.Normal = Vector3.up;
                Kinematics.Snap(hit.point, Vector3.up, true);
                
                float f = Mathf.Lerp(_config.maxSkiddingRate, _config.minSkiddingRate, Kinematics.Speed / _config.topSpeed);
                Rigidbody.linearVelocity = Vector3.MoveTowards(Rigidbody.linearVelocity, Vector3.zero,
                    Time.fixedDeltaTime * f);
                Kinematics.Project(hit.normal);
                Kinematics.SlopePhysics();

                Quaternion target = Quaternion.FromToRotation(Rigidbody.transform.up, Vector3.up) * Rigidbody.rotation;
                Rigidbody.rotation = Quaternion.Lerp(Rigidbody.rotation, target, Time.fixedDeltaTime * 8f);

                if (Rigidbody.linearVelocity.magnitude < 0.2f)
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