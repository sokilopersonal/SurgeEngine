using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.States.SonicSubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States.SonicSpecific
{
    public class FStateSweepKick : FStateMove
    {
        private float timer;

        private readonly SweepConfig _config;

        public FStateSweepKick(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            owner.TryGetConfig(out _config);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            timer = 0f;

            if (_rigidbody.linearVelocity.magnitude < 1f)
                Kinematics.ResetVelocity();

            StateMachine.GetSubState<FBoost>().Active = false;

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
            bool ceiling = Kinematics.CheckForCeiling(out RaycastHit data);
            timer += dt;
            if (timer > 0.85f && Kinematics.Speed > 1f)
            {
                if (!ceiling)
                    StateMachine.SetState<FStateGround>();
                else
                    StateMachine.SetState<FStateCrawl>();
            }
            if (timer > 1f && Kinematics.Speed < 1f)
            {
                if (Input.BHeld)
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

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            if (Kinematics.CheckForGround(out RaycastHit hit))
            {
                Kinematics.Point = hit.point;
                Kinematics.SlerpSnapNormal(hit.normal);
                
                _rigidbody.linearVelocity = Vector3.MoveTowards(_rigidbody.linearVelocity, Vector3.zero, _config.deceleration * dt);
                _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, Kinematics.Normal);
                Model.RotateBody(Kinematics.Normal);
                
                Kinematics.Snap(Kinematics.Point, Kinematics.Normal, true);
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }

            if (timer >= 0.2f && timer <= 0.7f)
                HurtBox.Create(Actor, 
                    Actor.transform.position + new Vector3(0f, 0.25f, 0f),
                    Actor.transform.rotation, new Vector3(1f, 0.5f, 1f), HurtBoxTarget.Enemy | HurtBoxTarget.Breakable);
        }
    }
}