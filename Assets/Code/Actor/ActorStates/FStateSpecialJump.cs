using System;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Parameters
{
    public class FStateSpecialJump : FStateMove
    {
        private float _jumpTimer;

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
            
            Common.ApplyGravity(35f, dt);
        }

        public override void OnExit()
        {
            base.OnExit();
            
            animation.TransitionToState("Run Cycle", 0f);
        }

        public void PlaySpecialAnimation(SpecialAnimationType type, float time)
        {
            switch (type)
            {
                case SpecialAnimationType.JumpBoard:
                    animation.TransitionToState("Jump Delux", time, true);
                    break;
                case SpecialAnimationType.TrickJumper:
                    animation.TransitionToState("Jump Spring", time, true);
                    break;
                case SpecialAnimationType.Spring:
                    animation.TransitionToState("Spring", time, true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }
    }

    public enum SpecialAnimationType
    {
        JumpBoard,
        TrickJumper,
        Spring
    }
}