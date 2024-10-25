using System;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public class FStateSpecialJump : FStateMove
    {
        private float _jumpTimer;
        private SpecialJumpData _data;
        
        private float _keepVelocityTimer;

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
                    stateMachine.SetState<FStateGround>();
                }
            }
        }

        public override void OnExit()
        {
            base.OnExit();

            if (_data.type is SpecialJumpType.Spring or SpecialJumpType.DashRing)
            {
                animation.TransitionToState(AnimatorParams.AirCycle, 0.5f);
            }
        }

        private void SpecialTick(float dt)
        {
            switch (_data.type)
            {
                case SpecialJumpType.JumpBoard:
                    Common.ApplyGravity(stats.gravity, dt);
                    break;
                case SpecialJumpType.TrickJumper:
                    Common.ApplyGravity(stats.gravity, dt);
                    break;
                case SpecialJumpType.Spring:
                    if (_keepVelocityTimer > 0)
                    {
                        _keepVelocityTimer -= dt;
                        
                        if (_keepVelocityTimer <= 0)
                        {
                            stateMachine.SetState<FStateAir>();
                        }
                    }
                    break;
                case SpecialJumpType.DashRing:
                    if (_keepVelocityTimer > 0)
                    {
                        _keepVelocityTimer -= dt;
                        
                        if (_keepVelocityTimer <= 0)
                        {
                            stateMachine.SetState<FStateAir>();
                        }
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void SetSpecialData(SpecialJumpData data)
        {
            _data = data;
        }
        
        public void PlaySpecialAnimation(float time, object arg = null)
        {
            switch (_data.type)
            {
                case SpecialJumpType.JumpBoard:
                    animation.TransitionToState((bool)arg ? "Jump Delux" : "Jump Standard", time, true);
                    break;
                case SpecialJumpType.TrickJumper:
                    animation.TransitionToState("Jump Spring", time, true);
                    break;
                case SpecialJumpType.Spring:
                    animation.TransitionToState("Jump Spring", time, true);
                    break;
                case SpecialJumpType.DashRing:
                    animation.TransitionToState("Dash Ring", time, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(_data), _data, null);
            }
        }
        
        public void SetKeepVelocity(float time)
        {
            _keepVelocityTimer = Mathf.Max(0.2f, time - (time * 0.075f));
        }
    }

    public enum SpecialJumpType
    {
        JumpBoard,
        TrickJumper,
        Spring,
        DashRing,
    }
}