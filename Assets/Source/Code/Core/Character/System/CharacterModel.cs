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

        private float _airRestoreTimer;
        private bool _airRestoring;
        
        private float _upRestoreTimer;
        private bool _upRestoring;
        private Vector3 _forwardVector;
        private Vector3 _upVector;
        
        private float _timer;

        private const float UpRestoreDuration = 5f;

        private CharacterBodyRotation _bodyRotation;

        private void Awake()
        {
            /*_collisionStartHeight = Collision.height;
            _collisionStartRadius = Collision.radius;
            
            Root.rotation = Character.transform.rotation;
            
            modelTransform.localPosition = new Vector3(0, verticalOffset, 0);*/
            
            _bodyRotation = new CharacterBodyRotation(Character);
        }

        private void Update()
        {
            /*Root.localPosition = Character.transform.localPosition;
            
            FState prev = Character.StateMachine.PreviousState;
            UpdateRotationVectors();

            if (prev is FStateObject)
            {
                HandleAirRotationRestore();
            }
            else
            {
                ResetRotationRestore();
            }

            if (_isFlipping)
            {
                HandleFlip();
            }

            Utility.TickTimer(ref _timer, AirRotationResetTime, false);
            
            ApplyFinalRotation();*/
        }

        private void HandleAirRotationRestore()
        {
            if (_airRestoring)
            {
                _bodyRotation.VelocityRotation(Character.Kinematics.Velocity.normalized);
                
                _airRestoreTimer -= Time.deltaTime;
                
                if (_airRestoreTimer <= 0)
                {
                    TransitionToUpRestore();
                }
            }
            else if (_upRestoring)
            {
                UpdateUpRestore();
            }
        }

        private void TransitionToUpRestore()
        {
            _airRestoreTimer = 0;
            _upRestoreTimer = 0f;
            _upRestoring = true;
            _airRestoring = false;
        }

        private void UpdateUpRestore()
        {
            float dt = Time.deltaTime;
            _upRestoreTimer += dt / UpRestoreDuration;
            _upVector = Vector3.Slerp(Root.up, Character.transform.up, _upRestoreTimer);
            
            if (_upRestoreTimer >= 1)
            {
                _upRestoreTimer = 0f;
                _upRestoring = false;
            }
        }

        private void ResetRotationRestore()
        {
            _airRestoreTimer = 0f;
            _airRestoring = false;
            _upRestoreTimer = 0f;
            _upRestoring = false;
        }

        private void ApplyFinalRotation()
        {
            Vector3.OrthoNormalize(ref _upVector, ref _forwardVector);
            Root.localRotation = Quaternion.LookRotation(_forwardVector, _upVector);
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
            _airRestoreTimer = time;
            _airRestoring = true;
        }

        public void StopAirRestore()
        {
            _airRestoreTimer = 0;
            _airRestoring = false;
        }
    }
}