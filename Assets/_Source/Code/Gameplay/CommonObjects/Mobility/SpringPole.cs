using FMODUnity;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
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

        public override void Contact(Collider msg, ActorBase context)
        {
            float vertSpeed = Mathf.Abs(context.Kinematics.Velocity.y);
            float pointDistance = Mathf.Clamp01(Vector3.Distance(new Vector3(context.transform.position.x, point.position.y, context.transform.position.z), point.position) * 0.5f);

            if (vertSpeed > 25f)
                animator.Play("Large", 0, 0);
            else if (vertSpeed < 25f && vertSpeed > 10f)
                animator.Play("Medium", 0, 0);
            else
                animator.Play("Small", 0, 0);

            RuntimeManager.PlayOneShot(soundEffect, transform.position);

            context.StateMachine.GetState<FStateSpecialJump>().SetSpecialData(new SpecialJumpData(SpecialJumpType.Spring, transform, 0)).SetKeepVelocity(keepVelocity);
            context.StateMachine.SetState<FStateSpecialJump>(0f, true, true);

            float finalSpeed = Mathf.Lerp(speed, speed * 0.5f, pointDistance);

            context.Kinematics.Rigidbody.linearVelocity = transform.up * finalSpeed;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(point.position, point.position + point.up * speed);
        }
    }
}