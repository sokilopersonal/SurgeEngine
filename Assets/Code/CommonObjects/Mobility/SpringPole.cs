using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using UnityEngine;
using FMODUnity;

namespace SurgeEngine.Code.CommonObjects
{
    public class SpringPole : ContactBase
    {
        [SerializeField] float speed = 10f;
        [SerializeField] float keepVelocity = 0.1f;
        [SerializeField] Transform pole;
        [SerializeField] Transform point;
        [SerializeField] Animator animator;
        [SerializeField] EventReference soundEffect;

        private void Start()
        {
            pole.localEulerAngles = new Vector3(-90, 0, 0);
        }

        public override void Contact(Collider msg)
        {
            base.Contact(msg);

            Actor context = ActorContext.Context;

            float vertSpeed = Mathf.Abs(context.kinematics.Velocity.y);

            float pointDistance = Mathf.Clamp01(Vector3.Distance(new Vector3(context.transform.position.x, point.position.y, context.transform.position.z), point.position) * 0.5f);

            if (vertSpeed > 25f)
                animator.Play("Large", 0, 0);
            else if (vertSpeed < 25f && vertSpeed > 10f)
                animator.Play("Medium", 0, 0);
            else
                animator.Play("Small", 0, 0);

            RuntimeManager.PlayOneShot(soundEffect, transform.position);

            FStateSpecialJump specialJump = context.stateMachine.SetState<FStateSpecialJump>(ignoreInactiveDelay: true, allowSameState: true);
            specialJump.SetSpecialData(new SpecialJumpData(SpecialJumpType.Spring, transform, 0));
            specialJump.PlaySpecialAnimation(0);
            specialJump.SetKeepVelocity(keepVelocity);

            float finalSpeed = Mathf.Lerp(speed, speed * 0.5f, pointDistance);

            Common.ApplyImpulse(transform.up * finalSpeed);
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();
            Gizmos.color = Color.green;
            Gizmos.DrawLine(point.position, point.position + point.up * speed);
        }
    }
}