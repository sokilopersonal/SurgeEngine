using SurgeEngine._Source.Code.Core.Character.States.BaseStates;
using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.States
{
    public class FStateJumpSelectorMissLand : FCharacterState
    {
        public FStateJumpSelectorMissLand(CharacterBase owner) : base(owner) { }

        public override void OnEnter()
        {
            Animation.StateAnimator.TransitionToState("JumpSelectorMissE", 0)
                .AfterThen(0.4f, () => StateMachine.SetState<FStateGround>());
            
            Kinematics.ResetVelocity();
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            if (Kinematics.CheckForGroundWithDirection(out var hit, Vector3.down))
            {
                Kinematics.Snap(hit.point, Vector3.up);
            }
            else
            {
                StateMachine.SetState<FStateAir>();
            }
        }
    }
}