using System;
using SurgeEngine.Code.Actor.States;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Actor.System
{
    public class ActorModel : ActorComponent
    {
        public Transform root;

        public CapsuleCollider collision;
        private float _collisionStartHeight;
        private float _collisionStartRadius;
        
        [SerializeField] private float horizontalRotationSpeed = 14f;
        [SerializeField] private float verticalRotationSpeed = 7.5f;
        [SerializeField] private float flipAngle = 360f;
        
        private Vector3 _modelForwardRotationVelocity;
        private Vector3 _modelUpRotationVelocity;

        private Vector3 _tUp;
        private float _airRestoreTimer;
        private bool _airRestoring;
        
        private float _upRestoreTimer;
        private bool _upRestoring;
        private Vector3 _forwardVector;
        private Vector3 _upVector;

        private bool _isFlipping;
        private float _flipTimer;

        private void Start()
        {
            _collisionStartHeight = collision.height;
            _collisionStartRadius = collision.radius;
            
            root.rotation = Actor.transform.rotation;
        }

        private void OnEnable()
        {
            Actor.stateMachine.OnStateAssign += OnStateAssign;
        }
        
        private void OnDisable()
        {
            Actor.stateMachine.OnStateAssign -= OnStateAssign;
        }

        private void Update()
        {
            root.localPosition = Actor.transform.localPosition;
            
            FState prev = Actor.stateMachine.PreviousState;
            _forwardVector = Vector3.Slerp(root.forward, Actor.transform.forward, Time.deltaTime * horizontalRotationSpeed);
            _upVector = Vector3.Slerp(root.up, Actor.transform.up, Time.deltaTime * verticalRotationSpeed
                * Mathf.Lerp(1f, 2f, Actor.kinematics.Speed / Actor.config.topSpeed));

            if (prev is FStateSpecialJump)
            {
                if (_airRestoring)
                {
                    VelocityRotation();
                    
                    _airRestoreTimer -= Time.deltaTime;
                    
                    if (_airRestoreTimer <= 0)
                    {
                        _airRestoreTimer = 0;
                        _upRestoreTimer = 0f;
                        _upRestoring = true;
                        _airRestoring = false;
                    }
                }
                else
                {
                    if (_upRestoring)
                    {
                        _upVector = Vector3.Slerp(root.up, Actor.transform.up, Easings.Get(Easing.InCirc, _upRestoreTimer));
                        _upRestoreTimer += Time.deltaTime / 0.8f;
                        
                        if (_upRestoreTimer >= 1)
                        {
                            _upRestoreTimer = 0f;
                            _upRestoring = false;
                        }
                    }
                }
            }
            else
            {
				_airRestoreTimer = 0f;
				_airRestoring = false;
                _upRestoreTimer = 0f;
                _upRestoring = false;
            }

            if (_isFlipping)
            {
                if (_airRestoring)
                {
                    _isFlipping = false;
                    _flipTimer = 0;
                }
                
                Flip();
                
                _flipTimer -= Time.deltaTime;
                if (_flipTimer <= 0)
                {
                    _isFlipping = false;
                    _flipTimer = 0;
                }
            }
            
            Vector3.OrthoNormalize(ref _upVector, ref _forwardVector);
            root.localRotation = Quaternion.LookRotation(_forwardVector, _upVector);
        }

        public void RotateBody(Vector3 normal)
        {
            Vector3 vel = Actor.kinematics.Velocity;
            if (vel.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(vel, normal);
                Actor.kinematics.Rigidbody.rotation = targetRotation;
            }
            else
            {
                Quaternion targetRotation = Quaternion.LookRotation(Actor.transform.forward, normal);
                Actor.kinematics.Rigidbody.rotation = targetRotation;
            }
        }

        public void RotateBody(Vector3 vector, Vector3 normal)
        {
            if (_airRestoring || _isFlipping) return;
            
            if (vector.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(vector, normal);
                Actor.kinematics.Rigidbody.rotation = targetRotation;
            }
            else
            {
                Quaternion targetRotation = Quaternion.LookRotation(Actor.transform.forward, normal);
                Actor.kinematics.Rigidbody.rotation = targetRotation;
            }
        }

        private void Flip()
        {
            Quaternion flipRotation = Quaternion.AngleAxis(flipAngle * Time.deltaTime, Vector3.left);
            Actor.kinematics.Rigidbody.rotation *= flipRotation;
        }

        public void VelocityRotation(bool transformRotation = false)
        {
            Vector3 vel = Actor.kinematics.Velocity.normalized;
            float dot = Vector3.Dot(_tUp, Vector3.up);
            Vector3 left = Vector3.Cross(vel, Vector3.up);

            if (dot >= 0.99f)
            {
                if (!transformRotation) Actor.kinematics.Rigidbody.rotation = Quaternion.FromToRotation(Actor.transform.up, Vector3.up) * Actor.kinematics.Rigidbody.rotation;
                else root.rotation = Quaternion.FromToRotation(Actor.transform.up, Vector3.up) * root.rotation;
            }
            else
            {
                if (vel.sqrMagnitude > 0.1f)
                {
                    Vector3 forward = Vector3.Cross(vel, left); 
                    if (!transformRotation) Actor.kinematics.Rigidbody.rotation = Quaternion.LookRotation(forward, vel);
                    else root.rotation = Quaternion.LookRotation(forward, vel);
                }
            }
            
            if (!transformRotation) root.rotation = Actor.kinematics.Rigidbody.rotation;
        }

        private void OnStateAssign(FState obj)
        {
            if (obj is FStateAir)
            {
                if (Actor.kinematics.Angle >= 90 && Actor.kinematics.Velocity.y > 3f)
                {
                    _isFlipping = true;
                    _flipTimer = 0.75f;
                }
            }
            else
            {
                _isFlipping = false;
                _flipTimer = 0;
            }
        }

        /// <summary>
        /// Sets the collision parameters for the actor
        /// </summary>
        /// <param name="height"></param>
        /// <param name="vertical"></param>
        /// <param name="radius"></param>
        public void SetCollisionParam(float height, float vertical, float radius = 0)
        {
            if (height != 0 || vertical != 0 || radius != 0)
            {
                collision.height = height;
                collision.radius = radius;
                collision.center = new Vector3(0, vertical, 0);
            }
        }

        /// <summary>
        /// Sets the collision to lower level (sitting, crawling, etc.)
        /// </summary>
        public void SetLowerCollision()
        {
            collision.height = 0.3f;
            collision.radius = 0.15f;
            collision.center = new Vector3(0, -0.5f, 0);
        }

        /// <summary>
        /// Sets the collision to default
        /// </summary>
        public void ResetCollisionToDefault()
        {
            collision.height = _collisionStartHeight;
            collision.radius = _collisionStartRadius;
            collision.center = new Vector3(0, -0.25f, 0);
        }
        
        public void SetRestoreUp(Vector3 tUp)
        {
            _tUp = tUp;
        }

        public void StartAirRestore(float time)
        {
            _airRestoreTimer = time;
            _airRestoring = true;
        }
    }
}