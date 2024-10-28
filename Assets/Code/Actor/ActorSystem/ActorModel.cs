using SurgeEngine.Code.Parameters;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorModel : ActorComponent
    {
        public Transform root;
        public CapsuleCollider collision;
        private float collisionStartHeight;
        private float collisionStartRadius;

        [SerializeField] private float groundSmoothness = 16f;
        [SerializeField] private float airSmoothness = 3f;
        [SerializeField] private float jumpSmoothness = 10f;

        // Added separate rotation speeds
        [SerializeField] private float horizontalRotationSpeed = 5f;
        [SerializeField] private float verticalRotationSpeed = 3f;

        private Vector3 forward;
        private Vector3 normal;

        private void Start()
        {
            collisionStartHeight = collision.height;
            collisionStartRadius = collision.radius;

            Quaternion parentRotation = actor.transform.parent.rotation;
            actor.transform.parent.rotation = Quaternion.identity;

            root.localPosition = actor.transform.localPosition;
            actor.transform.rotation = parentRotation;
            root.localRotation = parentRotation;
        }

        private void Update()
        {
            float speed = 0f;
            var state = actor.stateMachine.CurrentState;
            var prev = actor.stateMachine.PreviousState;
            if (prev is not FStateSpecialJump)
            {
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
            }
            else
            {
                speed = 2f;
            }

            root.localPosition = actor.transform.localPosition;
            root.localRotation = Quaternion.Slerp(root.localRotation,
                actor.transform.rotation,
                speed * Time.deltaTime);

            actor.effects.spinball.transform.SetParent(root, false);
        }

        public void RotateBody(Vector3 normal)
        {
            actor.stats.transformNormal = normal;
            Vector3 vel = actor.rigidbody.linearVelocity;

            if (vel.magnitude > 0.1f)
            {
                Quaternion rot = Quaternion.LookRotation(vel, actor.stats.transformNormal);
                actor.transform.rotation = rot;
            }
        }

        public void RotateModel(Vector3 forwardV, Vector3 normalV, float dt)
        {
            Quaternion targetRotation = actor.transform.rotation;
            Quaternion horizontalRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
            Quaternion verticalRotation = Quaternion.Euler(targetRotation.eulerAngles.x, 0, 0);
            
            root.rotation = Quaternion.Slerp(root.rotation,
                horizontalRotation,
                dt * horizontalRotationSpeed);
            
            root.rotation = Quaternion.Slerp(root.localRotation,
                verticalRotation,
                dt * verticalRotationSpeed);
        }

        /// <summary>
        /// Sets the collision parameters for the actor (Set height and vertical to 0 to reset to default)
        /// </summary>
        /// <param name="height"></param>
        /// <param name="vertical"></param>
        /// <param name="radius"></param>
        public void SetCollisionParam(float height, float vertical, float radius = 0)
        {
            if (height == 0)
            {
                collision.height = collisionStartHeight;
            }

            if (radius == 0)
            {
                collision.radius = collisionStartRadius;
            }

            if (vertical == 0)
            {
                collision.center = new Vector3(0, -0.25f, 0);
            }

            if (height != 0 || vertical != 0 || radius != 0)
            {
                collision.height = height;
                collision.radius = radius;
                collision.center = new Vector3(0, vertical, 0);
            }
        }
    }
}