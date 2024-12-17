using System;
using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateSpecialJump : FStateMove
    {
        private float _jumpTimer;
        private SpecialJumpData _data;
        
        private float _keepVelocityTimer;

        public FStateSpecialJump(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
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
            
            if (_jumpTimer > 0)
            {
                _jumpTimer -= dt;
            }
            else
            {
                _jumpTimer = 0;
            }
            
            SpecialTick(dt);
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);

            if (Mathf.Approximately(_jumpTimer, 0))
            {
                if (Common.CheckForGround(out var hit))
                {
                    StateMachine.SetState<FStateGround>();
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (_data.type is SpecialJumpType.Spring or SpecialJumpType.DashRing)
            {
                Actor.model.StartAirRestore(0.65f);
            }
        }

        private void SpecialTick(float dt)
        {
            switch (_data.type)
            {
                case SpecialJumpType.JumpBoard:
                    Common.ApplyGravity(Stats.gravity, dt);
                    break;
                case SpecialJumpType.TrickJumper:
                    Common.ApplyGravity(Stats.gravity, dt);
                    break;
                case SpecialJumpType.Spring:
                    Actor.model.SetRestoreUp(_data.transform.up);
                    Actor.model.VelocityRotation();
                    CountTimer(dt);
                    break;
                case SpecialJumpType.DashRing:
                    Actor.model.SetRestoreUp(_data.transform.up);
                    Actor.model.VelocityRotation();
                    CountTimer(dt);
                    break;
                case SpecialJumpType.JumpSelector:
                    Common.ApplyGravity(Stats.gravity, dt);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            if (_keepVelocityTimer < 0)
            {
                StateMachine.SetState<FStateAir>();
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
            _data = data;
        }
        
        public void PlaySpecialAnimation(float time, object arg = null)
        {
            Animation.ResetCurrentAnimationState();
            
            switch (_data.type)
            {
                case SpecialJumpType.JumpBoard:
                    Animation.TransitionToState((bool)arg ? "Jump Delux" : "Jump Standard", time, true);
                    break;
                case SpecialJumpType.TrickJumper:
                    Animation.TransitionToState("Jump Spring", time, true);
                    break;
                case SpecialJumpType.Spring:
                    Animation.TransitionToState("Jump Spring", time, true);
                    break;
                case SpecialJumpType.DashRing:
                    Animation.TransitionToState("Dash Ring", time, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_data), _data, null);
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