using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public class FStateJumpSelectorLaunch : FCharacterState
    {
        private float _keepVelocityTime;
        private float _elapsedTime;
        private bool _isFailed;

        public FStateJumpSelectorLaunch(CharacterBase owner) : base(owner)
        {

        }

        public override void OnEnter()
        {
            base.OnEnter();

            _elapsedTime = 0f;

            Character.Flags.AddFlag(FlagType.OutOfControl);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

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
            
            if (!_isFailed)
            {
                _elapsedTime += dt;

                if (_elapsedTime >= _keepVelocityTime)
                {
                    if (_keepVelocityTime > 0) Character.Flags.AddFlag(new Flag(FlagType.OutOfControl, _keepVelocityTime));
                    StateMachine.SetState<FStateAir>();
                }
            }
            else
            {
                Character.Flags.AddFlag(new Flag(FlagType.OutOfControl, 0.5f));
                Kinematics.ApplyGravity(Kinematics.Gravity);
            }

            HurtBox.CreateSphereAttached(Character, Character.transform, Vector3.zero, 0.5f,
                HurtBoxTarget.Breakable | HurtBoxTarget.Enemy);
        }

        public void SetData(float keepVelocityTime, JumpSelectorButton button, JumpSelectorResultType result)
        {
            _keepVelocityTime = keepVelocityTime;

            var anim = Animation.StateAnimator;
            if (result == JumpSelectorResultType.OK)
            {
                _isFailed = false;

                switch (button)
                {
                    case JumpSelectorButton.A:
                        anim.TransitionToState("JumpSelectorUS", 0)
                            .Then(() => anim.TransitionToState("JumpSelectorULoop")
                                .After(keepVelocityTime * 0.5f, () => anim.TransitionToState("JumpSelectorUE", 0)));
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