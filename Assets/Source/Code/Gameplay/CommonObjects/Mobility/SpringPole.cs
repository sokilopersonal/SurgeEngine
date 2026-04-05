using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    public class SpringPole : StageObject
    {
        [SerializeField] private float speed = 10f;
        [SerializeField] private float keepVelocity = 0.1f;
        [SerializeField] private Transform pole;
        [SerializeField] private Transform point;
        [SerializeField] private Animator animator;
        [SerializeField] private EventReference soundEffect;

        private void Awake()
        {
            pole.localEulerAngles = new Vector3(90, 0, 0);
        }

        public override void OnEnter(Collider msg, CharacterBase context)
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

            context.StateMachine.GetState<FStateSpecialJump>().SetSpecialData(new SpecialJumpData(SpecialJumpType.Spring, transform, 0)).SetKeepVelocity(0);
            context.StateMachine.SetState<FStateSpecialJump>(true);

            float finalSpeed = Mathf.Lerp(speed, speed * 0.5f, pointDistance);
            context.Kinematics.Rigidbody.linearVelocity = transform.up * finalSpeed;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            
            Vector3 velocity = point.up * speed;
            Vector3 currentPos = point.position;
            float timeStep = 0.1f;
            int segments = 32;

            for (int i = 0; i < segments; i++)
            {
                float t = timeStep * i;
                Vector3 gravity = Physics.gravity * t;
                Vector3 nextPos = point.position + velocity * t + 0.5f * gravity * t;

                Gizmos.DrawLine(currentPos, nextPos);
                currentPos = nextPos;

                // Stop drawing if trajectory goes below starting point
                if (nextPos.y < point.position.y)
                    break;
            }
        }
    }
}