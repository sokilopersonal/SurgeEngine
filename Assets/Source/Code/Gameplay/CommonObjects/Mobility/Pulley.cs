using System.Collections;
using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.Character.System;
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
        [SerializeField] private Transform handle;

        [Header("Sound")]
        [SerializeField] private EventReference sound;

        private bool _isPlayerAttached;
        private CharacterBase _character;
        private float _time;
        private float _speed;
        private bool _trackPulley;
        private bool _triggered = false;
        private BoxCollider _collider;
        private EventInstance _eventInstance;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider>();
            _eventInstance = RuntimeManager.CreateInstance(sound);
            _eventInstance.set3DAttributes(transform.To3DAttributes());
        }

        private void OnValidate()
        {
            if (!longStand || !shortStand)
                return;
            
            longStand.gameObject.SetActive(type == PulleyType.Long);
            shortStand.gameObject.SetActive(type == PulleyType.Short);
        }

        private void Update()
        {
            if (!_isPlayerAttached)
                return;

            // Please sokilo i need this my jump is kinda homeless
            // S: you're fine bro, I came to help
            if (_character.Input.APressed)
            {
                _character.Kinematics.SetDetachTime(0.1f);
                _character.Kinematics.Rigidbody.linearVelocity = _character.Kinematics.Velocity;
                _character.StateMachine.SetState<FStateJump>();

                Cancel();
            }
        }

        private void FixedUpdate()
        {
            if (_trackPulley)
            {
                _time += Time.fixedDeltaTime * _speed;
                handle.position = spline.EvaluatePosition(Mathf.Min(_time, 1f));
                handle.rotation = Quaternion.LookRotation(spline.EvaluateTangent(Mathf.Min(_time, 0.99f)));

                if (_time >= 1.0f)
                    _trackPulley = false;
            }
            
            if (_isPlayerAttached)
            {
                _character.Rigidbody.MovePosition(attachPoint.position);
                _character.Rigidbody.MoveRotation(attachPoint.rotation);

                if (_time > 0.99f)
                {
                    _character.Kinematics.SetDetachTime(0.1f);
                    _character.Kinematics.Rigidbody.linearVelocity = _character.Kinematics.Velocity;
                    _character.StateMachine.SetState<FStateAir>(); 
                    _character.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, Mathf.Abs(outOfControl)));

                    Cancel();
                }
            }
            
            _collider.center = handle.localPosition - Vector3.up;
            
            homingTarget.gameObject.SetActive(_time < 0.99f);
        }
        
        private void Cancel()
        {
            _isPlayerAttached = false;
            _character = null;
            _eventInstance.stop(STOP_MODE.IMMEDIATE);
        }

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);

            if (!_isPlayerAttached && !_triggered)
            {
                _time = 0.0f;
                _speed = Mathf.Lerp(minSpeed, maxSpeed, context.Kinematics.Speed / context.Config.topSpeed);
                _character = context;
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
        }

        public void Load()
        {
            homingTarget.gameObject.SetActive(true);
            _isPlayerAttached = false;
            _trackPulley = false;
            _triggered = false;
            _character = null;
            _time = 0.0f;
        }
    }

    public enum PulleyType
    {
        Long,
        Short
    }
}
