using FMODUnity;
using SurgeEngine.Code.Actor.States.SonicSpecific;
using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.Custom;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class ObjWaterSurface : ContactBase
    {
        [SerializeField] private ParticleSystem wave;
        
        private static float minSpeed = 22f;
        private static float gravityMultiplier = 0.325f;
        private static float resistance = 0.75f;
        private static float underwaterResistance = 0.55f;
        private static float waterResistSpeed = 16.5f;
        private static float underwaterThreshold = -0.3f;

        private Vector3 _initialPoint;
        private bool _isActorOnSurface;
        private bool _isRunningOnWater;
        private bool _isUnderwater;
        private Collider _collider;

        private Transform _camera;

        protected override void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
            
            _camera = Camera.main.transform;
        }

        protected override void Update()
        {
            if (_isActorOnSurface)
            {
                if (_collider.bounds.Contains(_camera.transform.position))
                {
                    RuntimeManager.StudioSystem.setParameterByName("Underwater", 1);
                }
                else
                {
                    RuntimeManager.StudioSystem.setParameterByName("Underwater", 0);
                }
            }
            else
            {
                RuntimeManager.StudioSystem.setParameterByName("Underwater", 0);
            }
        }

        protected override void FixedUpdate()
        {
            ActorBase actor = ActorContext.Context;
            if (_isActorOnSurface)
            {
                Rigidbody actorRigidbody = actor.kinematics.Rigidbody;
                Vector3 vel = actorRigidbody.linearVelocity;
                float currentSpeed = vel.magnitude;
                float horizontalSpeed = actorRigidbody.GetHorizontalMagnitude();
                
                _isRunningOnWater = horizontalSpeed > minSpeed;
                if (_isRunningOnWater && _isUnderwater)
                {
                    _isRunningOnWater = false;
                }
                
                _collider.isTrigger = !_isRunningOnWater;

                if (actor.stateMachine.CurrentState is FStateStomp)
                {
                    _collider.isTrigger = true; // Trigger collision for stomp so Sonic doesn't get hit when stomping
                }
                
                float diff = actor.transform.position.y - _initialPoint.y;
                _isUnderwater = diff <= underwaterThreshold;

                if (currentSpeed > waterResistSpeed)
                {
                    Vector3 counter;
                    if (_isUnderwater)
                    {
                        counter = new Vector3(vel.normalized.x, Mathf.Clamp(vel.normalized.y, float.NegativeInfinity, 0f), vel.normalized.z) * (currentSpeed * (_isUnderwater ? underwaterResistance : resistance) * Time.fixedDeltaTime);
                    }
                    else
                    {
                        counter = vel.normalized * (currentSpeed * (_isUnderwater ? underwaterResistance : resistance) * Time.fixedDeltaTime);
                    }

                    if (actor.stateMachine.CurrentState is not FStateDrift)
                    {
                        actorRigidbody.linearVelocity -= counter;
                    }
                }

                if (_isUnderwater)
                {
                    float underwaterGravity = actor.stats.startGravity * gravityMultiplier;
                    actor.stats.gravity = underwaterGravity;
                }
                else
                {
                    actor.stats.gravity = actor.stats.startGravity;
                }
            }
        }

        public override void Contact(Collider msg)
        {
            base.Contact(msg);
            
            _initialPoint = msg.ClosestPoint(transform.position);
            ParticleSystem wave = Instantiate(this.wave);
            wave.transform.position = _initialPoint;
            wave.Play();
            
            Destroy(wave.gameObject, wave.main.duration);
            
            _isActorOnSurface = true;
        }
        
        private void OnTriggerExit(Collider other)
        {
            ActorBase context = ActorContext.Context;
            
            if (context.gameObject == other.transform.parent.gameObject)
            {
                ParticleSystem wave = Instantiate(this.wave);
                wave.transform.position = other.ClosestPoint(transform.position);;
                wave.Play();
                
                context.flags.RemoveFlag(FlagType.OnWater);
                context.stats.gravity = context.stats.startGravity;
                _isActorOnSurface = false;
                _isUnderwater = false;
                _collider.isTrigger = true;
            }
        }
    }
}