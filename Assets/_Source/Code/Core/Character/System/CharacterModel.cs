using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Core.StateMachine.Base;
using SurgeEngine._Source.Code.Infrastructure.Custom;
using UnityEngine;

namespace SurgeEngine._Source.Code.Core.Character.System
{
    public class CharacterModel : CharacterComponent
    {
        public Transform root;
        [SerializeField] private Transform modelTransform;
        [SerializeField] private float verticalOffset = -1f;

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
        private float _timer;
        
        public const float AirRotationResetTime = 2f;

        private void Awake()
        {
            _collisionStartHeight = collision.height;
            _collisionStartRadius = collision.radius;
            
            root.rotation = character.transform.rotation;
            
            modelTransform.localPosition = new Vector3(0, verticalOffset, 0);
        }

        private void OnEnable()
        {
            character.StateMachine.OnStateAssign += OnStateAssign;
        }
        
        private void OnDisable()
        {
            character.StateMachine.OnStateAssign -= OnStateAssign;
        }

        private void Update()
        {
            root.localPosition = character.transform.localPosition;
            
            FState prev = character.StateMachine.PreviousState;
            _forwardVector = Vector3.Slerp(root.forward, character.transform.forward, Time.deltaTime * horizontalRotationSpeed);
            _upVector = Vector3.Slerp(root.up, character.transform.up, Time.deltaTime * verticalRotationSpeed
                * Mathf.Lerp(1f, 2f, character.Kinematics.Speed / character.Config.topSpeed));

            if (prev is FStateObject)
            {
                if (_airRestoring)
                {
                    VelocityRotation(character.Kinematics.Velocity.normalized);
                    
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
                        float dt = Time.deltaTime;
                        _upRestoreTimer += dt / 5f;
                        _upVector = Vector3.Slerp(root.up, character.transform.up, _upRestoreTimer);
                        
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

            Utility.TickTimer(ref _timer, AirRotationResetTime, false);
            
            Vector3.OrthoNormalize(ref _upVector, ref _forwardVector);
            root.localRotation = Quaternion.LookRotation(_forwardVector, _upVector);
        }
        
        public void RotateBody(Vector3 normal)
        {
            Vector3 vel = character.Kinematics.Velocity;
            if (vel.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(vel, normal);
                character.Kinematics.Rigidbody.MoveRotation(targetRotation);
            }
        }

        public void RotateBody(Vector3 vector, Vector3 normal, float angleDelta = 1200f)
        {
            if (_airRestoring || _isFlipping) return;
            
            var kinematics = character.Kinematics;
            var rb = kinematics.Rigidbody;
            Vector3 inputDir = kinematics.GetInputDir();
            var config = character.Config;
            Vector3 currentVelocity = Vector3.ProjectOnPlane(vector, normal);
            float currentSpeed = currentVelocity.magnitude;
            float speedThreshold = 3.5f;

            if (inputDir.sqrMagnitude > 0.02f)
            {
                Vector3 targetDir = Vector3.ProjectOnPlane(inputDir.normalized, Vector3.up);

                if (currentSpeed > speedThreshold)
                {
                    var velDir = Vector3.ProjectOnPlane(currentVelocity.normalized, normal);
                    float t = Mathf.Clamp01((currentSpeed - speedThreshold) / (config.topSpeed - speedThreshold));
                    t = Mathf.Sqrt(t);
                    targetDir = Vector3.Slerp(inputDir.normalized, velDir, t * 10f).normalized;
                }

                float rotSpeed = angleDelta * (currentSpeed > speedThreshold ? Mathf.Lerp(1f, 0.15f, Mathf.Pow((currentSpeed - speedThreshold) / (config.topSpeed - speedThreshold), 0.5f)) : 1f);

                if (targetDir != Vector3.zero)
                {
                    var targetRot = Quaternion.LookRotation(targetDir, normal);
                    var towards = Quaternion.RotateTowards(rb.rotation, targetRot, rotSpeed * Time.fixedDeltaTime);
                    rb.MoveRotation(towards);
                }
            }
            else
            {
                if (currentSpeed > 0.1f)
                {
                    Vector3 velocityDir = currentVelocity.normalized;
                    Quaternion targetRotation = Quaternion.LookRotation(velocityDir, normal);
                    var towards = Quaternion.RotateTowards(rb.rotation, targetRotation, (128f + currentSpeed / 2) * Time.fixedDeltaTime);
                    rb.MoveRotation(towards);
                }
            }
            
            Quaternion upRotation = Quaternion.FromToRotation(rb.transform.up, normal) * rb.rotation;
            rb.MoveRotation(upRotation);
        }

        private void Flip()
        {
            var rb = character.Kinematics.Rigidbody;
            Quaternion flipRotation = Quaternion.AngleAxis(flipAngle * Time.deltaTime, Vector3.left);
            rb.MoveRotation(rb.rotation * flipRotation);
        }

        public void VelocityRotation(Vector3 vel)
        {
            float dot = Vector3.Dot(vel, Vector3.up);
            Vector3 left = Vector3.Cross(vel, Vector3.up);

            if (dot >= 0.99f)
            {
                character.Kinematics.Rigidbody.MoveRotation(Quaternion.FromToRotation(character.transform.up, Vector3.up) * character.Kinematics.Rigidbody.rotation);
            }
            else
            {
                if (vel.sqrMagnitude > 0.1f)
                {
                    Vector3 forward = Vector3.Cross(vel, left);
                    character.Kinematics.Rigidbody.MoveRotation(Quaternion.LookRotation(forward, vel));
                }
            }
            
            root.rotation = character.Kinematics.Rigidbody.rotation;
        }

        private void OnStateAssign(FState obj)
        {
            if (obj is FStateAir)
            {
                if (Mathf.Abs(character.Kinematics.Angle - 90) < 0.05f && character.Kinematics.Velocity.y > 3f)
                {
                    _isFlipping = true;
                    _flipTimer = 0.75f;
                    _timer = AirRotationResetTime;
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
            _tUp = character.Kinematics.Velocity.normalized;
        }

        public void StopAirRestore()
        {
            _airRestoreTimer = 0;
            _airRestoring = false;
        }
    }
}