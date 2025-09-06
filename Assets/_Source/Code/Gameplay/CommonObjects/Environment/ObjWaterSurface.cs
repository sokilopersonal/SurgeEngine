using FMODUnity;
using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Gameplay.Effects;
using SurgeEngine._Source.Code.Infrastructure.Custom.Extensions;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Environment
{
    public class ObjWaterSurface : StageObject
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

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
            
            _camera = Camera.main.transform;
        }

        private void Update()
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

        private void FixedUpdate()
        {
            CharacterBase character = CharacterContext.Context;
            if (_isActorOnSurface)
            {
                Rigidbody actorRigidbody = character.Kinematics.Rigidbody;
                Vector3 vel = actorRigidbody.linearVelocity;
                float currentSpeed = vel.magnitude;
                float horizontalSpeed = actorRigidbody.GetHorizontalMagnitude();
                
                _isRunningOnWater = horizontalSpeed > minSpeed;
                if (_isRunningOnWater && _isUnderwater)
                {
                    _isRunningOnWater = false;
                }
                
                _collider.isTrigger = !_isRunningOnWater;

                if (character.StateMachine.CurrentState is FStateStomp)
                {
                    _collider.isTrigger = true; // Trigger collision for stomp so Sonic doesn't get hit when stomping
                }
                
                float diff = character.transform.position.y - _initialPoint.y;
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

                    if (character.StateMachine.CurrentState is not FStateDrift)
                    {
                        actorRigidbody.linearVelocity -= counter;
                    }
                }

                if (_isUnderwater)
                {
                    float underwaterGravity = character.Kinematics.InitialGravity * gravityMultiplier;
                    character.Kinematics.Gravity = underwaterGravity;
                }
                else
                {
                    character.Kinematics.Gravity = character.Kinematics.InitialGravity;
                }
            }
        }

        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);
            
            _initialPoint = msg.ClosestPoint(transform.position);
            ParticlesContainer.Spawn("WaterSplash", _initialPoint);
            Debug.DrawRay(transform.position, Vector3.up * 5, Color.red, 10f);
            
            _isActorOnSurface = true;
        }
        
        private void OnTriggerExit(Collider other)
        {
            CharacterBase context = CharacterContext.Context;
            
            if (context.gameObject == other.transform.parent.gameObject)
            {
                ParticlesContainer.Spawn("WaterSplash", other.ClosestPoint(transform.position));
                
                context.Flags.RemoveFlag(FlagType.OnWater);
                context.Kinematics.Gravity = context.Kinematics.InitialGravity;
                _isActorOnSurface = false;
                _isUnderwater = false;
                _collider.isTrigger = true;
            }
        }
    }
}