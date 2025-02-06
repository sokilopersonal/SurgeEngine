using SurgeEngine.Code.Enemy.AeroCannon.States;
using SurgeEngine.Code.StateMachine;
using SurgeEngine.Code.StateMachine.Components;
using UnityEngine;

namespace SurgeEngine.Code.Enemy.AeroCannon
{
    public class AeroCannonAnimation : StateAnimator
    {
        [SerializeField] ParticleSystem chargeParticle;
        [SerializeField] ParticleSystem fireParticle;
        protected override void AnimationTick()
        {
        }

        protected override void ChangeStateAnimation(FState obj)
        {
            base.ChangeStateAnimation(obj);

            if (obj is ACStateIdle)
            {
                TransitionToState("Idle");
            }

            if (obj is ACStatePrepare)
                chargeParticle.Play(true);
            else
                chargeParticle.Stop(true);

            if (obj is ACStateShoot)
            {
                TransitionToState("Shoot");
                fireParticle.Play(true);
            }
        }
    }
}