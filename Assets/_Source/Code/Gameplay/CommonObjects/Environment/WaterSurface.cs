using FMODUnity;
using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic;
using SurgeEngine._Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine._Source.Code.Core.Character.System;
using SurgeEngine._Source.Code.Core.Character.System.Characters.Sonic;
using SurgeEngine._Source.Code.Infrastructure.Custom.Extensions;
using UnityEngine;

namespace SurgeEngine._Source.Code.Gameplay.CommonObjects.Environment
{
    public class WaterSurface : ContactBase
    {
        [Header("Water")]
        [SerializeField] private float minimumSpeed = 20f;
        [SerializeField, Range(0, 1f)] private float resistance = 0.7f;
        [SerializeField, Range(0, 1f)] private float underwaterResistance = 0.45f;
        public float MinimumSpeed => minimumSpeed;

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

                if (_surfaceCharacter.StateMachine.Exists<FStateStomp>())
                {
                    if (_surfaceCharacter.StateMachine.IsExact<FStateStomp>())
                    {
                        _collider.isTrigger = true;
                    }
                }
                
                Vector3 counterForce;
                if (!_isUnderwater)
                {
                    counterForce = velocity.normalized * (speed * resistance * Time.fixedDeltaTime);
                }
                else
                {
                    counterForce = new Vector3(velocity.normalized.x, Mathf.Clamp(velocity.normalized.y, float.NegativeInfinity, 0f), velocity.normalized.z) * (speed * underwaterResistance * Time.fixedDeltaTime);
                }
                
                if (_surfaceCharacter.StateMachine.Exists<FStateDrift>() && _surfaceCharacter.StateMachine.IsExact<FStateDrift>() || _surfaceCharacter is Sonic && _surfaceCharacter.StateMachine.GetSubState<FBoost>().Active)
                {
                    counterForce = Vector3.zero;
                }
                
                _surfaceRigidbody.linearVelocity -= counterForce;

                if (!_isUnderwater)
                {
                    if (!_surfaceCharacter.Kinematics.CheckForGround(out _))
                    {
                        Detach(_surfaceCharacter);
                    }
                }
            }
            else
            {
                _isRunning = false;
                _isUnderwater = false;
            }
        }

        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);

            Attach(msg.ClosestPoint(context.transform.position), context);
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.TryGetCharacter(out CharacterBase character))
            {
                Detach(character);
            }
        }

        public void Attach(Vector3 point, CharacterBase context)
        {
            if (_surfaceCharacter == null)
            {
                _surfaceCharacter = context;
                _surfaceRigidbody = context.Rigidbody;
                _isInWater = true;
                _contactPoint = point;
            
                RuntimeManager.PlayOneShot(_splashSound, _contactPoint);

                Vector3 splashPoint = _contactPoint;
                splashPoint.y -= 0.75f;
                if (Mathf.Abs(_surfaceCharacter.Kinematics.VerticalVelocity.y) > 8) SpawnSplash(splashPoint);

                DestroyRunSplash();
                _currentRunSplash = Instantiate(runSplash);

                _surfaceCharacter.Flags.AddFlag(new Flag(FlagType.OnWater, false));
            }
        }

        public void Detach(CharacterBase character)
        {
            Vector3 splashPoint = character.Rigidbody.position;
            splashPoint.y -= 0.75f;
            if (Mathf.Abs(character.Kinematics.VerticalVelocity.y) > 8) SpawnSplash(splashPoint);
                    
            RuntimeManager.PlayOneShot(_splashSound, character.Rigidbody.position);
                    
            character.Flags.RemoveFlag(FlagType.OnWater);
                    
            _surfaceRigidbody = null;
            _surfaceCharacter = null;
            _isInWater = false;

            DestroyRunSplash();
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