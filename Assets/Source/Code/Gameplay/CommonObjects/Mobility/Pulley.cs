using FMOD.Studio;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using UnityEngine;
using UnityEngine.Splines;
using UnityEngine.UIElements;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    public class Pulley : StageObject, IPointMarkerLoader
    {
        [Header("Pulley")]
        [SerializeField] private PulleyType type;
        [SerializeField] private float minSpeed = 1.0f;
        [SerializeField] private float maxSpeed = 5.0f;
        [SerializeField] private float exitSpeed = 30.0f;
        [SerializeField, Tooltip("Out Of Control time when player exits the Pulley.")] private float outOfControl = 0.5f;

        [Header("Transforms")]
        [SerializeField] private Transform attachPoint;
        [SerializeField] private SplineContainer spline;
        [SerializeField] private HomingTarget homingTarget;
        [SerializeField] private Transform longStand;
        [SerializeField] private Transform shortStand;
        [SerializeField] private Transform handle;

        private bool _isPlayerAttached;
        private CharacterBase _attachedCharacter;
        private float _time;
        private float _speed;
        private bool _trackPulley = false;

        private void OnValidate()
        {
            longStand.gameObject.SetActive(type.Equals(PulleyType.Long));
            shortStand.gameObject.SetActive(type.Equals(PulleyType.Short));
        }

        private float GetExitSpeed()
        {
            return exitSpeed + _speed;
        }

        private void FixedUpdate()
        {
            if (_trackPulley)
            {
                _time += Time.deltaTime * _speed;
                handle.position = spline.EvaluatePosition(Mathf.Min(_time, 1f));
                handle.rotation = Quaternion.LookRotation(spline.EvaluateTangent(Mathf.Min(_time, 0.99f)));

                if (_time >= 1.0f)
                    _trackPulley = false;
            }
            
            if (_isPlayerAttached)
            {

                _attachedCharacter.Rigidbody.MovePosition(attachPoint.position);
                _attachedCharacter.Rigidbody.MoveRotation(attachPoint.rotation);

                // Please sokilo i need this my jump is kinda homeless
                if (_attachedCharacter.Input.APressed)
                {
                    _attachedCharacter.Kinematics.SetDetachTime(0.1f);
                    _attachedCharacter.StateMachine.SetState<FStateJump>();
                    _attachedCharacter.Kinematics.Rigidbody.AddForce(handle.forward * GetExitSpeed() / 4, ForceMode.Impulse);
                    Cancel(_attachedCharacter);
                }

                if (_time > 0.9f)
                {
                    _attachedCharacter.Kinematics.SetDetachTime(0.1f);
                    _attachedCharacter.StateMachine.SetState<FStateAir>();
                    _attachedCharacter.Kinematics.Rigidbody.AddForce(transform.up * GetExitSpeed() / 4, ForceMode.Impulse);
                    _attachedCharacter.Kinematics.Rigidbody.AddForce(handle.forward * GetExitSpeed(), ForceMode.Impulse);

                    Cancel(_attachedCharacter);
                }
            }
            
            homingTarget.gameObject.SetActive(_time < 0.9f);
        }
        private void Cancel(CharacterBase ctx)
        {
            _isPlayerAttached = false;
            ctx.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, Mathf.Abs(outOfControl)));
            _attachedCharacter = null;
        }

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);

            if (!_isPlayerAttached)
            {
                _time = 0.0f;
                _speed = Mathf.Lerp(minSpeed, maxSpeed, context.Kinematics.Speed / context.Config.topSpeed);
                _attachedCharacter = context;
                _attachedCharacter.StateMachine.SetState<FStatePulley>();

                if (_attachedCharacter.StateMachine.GetState(out FBoost boost))
                {
                    boost.Active = false;
                }

                _isPlayerAttached = true;
                _trackPulley = true;
            }
        }

        public void Load()
        {
            homingTarget.gameObject.SetActive(true);
            _isPlayerAttached = false;
            _trackPulley = false;
            _attachedCharacter = null;
            _time = 0.0f;
        }
    }

    public enum PulleyType
    {
        Long,
        Short
    }
}
