using SurgeEngine._Source.Code.Core.Character.Sound;
using SurgeEngine._Source.Code.Core.Character.States.BaseStates;
using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine._Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic
{
    public class FStateSwing : FCharacterState
    {
        public Transform poleGrip;
        private float _rotationAngle;

        private bool _swingSound;
        private readonly SwingSound _soundReference;
        
        public FStateSwing(CharacterBase owner) : base(owner)
        {
            _soundReference = owner.Sounds.GetComponent<SwingSound>();
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Kinematics.ResetVelocity();
            
            if (StateMachine.GetState(out FBoost boost))
                boost.Active = false;
        }

        private void Jump()
        {
            bool angleCorrect = _rotationAngle is > 0.1f and < 0.35f;
            StateMachine.SetState<FStateSwingJump>().Launch(angleCorrect);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            _rotationAngle = character.Animation.StateAnimator.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;

            if (_rotationAngle > 0.9f && !_swingSound)
            {
                _swingSound = true;
                _soundReference?.Swing();
            }
            else if (_rotationAngle < 0.9f && _swingSound)
            {
                _swingSound = false;
            }

            if(Input.APressed)
                Jump();
            
            character.transform.position = poleGrip.position;
            character.transform.rotation = poleGrip.rotation;
        }
    }
}
