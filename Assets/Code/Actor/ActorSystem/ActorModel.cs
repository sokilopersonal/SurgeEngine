using SurgeEngine.Code.Parameters;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorModel : MonoBehaviour, IActorComponent
    {
        public Actor actor { get; set; }

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
        
        public void OnInit()
        {
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
            
            Vector3 forward = Vector3.Slerp(root.forward, actor.transform.forward, 12 * Time.deltaTime);
            Vector3 up = Vector3.Slerp(root.up, actor.transform.up, 3 * Time.deltaTime);
            root.localRotation = Quaternion.LookRotation(forward, up);

            actor.effects.spinball.transform.SetParent(root, false);
        }

        public void RotateBody(Vector3 normal)
        {
            Vector3 vel = actor.rigidbody.linearVelocity;
            if (vel.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(vel, normal);
                actor.transform.rotation = targetRotation;
            }
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