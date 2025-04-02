using SurgeEngine.Code.Actor.States.BaseStates;
using SurgeEngine.Code.Actor.States.SonicSubStates;
using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Config.SonicSpecific;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Actor.States.SonicSpecific
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
                Common.ResetVelocity(ResetVelocityType.Both);

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

            timer += dt;
            if (timer > 0.85f && Kinematics.Speed > 1f)
            {
                StateMachine.SetState<FStateGround>();
            }
            if (timer > 1f && Kinematics.Speed < 1f)
            {
                if (Input.BHeld)
                {
                    StateMachine.SetState<FStateSit>();
                }
                else
                {
                    StateMachine.SetState<FStateIdle>();
                }
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            if (Common.CheckForGround(out RaycastHit hit, CheckGroundType.Normal, Actor.config.castDistance))
            {
                Vector3 point = hit.point;
                Vector3 normal = hit.normal; // loco put vector3.up here??
                Kinematics.Normal = normal;

                Kinematics.WriteMovementVector(_rigidbody.linearVelocity);
                _rigidbody.linearVelocity = Vector3.MoveTowards(_rigidbody.linearVelocity, Vector3.zero, _config.deceleration * dt);
                _rigidbody.linearVelocity = Vector3.ProjectOnPlane(_rigidbody.linearVelocity, normal);
                Model.RotateBody(normal);
                Kinematics.Snap(point, normal, true);
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