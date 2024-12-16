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
        
        [SerializeField] private float horizontalRotationSpeed = 14f;
        [SerializeField] private float verticalRotationSpeed = 7.5f;
        [SerializeField] private float verticalRestorationRotationSpeed = 3f;
        
        private Vector3 _modelForwardRotationVelocity;
        private Vector3 _modelUpRotationVelocity;

        private Vector3 _tUp;
        private float _airRestoreTimer;
        private bool _airRestoring;
        
        private float _upRestoreTimer;
        private bool _upRestoring;

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
            root.localPosition = actor.transform.localPosition;
            
            var prev = actor.stateMachine.PreviousState;
            Vector3 forward = Vector3.Slerp(root.forward, actor.transform.forward, Time.deltaTime * horizontalRotationSpeed);
            Vector3 up = Vector3.Slerp(root.up, actor.transform.up, Time.deltaTime * verticalRotationSpeed);

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
                        up = Vector3.Slerp(root.up, actor.transform.up, Easings.Get(Easing.InCirc, _upRestoreTimer));
                        _upRestoreTimer += Time.deltaTime;
                        
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
                _upRestoreTimer = 0f;
                _upRestoring = false;
            }
            
            Vector3.OrthoNormalize(ref up, ref forward);
            root.localRotation = Quaternion.LookRotation(forward, up);
        }

        public void RotateBody(Vector3 normal, bool project = false)
        {
            Vector3 vel = actor.kinematics.Velocity;
            if (project) vel = Vector3.ProjectOnPlane(vel, normal);
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
        
        public void RotateBody(Vector3 vector, Vector3 normal, bool project = false)
        {
            if (_airRestoring) return;
            
            if (project) vector = Vector3.ProjectOnPlane(vector, normal);
            if (vector.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(vector, normal);
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
        
        public void VelocityRotation(bool transformRotation = false)
        {
            Vector3 vel = actor.kinematics.Velocity.normalized;
            float dot = Vector3.Dot(_tUp, Vector3.up);
            var left = Vector3.Cross(vel, Vector3.up);

            if (dot >= 0.99f)
            {
                if (!transformRotation) actor.kinematics.Rigidbody.rotation = Quaternion.FromToRotation(actor.transform.up, Vector3.up) * actor.kinematics.Rigidbody.rotation;
                else root.rotation = Quaternion.FromToRotation(actor.transform.up, Vector3.up) * root.rotation;
            }
            else
            {
                if (vel.sqrMagnitude > 0.1f)
                {
                    var forward = Vector3.Cross(vel, left); 
                    if (!transformRotation) actor.kinematics.Rigidbody.rotation = Quaternion.LookRotation(forward, vel);
                    else root.rotation = Quaternion.LookRotation(forward, vel);
                }
            }
            
            if (!transformRotation) root.rotation = actor.kinematics.Rigidbody.rotation;
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