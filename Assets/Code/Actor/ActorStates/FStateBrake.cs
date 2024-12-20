using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Config;
using SurgeEngine.Code.Custom;
using UnityEngine;
namespace SurgeEngine.Code.ActorStates
{
    public class FStateBrake : FStateMove
    {
        private BaseActorConfig _config;
        
        public FStateBrake(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            _config = owner.config;
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
                Kinematics.Project();
                
                float f = Mathf.Lerp(_config.maxSkiddingRate, _config.minSkiddingRate, Kinematics.HorizontalSpeed / _config.topSpeed);
                _rigidbody.linearVelocity = Vector3.MoveTowards(_rigidbody.linearVelocity, Vector3.zero,
                    Time.fixedDeltaTime * f);

                if (_rigidbody.linearVelocity.magnitude < 0.2f)
                {
                    StateMachine.SetState<FStateBrakeTurn>();
                }
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }
    }
}