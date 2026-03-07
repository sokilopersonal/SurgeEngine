using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public class FStateWall : FCharacterState, ISkip2D
    {
        public FStateWall(CharacterBase owner) : base(owner)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            Kinematics.ResetVelocity();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            if (Input.APressed)
            {
                StateMachine.SetState<FStateWallJump>();
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            if (Kinematics.CheckForGroundWithDirection(out _, Vector3.down))
            {
                StateMachine.SetState<FStateGround>();
                return;
            }
            
            if (!Kinematics.CheckForGroundWithDirection(out _, Transform.right))
            {
                StateMachine.SetState<FStateAir>();
                return;
            }
            
            Kinematics.ApplyGravity(Kinematics.Gravity / 12f);
        }
    }

    public interface IWallJumpDetect
    {
        bool WallDetected { get; set; }
    }
}