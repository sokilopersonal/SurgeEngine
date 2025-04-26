using SurgeEngine.Code.Actor.Sound;
using SurgeEngine.Code.Actor.States.BaseStates;
using SurgeEngine.Code.Actor.States.SonicSubStates;
using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.Actor.States.SonicSpecific
{
    public class FStateSwing : FStateMove
    {
        public Transform poleGrip;
        private float _rotationAngle;

        private bool _swingSound;
        private readonly SwingSound soundReference;
        
        public FStateSwing(ActorBase owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            soundReference = owner.sounds.GetComponent<SwingSound>();
        }

        public override void OnEnter()
        {
            base.OnEnter();

            Common.ResetVelocity(ResetVelocityType.Both);
            StateMachine.GetSubState<FBoost>().Active = false;

            Actor.effects.swingTrail.trail.Clear();
            Actor.effects.swingTrail.Toggle(true);
        }

        public override void OnExit()
        {
            base.OnExit();
            Actor.effects.swingTrail.Toggle(false);
        }

        void Jump()
        {
            bool angleCorrect = _rotationAngle > 0.1f && _rotationAngle < 0.35f;

            StateMachine.SetState<FStateSwingJump>();
            StateMachine.GetState<FStateSwingJump>().Launch(angleCorrect);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            _rotationAngle = Actor.animation.StateAnimator.animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;

            if (_rotationAngle > 0.9f && !_swingSound)
            {
                _swingSound = true;
                soundReference.Swing();
            }
            else if (_rotationAngle < 0.9f && _swingSound)
            {
                _swingSound = false;
            }

            if(Input.JumpPressed)
                Jump();
            
            Actor.transform.position = poleGrip.position;
            Actor.transform.rotation = poleGrip.rotation;
        }
    }
}
