using FMODUnity;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.Interfaces;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Environment
{
    public class WaterSurface : MonoBehaviour, IPlayerContactable
    {
        [Header("Water")]
        [SerializeField] private float minimumSpeed = 20f;
        [SerializeField, Range(0, 1f)] private float resistance = 0.7f;
        [SerializeField, Range(0, 1f)] private float underwaterResistance = 0.45f;

        [Header("Particles")] 
        [SerializeField] private ParticleSystem splash;
        [SerializeField] private ParticleSystem runSplash;
        private ParticleSystem _currentRunSplash;
        
        private ActorBase _surfaceActor;
        private Rigidbody _surfaceRigidbody;
        private Collider _collider;
        private Transform _camera;
        private Vector3 _contactPoint;
        private bool _isUnderwater;
        private bool _isRunning;
        
        private EventReference _splashSound;
        private const string SplashEventPath = "event:/CommonObjects/WaterSplash";

        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
            _camera = Camera.main.transform;
            
            _splashSound = RuntimeManager.PathToEventReference(SplashEventPath);
            
            gameObject.layer = LayerMask.NameToLayer("WaterCollision");
            if (!gameObject.name.Contains("@Water"))
            {
                gameObject.name += "@Water"; // Add water tag
            }
        }

        private void Update()
        {
            if (_surfaceRigidbody)
            {
                if (_isUnderwater)
                {
                    RuntimeManager.StudioSystem.setParameterByName("Underwater", 1);
                }
                else
                {
                    RuntimeManager.StudioSystem.setParameterByName("Underwater", 0);
                }
            }
        }

        private void FixedUpdate()
        {
            if (_surfaceRigidbody)
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

                if (_surfaceActor.stateMachine.Exists<FStateStomp>())
                {
                    if (_surfaceActor.stateMachine.IsExact<FStateStomp>())
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

                    if (_surfaceActor.stateMachine.Exists<FStateDrift>())
                    {
                        if (_surfaceActor.stateMachine.IsExact<FStateDrift>())
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

        public void OnContact(Collider msg)
        {
            _surfaceActor = msg.transform.root.GetComponentInChildren<ActorBase>(); ;
            _surfaceRigidbody = _surfaceActor.Kinematics.Rigidbody;
            _contactPoint = msg.ClosestPoint(_surfaceRigidbody.transform.position);
            
            RuntimeManager.PlayOneShot(_splashSound, _contactPoint);

            Vector3 splashPoint = _contactPoint;
            splashPoint.y -= 0.75f;
            SpawnSplash(splashPoint);

            DestroyRunSplash();
            _currentRunSplash = Instantiate(runSplash);

            _surfaceActor.Flags.AddFlag(new Flag(FlagType.OnWater, null, false));
        }

        private void OnTriggerExit(Collider other)
        {
            if (_surfaceRigidbody)
            {
                if (_surfaceRigidbody.gameObject == other.transform.parent.gameObject)
                {
                    Vector3 splashPoint = _surfaceRigidbody.position;
                    splashPoint.y -= 0.75f;
                    SpawnSplash(splashPoint);
                    
                    RuntimeManager.PlayOneShot(_splashSound, _surfaceRigidbody.position);
                    
                    _surfaceActor.Flags.RemoveFlag(FlagType.OnWater);
                    
                    _surfaceRigidbody = null;
                    _surfaceActor = null;

                    DestroyRunSplash();
                }
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