using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic
{
    public class FStateSweepKick : FActorState
    {
        private float timer;

        private readonly SweepConfig _config;

        public FStateSweepKick(ActorBase owner) : base(owner)
        {
            owner.TryGetConfig(out _config);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            timer = 0f;

            if (Rigidbody.linearVelocity.magnitude < 1f)
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
                Kinematics.RotateSnapNormal(hit.normal);
                
                Rigidbody.linearVelocity = Vector3.MoveTowards(Rigidbody.linearVelocity, Vector3.zero, _config.deceleration * dt);
                Rigidbody.linearVelocity = Vector3.ProjectOnPlane(Rigidbody.linearVelocity, Kinematics.Normal);
                Model.RotateBody(Kinematics.Normal);
                
                Kinematics.Snap(Kinematics.Point, Kinematics.Normal, true);
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }

            if (timer >= 0.26f && timer <= 0.7f)
            {
                var offset = -Rigidbody.transform.up * 0.65f;
                var size = new Vector3(1.4f, 0.4f, 1.4f);
                HurtBox.CreateAttached(Actor, Rigidbody.transform, offset, size, HurtBoxTarget.Enemy | HurtBoxTarget.Breakable);
            }
        }
    }
}