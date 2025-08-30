using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.Mobility;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States
{
    public class FStateJumpSelectorLaunch : FCharacterState
    {
        private float _timer;
        private bool _isFailed;
        
        public FStateJumpSelectorLaunch(CharacterBase owner) : base(owner)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0f;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (!_isFailed)
            {
                if (_timer > 0)
                {
                    _timer -= dt;
                }
                else
                {
                    StateMachine.SetState<FStateAir>();
                }
            }

            if (Kinematics.CheckForGroundWithDirection(out var hit, Vector3.down))
            {
                Kinematics.Normal = Vector3.up;

                if (!_isFailed)
                {
                    StateMachine.SetState<FStateGround>();
                }
                else
                {
                    StateMachine.SetState<FStateJumpSelectorMissLand>();
                }
            }
        }

        public override void OnFixedTick(float dt)
        {
            base.OnFixedTick(dt);
            
            Kinematics.ApplyGravity(Kinematics.Gravity);
        }

        public void SetData(float keep, JumpSelectorButton button, JumpSelectorResultType result)
        {
            _timer = keep;

            var anim = Animation.StateAnimator;
            if (result == JumpSelectorResultType.OK)
            {
                _isFailed = false;
                
                switch (button)
                {
                    case JumpSelectorButton.A:
                        anim.TransitionToState("JumpSelectorUS", 0)
                            .Then(() => anim.TransitionToState("JumpSelectorULoop")
                                .After(keep * 0.5f, () => anim.TransitionToState("JumpSelectorUE", 0)));
                        break;
                    case JumpSelectorButton.X:
                        anim.TransitionToState("JumpSelectorFS", 0)
                            .Then(() => anim.TransitionToState("JumpSelectorFLoop"));
                        break;
                    case JumpSelectorButton.B:
                        anim.TransitionToState("JumpSelectorDS", 0).Then(() => anim.TransitionToState("JumpSelectorDLoop"));
                        break;
                    case JumpSelectorButton.U:
                        break;
                }
            }
            else if (result == JumpSelectorResultType.Fall)
            {
                anim.TransitionToState("JumpSelectorMissS", 0)
                    .Then(() => anim.TransitionToState("JumpSelectorMissLoop"));

                _isFailed = true;
            }
        }
    }
}