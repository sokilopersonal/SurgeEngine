using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Infrastructure.Custom.Extensions;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Environment
{
    public class WaterSurface : StageObject
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
        private CharacterBase _character;
        private Rigidbody _cRigidbody;
        private Vector3 _contactPoint;
        private bool _isUnderwater;
        private bool _isRunning;
        
        private EventReference _splashSound;
        private const string SplashEventPath = "event:/CommonObjects/WaterSplash";

        private void Awake()
        {
            GetComponent<Collider>();
            
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
                Vector3 velocity = new Vector3(_cRigidbody.linearVelocity.x, 0, _cRigidbody.linearVelocity.z);
                float speed = velocity.magnitude;
                float delta = _cRigidbody.transform.position.y - _contactPoint.y;

                _isUnderwater = delta <= -0.5f;
                _isRunning = speed > minimumSpeed;
                
                if (_isRunning && !_isUnderwater && _currentRunSplash)
                {
                    _currentRunSplash.Play(true);
                    Vector3 runSplashPosition = _cRigidbody.position;
                    runSplashPosition += _cRigidbody.transform.forward;
                    runSplashPosition -= _cRigidbody.transform.up * 0.75f;
                    Quaternion runSplashRotation = _cRigidbody.rotation;
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
                
                Vector3 counterForce;
                if (!_isUnderwater)
                {
                    counterForce = velocity.normalized * (speed * resistance * Time.fixedDeltaTime);
                }
                else
                {
                    counterForce = new Vector3(velocity.normalized.x, Mathf.Clamp(velocity.normalized.y, float.NegativeInfinity, 0f), velocity.normalized.z) * (speed * underwaterResistance * Time.fixedDeltaTime);
                }
                
                if (_character.StateMachine.CurrentState is IWaterMaintainSpeed
                    || _character.StateMachine.GetState(out FBoost boost) && boost.Active)
                {
                    counterForce = Vector3.zero;
                }
                
                _cRigidbody.linearVelocity -= counterForce;

                if (!_isUnderwater)
                {
                    if (!_character.Kinematics.CheckForGround(out _))
                    {
                        Detach(_character);
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
            if (_character == null)
            {
                _character = context;
                _cRigidbody = context.Rigidbody;
                _isInWater = true;
                _contactPoint = point;
            
                RuntimeManager.PlayOneShot(_splashSound, _contactPoint);

                Vector3 splashPoint = _contactPoint;
                splashPoint.y -= 0.75f;
                if (Mathf.Abs(_character.Kinematics.VerticalVelocity.y) > 8) SpawnSplash(splashPoint);

                DestroyRunSplash();
                _currentRunSplash = Instantiate(runSplash);

                _character.Flags.AddFlag(new Flag(FlagType.OnWater, false));
            }
        }

        public void Detach(CharacterBase character)
        {
            Vector3 splashPoint = character.Rigidbody.position;
            splashPoint.y -= 0.75f;
            if (Mathf.Abs(character.Kinematics.VerticalVelocity.y) > 8) SpawnSplash(splashPoint);
                    
            RuntimeManager.PlayOneShot(_splashSound, character.Rigidbody.position);
                    
            character.Flags.RemoveFlag(FlagType.OnWater);
                    
            _cRigidbody = null;
            _character = null;
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
            if (_cRigidbody)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(_contactPoint, 0.2f);
            }
        }
    }
    
    /// <summary>
    /// Interface for player states that maintain water speed.
    /// </summary>
    public interface IWaterMaintainSpeed { }
}