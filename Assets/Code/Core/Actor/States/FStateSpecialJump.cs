using System;
using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.Mobility;
using SurgeEngine.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateSpecialJump : FStateMove
    {
        public SpecialJumpData data;
        private float _jumpTimer;
        
        private float _keepVelocityTimer;

        public FStateSpecialJump(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _jumpTimer = 0.25f;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            Common.TickTimer(ref _jumpTimer, 0.25f, false);
            
            CountTimer(dt);
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            SpecialTick(dt);
            
            if (Mathf.Approximately(_jumpTimer, 0))
            {
                if (Common.CheckForGround(out _, CheckGroundType.Predict))
                {
                    StateMachine.SetState<FStateGround>();
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (data.type is SpecialJumpType.Spring or SpecialJumpType.DashRing)
            {
                Model.StartAirRestore(0.4f);
            }
        }

        private void SpecialTick(float dt)
        {
            switch (data.type)
            {
                case SpecialJumpType.JumpBoard:
                    Common.ApplyGravity(Stats.gravity, dt);
                    break;
                case SpecialJumpType.TrickJumper:
                    Common.ApplyGravity(Stats.gravity, dt);
                    break;
                case SpecialJumpType.Spring:
                    Model.SetRestoreUp(data.transform.up);
                    Model.VelocityRotation();
                    break;
                case SpecialJumpType.DashRing:
                    Model.SetRestoreUp(data.transform.up);
                    Model.VelocityRotation();
                    break;
                case SpecialJumpType.JumpSelector:
                    Common.ApplyGravity(Stats.gravity, dt);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (data.type == SpecialJumpType.Spring || data.type == SpecialJumpType.DashRing)
            {
                if (_keepVelocityTimer < 0)
                {
                    StateMachine.SetState<FStateAir>();
                }
            }
        }
        
        private void CountTimer(float dt)
        {
            if (_keepVelocityTimer > 0)
            {
                _keepVelocityTimer -= dt;
            }
        }

        public void SetSpecialData(SpecialJumpData data)
        {
            this.data = data;
        }
        
        public void PlaySpecialAnimation(float time, object arg = null)
        {
            Animation.StateAnimator.ResetCurrentAnimationState();
            
            switch (data.type)
            {
                case SpecialJumpType.JumpBoard:
                    Animation.StateAnimator.TransitionToState((bool)arg ? "Jump Delux" : "Jump Standard", time);
                    break;
                case SpecialJumpType.TrickJumper:
                    Animation.StateAnimator.TransitionToState("Jump Spring", time);
                    break;
                case SpecialJumpType.Spring:
                    Animation.StateAnimator.TransitionToState("Jump Spring", time);
                    break;
                case SpecialJumpType.DashRing:
                    Animation.StateAnimator.TransitionToState("Dash Ring", time);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(data), data, null);
            }
        }
        
        public void SetKeepVelocity(float time)
        {
            _keepVelocityTimer = time;
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