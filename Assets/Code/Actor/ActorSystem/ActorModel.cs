using SurgeEngine.Code.Parameters;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorModel : ActorComponent
    {
        public Transform root;

        [SerializeField] private float groundSmoothness = 16f;
        [SerializeField] private float airSmoothness = 3f;
        [SerializeField] private float jumpSmoothness = 10f;

        private Vector3 forward;
        private Vector3 normal;

        private void Update()
        {
            float speed = 0;
            var state = actor.stateMachine.CurrentState;
            if (state is FStateJump)
            {
                speed = jumpSmoothness;
            }
            else if (actor.stats.isInAir)
            {
                speed = airSmoothness;
            }
            else
            {
                speed = groundSmoothness;
            }
            
            root.localPosition = actor.transform.localPosition;
            root.localRotation = Quaternion.Slerp(root.localRotation,
                Quaternion.LookRotation(actor.transform.forward, actor.transform.up),
                speed * Time.deltaTime);
            
            actor.effects.spinball.transform.SetParent(root, false);
        }

        public void RotateBody(Vector3 normal)
        {
            actor.stats.transformNormal = normal;

            Vector3 vel = actor.rigidbody.linearVelocity;
            vel = Vector3.ProjectOnPlane(vel, normal);

            if (vel.magnitude > 0.1f)
            {
                Quaternion rot = Quaternion.LookRotation(vel, actor.stats.transformNormal);
                actor.transform.rotation = rot;
            }
        }

        public void RotateModel(Vector3 forwardV, Vector3 normalV, float dt, float speed)
        {
            root.localRotation = Quaternion.Slerp(root.localRotation,
                Quaternion.LookRotation(forwardV, normalV),
                dt * speed);
        }
    }
}