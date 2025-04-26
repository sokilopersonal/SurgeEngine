using SurgeEngine.Code.Actor.States.BaseStates;
using SurgeEngine.Code.Actor.States.SonicSubStates;
using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Config.SonicSpecific;
using SurgeEngine.Code.Custom;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace SurgeEngine.Code.Actor.States.SonicSpecific
{
    public class FStateStomp : FStateMove
    {
        private float _timer;

        private bool _released;
        private readonly StompConfig _config;

        public FStateStomp(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            owner.TryGetConfig(out _config);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            StateMachine.GetSubState<FBoost>().Active = false;
            _released = false;
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
                _released = true;
            
            if (Common.CheckForGround(out RaycastHit hit))
            {
                Vector3 point = hit.point;
                Vector3 normal = hit.normal;
                Kinematics.Point = point;
                Kinematics.Normal = normal;

                float speed = Kinematics.HorizontalSpeed;
                float angle = Vector3.Angle(hit.normal, Vector3.up);
                if (angle >= 20 && Input.BHeld)
                {
                    StateMachine.SetState<FStateSlide>();
                    return;
                }
                
                if (speed > _config.slideSpeed && !_released)
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
                new Vector3(1.1f, 2f, 1.1f), HurtBoxTarget.Enemy | HurtBoxTarget.Breakable);

            Vector3 vel = _rigidbody.linearVelocity;
            float horizontalSpeedMultiplier = _released ? 0f : _config.curve.Evaluate(_timer);
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