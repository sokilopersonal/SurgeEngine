using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine.Source.Code.Core.Character.System
{
    public class CharacterModel : CharacterComponent
    {
        [SerializeField] private Transform root;
        public Transform Root => root;
        [SerializeField] private Transform modelTransform;

        [SerializeField] private CapsuleCollider collision;
        public CapsuleCollider Collision => collision;
        private float _collisionStartHeight;
        private float _collisionStartRadius;

        private CharacterBodyRotation _bodyRotation;

        private void Awake()
        {
            _collisionStartHeight = Collision.height;
            _collisionStartRadius = Collision.radius;
            
            _bodyRotation = new CharacterBodyRotation(Character);
        }

        private void Update()
        {
            
        }

        public void RotateBody(Vector3 normal)
        {
            _bodyRotation.RotateBody(normal);
        }

        public void RotateBody(Vector3 vector, Vector3 normal, float angleDelta = 1000f)
        {
            _bodyRotation.RotateBody(vector, normal, angleDelta);
        }

        public void VelocityRotation(Vector3 vel)
        {
            _bodyRotation.VelocityRotation(vel);
        }

        /// <summary>
        /// Sets the collision parameters for the actor
        /// </summary>
        public void SetCollisionParam(float height, float vertical, float radius = 0)
        {
            if (height != 0 || vertical != 0 || radius != 0)
            {
                Collision.height = height;
                Collision.radius = radius;
                Collision.center = new Vector3(0, vertical, 0);
            }
        }

        /// <summary>
        /// Sets the collision to lower level (sitting, crawling, etc.)
        /// </summary>
        public void SetLowerCollision()
        {
            Collision.height = 0.3f;
            Collision.radius = 0.15f;
            Collision.center = new Vector3(0, -0.5f, 0);
        }

        /// <summary>
        /// Sets the collision to default
        /// </summary>
        public void ResetCollisionToDefault()
        {
            Collision.height = _collisionStartHeight;
            Collision.radius = _collisionStartRadius;
            Collision.center = new Vector3(0, -0.25f, 0);
        }

        public void StartAirRestore(float time)
        {
            
        }

        public void StopAirRestore()
        {
            
        }
    }
}