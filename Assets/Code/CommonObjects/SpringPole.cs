using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.SurgeDebug;
using UnityEngine;
using FMODUnity;

namespace SurgeEngine.Code.CommonObjects
{
    public class SpringPole : Spring
    {
        [SerializeField] Transform pole;
        [SerializeField] Animator animator;
        [SerializeField] EventReference soundEffect;

        private void Start()
        {
            pole.localEulerAngles = new Vector3(-90, 0, 0);
        }

        public override void Contact(Collider msg)
        {
            Actor context = ActorContext.Context;

            float vertSpeed = Mathf.Abs(context.kinematics.Velocity.y);

            if (vertSpeed > 25f)
                animator.Play("Large", 0, 0);
            else if (vertSpeed < 25f && vertSpeed > 10f)
                animator.Play("Medium", 0, 0);
            else
                animator.Play("Small", 0, 0);

            RuntimeManager.PlayOneShot(soundEffect, transform.position);

            base.Contact(msg);
        }
    }
}