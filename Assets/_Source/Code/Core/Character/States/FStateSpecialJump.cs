using System;
using SurgeEngine._Source.Code.Core.Character.States.BaseStates;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility;
using SurgeEngine._Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.States
{
    public class FStateSpecialJump : FCharacterState
    {
        private const float DefaultJumpTime = 0.2f;

        public SpecialJumpData SpecialJumpData { get; private set; }
        private float _jumpTimer;
        private float _keepVelocityTimer;

        public bool IsDelux { get; private set; }

        public FStateSpecialJump(CharacterBase owner) : base(owner)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _jumpTimer = DefaultJumpTime;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            Utility.TickTimer(ref _jumpTimer, DefaultJumpTime, false);
            UpdateKeepVelocityTimer(dt);
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            SpecialTick();

            if (Mathf.Approximately(_jumpTimer, 0))
            {
                if (Kinematics.CheckForGroundWithDirection(out _, Vector3.down))
                {
                    StateMachine.SetState<FStateGround>();
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();
            
            if (SpecialJumpData.type is SpecialJumpType.Spring or SpecialJumpType.DashRing)
            {
                Model.StartAirRestore(0.3f);
            }
        }

        private void SpecialTick()
        {
            switch (SpecialJumpData.type)
            {
                case SpecialJumpType.JumpBoard:
                case SpecialJumpType.TrickJumper:
                case SpecialJumpType.JumpSelector:
                    Kinematics.ApplyGravity(Kinematics.Gravity);
                    break;
                case SpecialJumpType.Spring:
                case SpecialJumpType.DashRing:
                    HandleSpringOrDashRing();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (SpecialJumpData.type != SpecialJumpType.TrickJumper)
            {
                if (_keepVelocityTimer <= 0)
                {
                    StateMachine.SetState<FStateAir>();
                }
            }
        }

        private void HandleSpringOrDashRing()
        {
            Model.SetRestoreUp(SpecialJumpData.transform.up);
            Model.VelocityRotation(character.Kinematics.Velocity.normalized);
        }

        private void UpdateKeepVelocityTimer(float dt)
        {
            if (_keepVelocityTimer > 0)
            {
                _keepVelocityTimer -= dt;
            }
        }

        public FStateSpecialJump SetSpecialData(SpecialJumpData data)
        {
            SpecialJumpData = data;
            return this;
        }

        public FStateSpecialJump SetKeepVelocity(float time)
        {
            _keepVelocityTimer = time;
            return this;
        }

        public FStateSpecialJump SetDelux(bool value)
        {
            IsDelux = value;
            return this;
        }
    }

    public enum SpecialJumpType
    {
        JumpBoard,
        TrickJumper,
        Spring,
        DashRing,
        JumpSelector
    }
}