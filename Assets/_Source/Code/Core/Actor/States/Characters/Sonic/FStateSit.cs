using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic
{
    public class FStateSit : FCharacterState, IDamageableState
    {
        private QuickStepConfig _quickstepConfig;
        
        public FStateSit(CharacterBase owner) : base(owner)
        {
            owner.TryGetConfig(out _quickstepConfig);
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Kinematics.ResetVelocity();
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
                StateMachine.SetState<FStateJump>();
            }

            if (Input.moveVector.magnitude >= 0.2f)
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
                
                Kinematics.Snap(Kinematics.Point, Vector3.up, true);
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }
    }
}