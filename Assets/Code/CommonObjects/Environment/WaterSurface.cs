using SurgeEngine.Code.Actor.States.SonicSpecific;
using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.CommonObjects.Interfaces;
using SurgeEngine.Code.Effects;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects.Environment
{
    public class WaterSurface : MonoBehaviour, IPlayerContactable
    {
        [SerializeField] private float minimumSpeed = 20f;
        
        private ActorBase _surfaceActor;
        private Rigidbody _surfaceRigidbody;
        private Collider _collider;
        private Transform _camera;
        private Vector3 _contactPoint;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
            
            _camera = Camera.main.transform;
            
            gameObject.layer = LayerMask.NameToLayer("WaterCollision");
        }

        private void FixedUpdate()
        {
            if (_surfaceRigidbody)
            {
                Vector3 velocity = _surfaceRigidbody.linearVelocity;
                float speed = velocity.magnitude;
                float delta = _surfaceRigidbody.transform.position.y - _contactPoint.y;

                bool isUnderwater = delta <= -1f;
                bool isRunning = speed > minimumSpeed;
                if (isRunning && isUnderwater)
                {
                    isRunning = false;
                }
                
                _collider.isTrigger = !isRunning;

                if (_surfaceActor.stateMachine.Has<FStateStomp>())
                {
                    if (_surfaceActor.stateMachine.IsExact<FStateStomp>())
                    {
                        _collider.isTrigger = true;
                    }
                }

                if (speed > 17f)
                {
                    Vector3 counterForce;
                    if (isUnderwater)
                    {
                        counterForce = new Vector3(velocity.normalized.x, Mathf.Clamp(velocity.normalized.y, float.NegativeInfinity, 0f), velocity.normalized.z) * (speed * 0.55f * Time.fixedDeltaTime);
                    }
                    else
                    {
                        counterForce = velocity.normalized * (speed * 0.75f * Time.fixedDeltaTime);
                    }

                    _surfaceRigidbody.linearVelocity -= counterForce;
                }
            }
            else
            {
                _collider.isTrigger = true;
            }
        }

        public void OnContact(Collider msg)
        {
            _surfaceActor = msg.transform.root.GetComponentInChildren<ActorBase>(); ;
            _surfaceRigidbody = _surfaceActor.kinematics.Rigidbody;
            _contactPoint = msg.ClosestPoint(_surfaceRigidbody.transform.position);

            Vector3 splashPoint = _contactPoint;
            splashPoint.y -= 0.75f;
            ParticlesContainer.Spawn("WaterSplash", splashPoint);
            
            _surfaceActor.flags.AddFlag(new Flag(FlagType.OnWater, null, false));
        }

        private void OnTriggerExit(Collider other)
        {
            if (_surfaceRigidbody)
            {
                if (_surfaceRigidbody.gameObject == other.transform.parent.gameObject)
                {
                    _surfaceActor.flags.RemoveFlag(FlagType.OnWater);
                    
                    _surfaceRigidbody = null;
                    _surfaceActor = null;
                }
            }
        }

        private void OnDrawGizmos()
        {
            if (_surfaceRigidbody)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(_contactPoint, 0.2f);
            }
        }
    }
}