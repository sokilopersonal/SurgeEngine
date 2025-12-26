using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.StateMachine.Components;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Environment;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Source.Code.Infrastructure.Config.Sonic;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public class FStateSkydive : FCharacterState, IDamageableState, IPointMarkerLoader
    {
        private StateAnimator _stateAnimator => Character.Animation.StateAnimator;
        private Animator _animator => Character.Animation.StateAnimator.Animator;
        private bool _diving = false;
        private readonly SkydiveConfig _config;
        private float _speed;

        public FStateSkydive(CharacterBase owner) : base(owner)
        {
            owner.TryGetConfig(out _config);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            _diving = false;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (Input.XPressed && !_diving)
            {
                _diving = true;
                _stateAnimator.TransitionToState("SkydiveDown", 0.2f);
            }

            if (Input.XReleased && _diving)
            {
                _diving = false;
                _stateAnimator.TransitionToState("SkydiveDownEnd", 0f).Then(() => _stateAnimator.TransitionToState("SkydiveLoop", 0f));
            }

           _animator.SetFloat("Skydive", Mathf.Lerp(_animator.GetFloat("Skydive"), Character.Input.MoveVector.x, dt * 4f));
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            bool air = !Kinematics.CheckForGroundWithDirection(out var hit, Vector3.down);
            bool isWater = hit.transform.IsWater(out var surface);

            if (isWater)
            {
                StateMachine.SetState<FStateAir>();
                return;
            }

            _speed = Mathf.Lerp(_speed, _diving ? _config.diveSpeed : _config.fallSpeed, dt * _config.speedLerpSpeed);

            Kinematics.Rigidbody.linearVelocity = Kinematics.HorizontalVelocity + Vector3.Lerp(Kinematics.VerticalVelocity, Vector3.down * _speed, dt * _config.lerpSpeed);

            if (air)
            {
                Kinematics.Point = hit.point;
                Kinematics.Normal = Vector3.up;

                Kinematics.BasePhysics(Vector3.up, MovementType.Air);

                Vector3 vel = Kinematics.Velocity;
                vel.y = 0;
                Model.RotateBody(vel, Vector3.up, 180f);

                if (Kinematics.Path2D != null && Kinematics.GetAttachState())
                {
                    var path = Kinematics.Path2D.Spline;
                    var pos = path.EvaluatePosition();
                    var up = path.EvaluateUp();

                    var ray = new Ray(Rigidbody.position, pos - Rigidbody.position);
                    if (Physics.Raycast(ray, out var predictHit, 1f, Character.Config.castLayer, QueryTriggerInteraction.Ignore))
                    {
                        Kinematics.Normal = predictHit.normal;
                        Kinematics.Snap(pos + up);
                        Rigidbody.rotation = Quaternion.FromToRotation(Vector3.up, up) * Rigidbody.rotation;
                        Rigidbody.linearVelocity = Vector3.ProjectOnPlane(Rigidbody.linearVelocity, up);
                    }
                }
            }
            else
            {
                bool predictedGround = Kinematics.CheckForPredictedGround(dt, Character.Config.castDistance, 4);
                if (Kinematics.GetAttachState() && predictedGround)
                {
                    var vel = Kinematics.Velocity;
                    vel.y = 0;
                    float speed = vel.magnitude;
                    if (speed > Character.Config.landingSpeed) StateMachine.SetState<FStateGround>();
                    else
                    {
                        if (Kinematics.GetInputDir().magnitude < 0.1f)
                        {
                            StateMachine.SetState<FStateIdle>();
                        }
                        else
                        {
                            StateMachine.SetState<FStateGround>();
                        }
                    }
                }
            }
        }

        public void Load() { }
    }
}