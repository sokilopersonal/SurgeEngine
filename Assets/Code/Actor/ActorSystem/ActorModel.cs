using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.ActorSystem
{
    public class ActorModel : ActorComponent
    {
        public Transform root;

        public CapsuleCollider collision;
        private float collisionStartHeight;
        private float collisionStartRadius;
        
        [SerializeField, Range(0, 1)] private float horizontalRotationSpeed = 0.85f;
        [SerializeField, Range(0, 1)] private float verticalRotationSpeed = 0.9f;
        
        private Vector3 _modelRotationVelocity;

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
            root.localPosition = actor.transform.localPosition;
            
            Vector3 forward = Vector3.Slerp(root.forward, actor.transform.forward, SurgeMath.Smooth(1 - horizontalRotationSpeed));
            Vector3 up = Vector3.Slerp(root.up, actor.transform.up, SurgeMath.Smooth(1 - verticalRotationSpeed));
            Vector3.OrthoNormalize(ref up, ref forward);
            root.localRotation = Quaternion.LookRotation(forward, up);
        }

        public void RotateBody(Vector3 normal)
        {
            Vector3 vel = actor.kinematics.Velocity;
            if (vel.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(vel, normal);
                actor.kinematics.Rigidbody.rotation = targetRotation;
            }
            else
            {
                Quaternion targetRotation = Quaternion.LookRotation(actor.transform.forward, normal);
                actor.kinematics.Rigidbody.rotation = targetRotation;
            }

            if (actor.stateMachine.IsExact<FStateIdle>())
            {
                Quaternion targetRotation = Quaternion.LookRotation(actor.transform.forward, normal);
                actor.kinematics.Rigidbody.rotation = targetRotation;
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