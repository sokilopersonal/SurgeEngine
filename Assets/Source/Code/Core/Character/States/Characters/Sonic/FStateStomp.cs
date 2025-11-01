using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Environment;
using SurgeEngine.Source.Code.Infrastructure.Config.SonicSpecific;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic
{
    public class FStateStomp : FCharacterState
    {
        private float _timer;

        private bool _released;
        private readonly StompConfig _config;

        public FStateStomp(CharacterBase owner) : base(owner)
        {
            owner.TryGetConfig(out _config);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            if (StateMachine.GetState(out FBoost boost))
                boost.Active = false;
            
            _released = false;
            Rigidbody.linearVelocity = new Vector3(Rigidbody.linearVelocity.x, 0f, Rigidbody.linearVelocity.z);
            
            Kinematics.Normal = Vector3.up;
            Rigidbody.rotation = Quaternion.LookRotation(Vector3.Cross(Rigidbody.transform.right, Vector3.up), Vector3.up);

            _timer = 0;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (Input.BReleased)
                _released = true;
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            HurtBox.CreateAttached(character, Rigidbody.transform, new Vector3(0f, -0.1f, 0f),
                new Vector3(1.1f, 2f, 1.1f), HurtBoxTarget.Enemy | HurtBoxTarget.Breakable);

            Vector3 vel = Rigidbody.linearVelocity;
            float horizontalSpeedMultiplier = _released ? 0f : _config.curve.Evaluate(_timer);
            Vector3 smoothedXZVelocity = new Vector3(vel.x * horizontalSpeedMultiplier, vel.y, vel.z * horizontalSpeedMultiplier);
            
            float stompSpeed = _config.speed;
            
            float minYVelocity = -stompSpeed * 1.25f;
            float maxYVelocity = 5f;

            vel = new Vector3(smoothedXZVelocity.x, 
                Mathf.Clamp(vel.y + -stompSpeed, minYVelocity, maxYVelocity), 
                smoothedXZVelocity.z);

            Rigidbody.linearVelocity = vel;
            _timer += dt;

            bool ground = Kinematics.CheckForGround(out RaycastHit hit);
            bool isWater = false;
            if (hit.transform != null)
            {
                isWater = hit.transform.gameObject.GetGroundTag() == GroundTag.Water;
                if (isWater)
                {
                    WaterSurface water = hit.transform.GetComponent<WaterSurface>();
                    water.Attach(Rigidbody.position, character);
                }
            }
            
            if (ground && !isWater)
            {
                Vector3 point = hit.point;
                Kinematics.Point = point;
                Kinematics.Normal = Vector3.up;

                if (hit.transform.TryGetComponent(out IStompHandler stompHandler))
                {
                    stompHandler.OnStomp();
                }

                float speed = Kinematics.HorizontalVelocity.magnitude;
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
                    Rigidbody.linearVelocity = Vector3.zero;
                    Kinematics.Snap(point, Vector3.up);
                    StateMachine.SetState<FStateStompLand>();
                }
            }
        }
    }

    public interface IStompHandler
    {
        void OnStomp();
    }
}