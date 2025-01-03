using SurgeEngine.Code.ActorStates.BaseStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using UnityEngine;
using SurgeEngine.Code.ActorSoundEffects;

namespace SurgeEngine.Code.ActorStates
{
    public class FStateSwing : FStateMove
    {
        public Transform poleGrip;
        float rotation;

        bool swingSound = false;
        SwingSound soundReference;
        public FStateSwing(Actor owner, Rigidbody rigidbody) : base(owner, rigidbody)
        {
            soundReference = owner.sounds.GetComponent<SwingSound>();
        }

        public override void OnEnter()
        {
            base.OnEnter();
            rotation = 0f;
            Common.ResetVelocity(ResetVelocityType.Both);
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
            bool angleCorrect = rotation > 0.1f && rotation < 0.35f;

            StateMachine.SetState<FStateSwingJump>();
            StateMachine.GetState<FStateSwingJump>().Launch(angleCorrect);
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            rotation = Actor.animation.animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;

            if (rotation > 0.9f && !swingSound)
            {
                swingSound = true;
                soundReference.Swing();
            }
            else if (rotation < 0.9f && swingSound)
            {
                swingSound = false;
            }

            if(Input.JumpPressed)
                Jump();
            
            Actor.transform.position = poleGrip.position;
            Actor.transform.rotation = poleGrip.rotation;
        }
    }
}
