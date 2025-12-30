using SurgeEngine.Source.Code.Core.Character.States.BaseStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.States
{
    public class FStateReactionPlateJump : FCharacterState
    {
        private ReactionPlateJumpInfo _info;
        private float _timer;
        private float _jumpDuration;

        private Vector3 _endPosition;

        public FStateReactionPlateJump(CharacterBase owner) : base(owner)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Kinematics.IsKinematic = true;
            Kinematics.ResetVelocity();
            
            _timer = 0;

            float distance = Vector3.Distance(_info.start, _endPosition);
            _jumpDuration = distance / _info.jumpMaxVelocity;
            
            var dir = (_endPosition - _info.start).normalized;
            Rigidbody.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(dir, Vector3.up));
        }

        public override void OnExit()
        {
            base.OnExit();

            Kinematics.IsKinematic = true;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            var target = _info.target;
            
            _timer += dt;
            if (_timer >= _jumpDuration)
            {
                Rigidbody.MovePosition(_endPosition);

                if (target.Type == ReactionPlateType.Panel)
                {
                    Rigidbody.linearVelocity = Vector3.zero;
                    StateMachine.SetState<FStateReactionPlate>();
                    target.PerformTrickContact(Character);
                
                    var nextTarget = target.Target;
                    if (nextTarget != null)
                    {
                        Rigidbody.rotation = Quaternion.LookRotation(-target.transform.up);
                    
                        var dirToNext = Vector3.ProjectOnPlane(
                            nextTarget.transform.position - target.transform.position,
                            Vector3.up
                        ).normalized;

                        var characterRight = Rigidbody.transform.right;
                        var dot = Vector3.Dot(dirToNext, characterRight);
            
                        var animator = Animation.StateAnimator;
                        if (dot < 0f)
                            animator.TransitionToState("ReactionPlate_R", 0.1f);
                        else if (dot > 0f)
                            animator.TransitionToState("ReactionPlate_L", 0.1f);
                    }
                    else
                    {
                        StateMachine.SetState<FStateAir>();
                    }
                }
                else if (target.Type == ReactionPlateType.End)
                {
                    StateMachine.SetState<FStateAir>();
                    Rigidbody.linearVelocity = Kinematics.Velocity;
                }
                
                return;
            }
    
            float t = _timer / _jumpDuration;

            Vector3 targetPosition = Vector3.Lerp(_info.start, _endPosition, t);

            float height = 4f;
            float heightOffset = Mathf.Sin(t * Mathf.PI) * height;

            float yOffset = Mathf.Lerp(0, _endPosition.y - _info.start.y, t);
    
            targetPosition.y = _info.start.y + yOffset + heightOffset;
            Rigidbody.MovePosition(targetPosition);
        }

        public void SetInfo(ReactionPlateJumpInfo info)
        {
            _info = info;
            
            var targetTransform = _info.target.transform;
            _endPosition = targetTransform.position + -targetTransform.forward - targetTransform.up * 0.2f;
        }
    }

    public readonly struct ReactionPlateJumpInfo
    {
        public readonly Vector3 start;
        public readonly ReactionPlate target;
        public readonly float jumpMaxVelocity;
        public readonly float jumpMinVelocity;

        public ReactionPlateJumpInfo(Vector3 start, ReactionPlate target, float jumpMaxVelocity, float jumpMinVelocity)
        {
            this.start = start;
            this.target  = target;
            
            if (jumpMaxVelocity == 0)
            {
                jumpMaxVelocity = 15;
            }
            this.jumpMaxVelocity = jumpMaxVelocity;
            this.jumpMinVelocity = jumpMinVelocity;
        }
    }
}