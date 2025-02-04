using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorSystem.Actors;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Config.SonicSpecific;
using SurgeEngine.Code.Custom;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace SurgeEngine.Code.ActorStates.SonicSpecific
{
    public class FStateStomp : FStateMove
    {
        private float _timer;

        private bool released = false;
        private readonly StompConfig _config;

        public FStateStomp(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            owner.TryGetConfig(out _config);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            StateMachine.GetSubState<FBoost>().Active = false;
            released = false;
            _rigidbody.linearVelocity = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);
            
            Kinematics.Normal = Vector3.up;
            _rigidbody.rotation = Quaternion.LookRotation(Vector3.Cross(_rigidbody.transform.right, Vector3.up), Vector3.up);

            _timer = 0;
        }

        public override void OnExit()
        {
            base.OnExit();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (Input.BReleased)
                released = true;
            
            if (Common.CheckForGround(out RaycastHit hit))
            {
                Vector3 point = hit.point;
                Vector3 normal = hit.normal;

                float speed = Kinematics.HorizontalSpeed;

                if (speed > _config.slideSpeed && !released)
                {
                    StateMachine.SetState<FStateSlide>();
                }
                else
                {
                    StateMachine.SetState<FStateStompLand>();
                }
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            HurtBox.Create(Actor, Actor.transform.position + new Vector3(0f, -0.1f, 0f), Actor.transform.rotation,
                new Vector3(1.1f, 2f, 1.1f));

            Vector3 vel = _rigidbody.linearVelocity;
            float horizontalSpeedMultiplier = released ? 0f : _config.curve.Evaluate(_timer);
            Vector3 smoothedXZVelocity = new Vector3(vel.x * horizontalSpeedMultiplier, vel.y, vel.z * horizontalSpeedMultiplier);
            
            float stompSpeed = _config.speed;
            
            float minYVelocity = -stompSpeed * 1.25f;
            float maxYVelocity = 5f;

            vel = new Vector3(smoothedXZVelocity.x, 
                Mathf.Clamp(vel.y + -stompSpeed, minYVelocity, maxYVelocity), 
                smoothedXZVelocity.z);

            _rigidbody.linearVelocity = vel;
            _timer += dt;

            if (Common.CheckForGround(out RaycastHit hit))
            {
                Vector3 point = hit.point;
                Vector3 normal = hit.normal;

                float speed = Kinematics.HorizontalSpeed;

                if (speed < _config.slideSpeed && !released)
                    Common.ResetVelocity(ResetVelocityType.Both);

                Kinematics.Snap(point, normal, true);
            }
        }
    }
}