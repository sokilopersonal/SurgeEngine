using System;
using FMODUnity;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom.Extensions;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Environment
{
    public class WaterSurface : ContactBase
    {
        [Header("Water")]
        [SerializeField] private float minimumSpeed = 20f;
        [SerializeField, Range(0, 1f)] private float resistance = 0.7f;
        [SerializeField, Range(0, 1f)] private float underwaterResistance = 0.45f;

        [Header("Particles")] 
        [SerializeField] private ParticleSystem splash;
        [SerializeField] private ParticleSystem runSplash;
        private ParticleSystem _currentRunSplash;

        private bool _isInWater;
        private CharacterBase _surfaceCharacter;
        private Rigidbody _surfaceRigidbody;
        private Collider _collider;
        private Vector3 _contactPoint;
        private bool _isUnderwater;
        private bool _isRunning;
        
        private EventReference _splashSound;
        private const string SplashEventPath = "event:/CommonObjects/WaterSplash";

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
            
            _splashSound = RuntimeManager.PathToEventReference(SplashEventPath);
            
            gameObject.layer = LayerMask.NameToLayer("WaterCollision");
            if (!gameObject.name.Contains("@Water"))
            {
                gameObject.name += "@Water"; // Add water tag
            }
        }

        private void FixedUpdate()
        {
            if (_isInWater)
            {
                Vector3 velocity = new Vector3(_surfaceRigidbody.linearVelocity.x, 0, _surfaceRigidbody.linearVelocity.z);
                float speed = velocity.magnitude;
                float delta = _surfaceRigidbody.transform.position.y - _contactPoint.y;

                _isUnderwater = delta <= -0.5f;
                _isRunning = speed > minimumSpeed;
                
                if (_isRunning && !_isUnderwater && _currentRunSplash)
                {
                    _currentRunSplash.Play(true);
                    Vector3 runSplashPosition = _surfaceRigidbody.position;
                    runSplashPosition += _surfaceRigidbody.transform.forward;
                    runSplashPosition -= _surfaceRigidbody.transform.up * 0.75f;
                    Quaternion runSplashRotation = _surfaceRigidbody.rotation;
                    runSplashRotation *= Quaternion.Euler(-90f, 0f, 0f);
                    _currentRunSplash.transform.SetPositionAndRotation(runSplashPosition, runSplashRotation);
                }
                else
                {
                    _currentRunSplash.Stop(true);
                }
                
                if (_isRunning && _isUnderwater)
                {
                    _isRunning = false;
                }
                
                _collider.isTrigger = !_isRunning;

                if (_surfaceCharacter.StateMachine.Exists<FStateStomp>())
                {
                    if (_surfaceCharacter.StateMachine.IsExact<FStateStomp>())
                    {
                        _collider.isTrigger = true;
                    }
                }

                if (speed > 17f)
                {
                    Vector3 counterForce;
                    if (_isUnderwater)
                    {
                        counterForce = new Vector3(velocity.normalized.x, Mathf.Clamp(velocity.normalized.y, float.NegativeInfinity, 0f), velocity.normalized.z) * (speed * underwaterResistance * Time.fixedDeltaTime);
                    }
                    else
                    {
                        counterForce = velocity.normalized * (speed * resistance * Time.fixedDeltaTime);
                    }

                    if (_surfaceCharacter.StateMachine.Exists<FStateDrift>())
                    {
                        if (_surfaceCharacter.StateMachine.IsExact<FStateDrift>())
                        {
                            counterForce = Vector3.zero;
                        }
                    }
                    
                    _surfaceRigidbody.linearVelocity -= counterForce;
                }
            }
            else
            {
                _isRunning = false;
                _isUnderwater = false;
                
                _collider.isTrigger = true;
            }
        }

        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);
            
            _surfaceCharacter = context;
            _surfaceRigidbody = context.Rigidbody;
            _isInWater = true;
            _contactPoint = msg.ClosestPoint(_surfaceRigidbody.transform.position);
            
            RuntimeManager.PlayOneShot(_splashSound, _contactPoint);

            Vector3 splashPoint = _contactPoint;
            splashPoint.y -= 0.75f;
            SpawnSplash(splashPoint);

            DestroyRunSplash();
            _currentRunSplash = Instantiate(runSplash);

            _surfaceCharacter.Flags.AddFlag(new Flag(FlagType.OnWater, false));
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetComponent(out CharacterBase character))
            {
                Vector3 splashPoint = character.Rigidbody.position;
                splashPoint.y -= 0.75f;
                SpawnSplash(splashPoint);
                    
                RuntimeManager.PlayOneShot(_splashSound, character.Rigidbody.position);
                    
                character.Flags.RemoveFlag(FlagType.OnWater);
                    
                _surfaceRigidbody = null;
                _surfaceCharacter = null;
                _isInWater = false;

                DestroyRunSplash();
            }
        }

        private void DestroyRunSplash()
        {
            if (_currentRunSplash)
            {
                _currentRunSplash.Stop(true);
                Destroy(_currentRunSplash.gameObject, 1f);
            }
        }

        private void SpawnSplash(Vector3 splashPoint)
        {
            var instance = Instantiate(splash, splashPoint, splash.transform.rotation);
            Destroy(instance.gameObject, 2f);
        }

        private void OnDestroy()
        {
            RuntimeManager.StudioSystem.setParameterByName("Underwater", 0);
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