using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.StateMachine.Interfaces;
using SurgeEngine.Code.Gameplay.CommonObjects;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic
{
    public class FStateSlide : FCharacterState, IStateTimeout, IDamageableState
    {
        public float Timeout { get; set; }
        
        private readonly SlideConfig _config;
        private readonly QuickStepConfig _quickstepConfig;

        private bool _released;
        
        public FStateSlide(CharacterBase owner) : base(owner)
        {
            owner.TryGetConfig(out _config);
            owner.TryGetConfig(out _quickstepConfig);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Timeout = 0.25f;
            StateMachine.GetSubState<FBoost>().Active = false;
            _released = false;
            
            Model.SetLowerCollision();
        }

        public override void OnExit()
        {
            base.OnExit();

            Model.ResetCollisionToDefault();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (Input.BReleased)
                _released = true;

            bool ceiling = Kinematics.CheckForCeiling(out _);
            if (Kinematics.Speed < _config.minSpeed || _released)
            {
                if (Kinematics.Speed > _config.minSpeed)
                {
                    if (!ceiling)
                        StateMachine.SetState<FStateGround>();
                }
                else
                {
                    if (!_released)
                    {
                        StateMachine.SetState<FStateSit>();
                    }
                    else
                    {
                        if (!ceiling)
                            StateMachine.SetState<FStateIdle>();
                        else
                            StateMachine.SetState<FStateSit>();
                    }
                }
            }

            if (!ceiling)
            {
                if (Input.LeftBumperPressed)
                {
                    var qs = StateMachine.GetState<FStateQuickstep>();
                    qs.SetDirection(QuickstepDirection.Left).SetRun(true);
                    StateMachine.SetState<FStateQuickstep>();
                }
                else if (Input.RightBumperPressed)
                {
                    var qs = StateMachine.GetState<FStateQuickstep>();
                    qs.SetDirection(QuickstepDirection.Right).SetRun(true);
                    StateMachine.SetState<FStateQuickstep>();
                }
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            var config = character.Config;
            float distance = config.castDistance *
                             config.castDistanceCurve.Evaluate(Kinematics.Speed / config.topSpeed);
            if (Kinematics.CheckForGround(out RaycastHit hit, castDistance: distance))
            {
                Kinematics.Point = hit.point;
                Kinematics.Normal = hit.normal;
                Kinematics.Snap(hit.point, hit.normal, true);
                
                Rigidbody.linearVelocity = Vector3.MoveTowards(Rigidbody.linearVelocity, Vector3.zero, _config.deceleration * dt);
                Rigidbody.linearVelocity = Vector3.ProjectOnPlane(Rigidbody.linearVelocity, hit.normal);
                Model.RotateBody(hit.normal);
                
                Kinematics.SlopePhysics();
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }

            var transform = Rigidbody.transform;
            var offset = -transform.up * 0.6f;
            offset += transform.forward * 0.2f;
            HurtBox.CreateAttached(character, transform, offset, new Vector3(0.5f, 0.5f, 0.75f), HurtBoxTarget.Enemy | HurtBoxTarget.Breakable);
        }
    }
}