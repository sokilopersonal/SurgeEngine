using SurgeEngine.Code.Core.Actor.Sound;
using SurgeEngine.Code.Core.Actor.States.BaseStates;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Core.Actor.States.Characters.Sonic
{
    public class FStateSwing : FCharacterState
    {
        public Transform poleGrip;
        private float _rotationAngle;

        private bool _swingSound;
        private readonly SwingSound soundReference;
        
        public FStateSwing(CharacterBase owner) : base(owner)
        {
            soundReference = owner.Sounds.GetComponent<SwingSound>();
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Kinematics.ResetVelocity();
            StateMachine.GetSubState<FBoost>().Active = false;
        }

        public override void OnExit()
        {
            base.OnExit();
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
                soundReference.Swing();
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
