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
                Animation.TransitionToState(AnimatorParams.AirCycle, 0.5f);
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
                    VelocityRotation();
                    CountTimer(dt);
                    break;
                case SpecialJumpType.DashRing:
                    VelocityRotation();
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
        private void VelocityRotation()
        {
            Debug.Log(Actor.kinematics);
            Vector3 vel = Actor.kinematics.Velocity.normalized;
            float dot = Vector3.Dot(_data.transform.up, Vector3.up);
            Vector3 upwards = dot > 0 ? Vector3.up : Vector3.down;
            var left = Vector3.Cross(vel, Vector3.down);

            if (dot >= 0.99f)
            {
                Actor.kinematics.Rigidbody.rotation = Quaternion.FromToRotation(Actor.transform.up, Vector3.up) * Actor.kinematics.Rigidbody.rotation;
            }
            else
            {
                if (vel.sqrMagnitude > 0.1f)
                    Actor.kinematics.Rigidbody.rotation = Quaternion.LookRotation(Quaternion.AngleAxis(90, left) * vel, upwards);
            }
            
            //Actor.model.root.localRotation = Actor.kinematics.Rigidbody.rotation;
        }
        private void CountTimer(float dt)
        {
            if (_keepVelocityTimer > 0)
            {
                _keepVelocityTimer -= dt;
            }
        }

        public FStateSpecialJump SetSpecialData(SpecialJumpData data)
        {
            _data = data;

            return this;
        }
        
        public FStateSpecialJump PlaySpecialAnimation(float time, object arg = null)
        {
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

            return this;
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