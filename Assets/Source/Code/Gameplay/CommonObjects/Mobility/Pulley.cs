using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using UnityEngine;
using UnityEngine.Splines;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    public class Pulley : StageObject, IPointMarkerLoader
    {
        [Header("Pulley")]
        [SerializeField] private PulleyType type;
        [SerializeField] private float minSpeed = 1.0f;
        [SerializeField] private float maxSpeed = 5.0f;
        [SerializeField, Tooltip("Out Of Control time when player exits the Pulley.")] private float outOfControl = 0.5f;

        [Header("Transforms")]
        [SerializeField] private Transform attachPoint;
        [SerializeField] private SplineContainer spline;
        [SerializeField] private HomingTarget homingTarget;
        [SerializeField] private Transform longStand;
        [SerializeField] private Transform shortStand;
        [SerializeField] private Transform longEndpoint;
        [SerializeField] private Transform shortEndpoint;
        [SerializeField] private Transform handle;

        [Header("Raycast")]
        [SerializeField] private LayerMask rayMask;
        [SerializeField] private float longRayDistance;
        [SerializeField] private float shortRayDistance;

        [Header("Sound")]
        [SerializeField] private EventReference sound;

        private SplineData _splineData;
        private bool _isPlayerAttached;
        private CharacterBase _character;
        private float _speed;
        private bool _trackPulley;
        private bool _triggered;
        private BoxCollider _collider;
        private EventInstance _eventInstance;

#if UNITY_EDITOR
        private Vector3 _lastPosition;
#endif
        
        private void Awake()
        {
            if (!_collider) _collider = GetComponent<BoxCollider>();
            _eventInstance = RuntimeManager.CreateInstance(sound);
            _eventInstance.set3DAttributes(transform.To3DAttributes());
            
            UpdateVisual();
        }
        
#if UNITY_EDITOR
        private void UpdateVisual()
        {
            if (!longEndpoint || !shortEndpoint || !spline) return;

            if (_lastPosition == transform.position) return; // Only check when moved
            _lastPosition = transform.position;

            Vector3 pos = spline.transform.TransformPoint(spline.Spline.EvaluatePosition(1f));

            shortEndpoint.position = pos;
            longEndpoint.position = pos;

            if (type == PulleyType.Short)
            {
                bool shortCast = Physics.Raycast(pos, Vector3.down, shortRayDistance, rayMask);
                shortEndpoint.gameObject.SetActive(shortCast);
            }
            else
            {
                bool longCast = Physics.Raycast(pos, Vector3.down, longRayDistance, rayMask);
                longEndpoint.gameObject.SetActive(longCast);
            }
        }

        private void OnDrawGizmos()
        {
            if (!longEndpoint || !shortEndpoint) return;

            if (type == PulleyType.Long)
                Gizmos.DrawRay(longEndpoint.position, Vector3.down * longRayDistance);
            else
                Gizmos.DrawRay(shortEndpoint.position, Vector3.down * shortRayDistance);
            
            UpdateVisual();
        }
#endif
        
        private void FixedUpdate()
        {
            if (_splineData != null)
            {
                float time = _splineData.NormalizedTime;
                
                if (_trackPulley)
                {
                    _splineData.EvaluateWorld(out var pos, out var tangent, out _, out _);
                    
                    handle.position = pos;
                    if (tangent != Vector3.zero) handle.rotation = Quaternion.LookRotation(tangent);
                    _eventInstance.set3DAttributes(handle.To3DAttributes());

                    _splineData.Time += Time.fixedDeltaTime * _speed;
                    if (time >= 1.0f)
                    {
                        _trackPulley = false;
                        _eventInstance.stop(STOP_MODE.IMMEDIATE);
                    }
                }
            
                if (_isPlayerAttached)
                {
                    _character.Rigidbody.MovePosition(attachPoint.position);
                    _character.Rigidbody.MoveRotation(attachPoint.rotation);

                    if (time > 0.99f)
                    {
                        _character.Kinematics.SetDetachTime(0.1f);
                        _character.Kinematics.Rigidbody.linearVelocity = _character.Kinematics.Velocity;
                        _character.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, Mathf.Abs(outOfControl)));
                        _character.StateMachine.SetState<FStateAir>(); 
                    }
                }
            
                _collider.center = handle.localPosition - Vector3.up;
                homingTarget.gameObject.SetActive(_trackPulley);
            }
        }

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);

            if (!_isPlayerAttached && !_triggered)
            {
                AttachPlayer(context);
            }
        }

        public void AttachPlayer(CharacterBase context)
        {
            _splineData = new SplineData(spline, context.transform.position);
            
            _speed = Mathf.Clamp(context.Kinematics.Speed, minSpeed, maxSpeed);
            _character = context;
            _character.StateMachine.OnStateAssign += OnCharacterStateAssign;
            _character.StateMachine.SetState<FStatePulley>();

            if (_character.StateMachine.GetState(out FBoost boost))
            {
                boost.Active = false;
            }

            _isPlayerAttached = true;
            _trackPulley = true;
            _triggered = true;
            _eventInstance.start();
        }

        private void OnCharacterStateAssign(FState obj)
        {
            if (obj is not FStatePulley)
                Cancel();
        }

        private void Cancel()
        {
            _character.StateMachine.OnStateAssign -= OnCharacterStateAssign;
            
            _isPlayerAttached = false;
            _character = null;
        }

        private void OnDestroy()
        {
            _eventInstance.stop(STOP_MODE.IMMEDIATE);
        }

        public void Load()
        {
            homingTarget.gameObject.SetActive(true);
            _isPlayerAttached = false;
            _trackPulley = false;
            _triggered = false;
            _character = null;
            _eventInstance.stop(STOP_MODE.IMMEDIATE);
        }
    }

    public enum PulleyType
    {
        Long,
        Short
    }
}
