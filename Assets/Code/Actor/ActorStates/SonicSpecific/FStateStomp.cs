using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorSystem.Actors;
using SurgeEngine.Code.Config.SonicSpecific;
using SurgeEngine.Code.Custom;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateStomp : FStateMove
    {
        private float _timer;
        private StompConfig _config;

        public FStateStomp(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            _config = (owner as Sonic).stompConfig;
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            StateMachine.GetSubState<FBoost>().Active = false;
            _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);

            _timer = 0;
        }

        public override void OnExit()
        {
            base.OnExit();
            
            Animation.ResetAction();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            if (Common.CheckForGround(out var hit))
            {
                var point = hit.point;
                var normal = hit.normal;

                Common.ResetVelocity(ResetVelocityType.Both);
                Kinematics.Snap(point, normal, true);

                StateMachine.SetState<FStateIdle>();
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            Vector3 vel = _rigidbody.linearVelocity;
            float horizontalSpeedMultiplier = _config.curve.Evaluate(_timer);
            Vector3 smoothedXZVelocity = new Vector3(vel.x * horizontalSpeedMultiplier, vel.y, vel.z * horizontalSpeedMultiplier);
            
            float stompSpeed = _config.speed;
            
            float minYVelocity = -stompSpeed * 1.25f;
            float maxYVelocity = 5f;

            vel = new Vector3(smoothedXZVelocity.x, 
                Mathf.Clamp(vel.y + -stompSpeed, minYVelocity, maxYVelocity), 
                smoothedXZVelocity.z);

            _rigidbody.linearVelocity = vel;
            _timer += dt;
        }
    }
}