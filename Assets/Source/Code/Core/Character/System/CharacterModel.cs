using SurgeEngine.Source.Code.Core.Character.States;
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

        private bool _isAirRestoring;
        private float _restoreTimer;
        private bool _isUpRestoring;
        private float _upRestoreTimer;

        private CharacterBodyRotation _bodyRotation;

        private void Awake()
        {
            _collisionStartHeight = Collision.height;
            _collisionStartRadius = Collision.radius;
            
            _bodyRotation = new CharacterBodyRotation(Character);
        }

        private void Update()
        {
            bool isObject = Character.StateMachine.PreviousState is FStateObject;

            if (isObject)
            {
                if (_isAirRestoring)
                {
                    _bodyRotation.VelocityRotation(Character.Kinematics.Velocity.normalized);
                    _restoreTimer -= Time.deltaTime;

                    if (_restoreTimer <= 0)
                    {
                        _restoreTimer = 0;
                        _isAirRestoring = false;
                        _isUpRestoring = true;
                        _upRestoreTimer = 0.4f;
                    }
                }
                else if (_isUpRestoring)
                {
                    bool isComplete = _bodyRotation.AlignToUpOverTime(Time.deltaTime, ref _upRestoreTimer);
                    if (isComplete)
                    {
                        _isUpRestoring = false;
                        _upRestoreTimer = 0;
                    }
                }
            }
            else
            {
                _isAirRestoring = false;
                _isUpRestoring = false;
                _restoreTimer = 0;
                _upRestoreTimer = 0;
            }
        }
        
        public void RotateBody(Vector3 normal)
        {
            _bodyRotation.RotateBody(normal);
        }

        public void RotateBody(Vector3 vector, Vector3 normal, float angleDelta = 1000f)
        {
            if (_isAirRestoring || _isUpRestoring)
                return;
            
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
            _isAirRestoring = true;
            _restoreTimer = time;
        }

        public void StopAirRestore()
        {
            _isAirRestoring = false;
            _isUpRestoring = false;
            _restoreTimer = 0;
            _upRestoreTimer = 0;
        }
    }
}