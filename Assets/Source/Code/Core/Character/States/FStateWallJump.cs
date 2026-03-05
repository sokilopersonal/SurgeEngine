using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public class FStateWallJump : FCharacterState, IWallJumpDetect
    {
        public bool WallDetected { get; set; }
        
        private float _time;
        
        public FStateWallJump(CharacterBase owner) : base(owner)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _time = 0;

            var cfg = Character.Config;
            var direction = -Rigidbody.transform.right; // wth Sonic Team??
            Rigidbody.linearVelocity = direction * cfg.WallJumpForce + Vector3.up * cfg.WallJumpHeightForce;
            Rigidbody.rotation = Quaternion.LookRotation(direction);
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            _time += dt / 0.7f;
            if (_time >= 1)
            {
                StateMachine.SetState<FStateAir>();
                return;
            }
            
            Kinematics.ApplyGravity(Kinematics.Gravity);
            if (Kinematics.CheckForGroundWithDirection(out var hit, Vector3.down))
            {
                StateMachine.SetState<FStateGround>();
            }
        }
    }
}