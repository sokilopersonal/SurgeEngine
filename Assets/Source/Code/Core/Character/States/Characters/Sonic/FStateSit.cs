using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Infrastructure.Config.Sonic;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic
{
    public class FStateSit : FCharacterState, IDamageableState
    {
        private readonly QuickStepConfig _quickstepConfig;

        private Vector3 _squatVelocity;
        
        public FStateSit(CharacterBase owner) : base(owner)
        {
            owner.TryGetConfig(out _quickstepConfig);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Kinematics.ResetVelocity();

            if (!StateMachine.IsPrevExact<FStateSweepKick>())
                Rigidbody.AddForce(Character.transform.forward, ForceMode.Impulse);

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

            if (!Input.BHeld && !ceiling)
            {
                StateMachine.SetState<FStateIdle>();
            }

            if (Input.APressed && !ceiling)
            {
                Kinematics.SetDetachTime(0.1f);
                StateMachine.SetState<FStateJump>();
            }

            if (Input.MoveVector.magnitude >= 0.2f)
            {
                StateMachine.SetState<FStateCrawl>();
            }

            if (Input.LeftBumperPressed && !ceiling)
            {
                var qs = StateMachine.GetState<FStateQuickstep>();
                qs.SetDirection(QuickstepDirection.Left).SetRun(Kinematics.Speed >= _quickstepConfig.minSpeed);
                StateMachine.SetState<FStateQuickstep>();
            }
            else if (Input.RightBumperPressed && !ceiling)
            {
                var qs = StateMachine.GetState<FStateQuickstep>();
                qs.SetDirection(QuickstepDirection.Right).SetRun(Kinematics.Speed >= _quickstepConfig.minSpeed);
                StateMachine.SetState<FStateQuickstep>();
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            if (Kinematics.CheckForGroundWithDirection(out RaycastHit data, Vector3.down))
            {
                Kinematics.Point = data.point;
                Kinematics.Normal = Vector3.up;
                Rigidbody.linearVelocity = Vector3.SmoothDamp(Rigidbody.linearVelocity, 
                    Vector3.zero, ref _squatVelocity, 0.067f);
                Kinematics.Snap(Kinematics.Point, Vector3.up);
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }
    }
}