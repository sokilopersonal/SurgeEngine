using System.Collections;
using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.States.Characters.Sonic.SubStates;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility
{
    public class Upreel : StageObject, IPointMarkerLoader
    {
        [Header("Upreel")]
        [SerializeField] private float upMaxSpeed = 50;
        [SerializeField] private float impulseVelocity = 15;
        [SerializeField, Min(1)] private float length = 10;
        [SerializeField, Tooltip("Out Of Control time when player exits the Upreel.")] private float outOfControl = 0.5f;
        [SerializeField] private bool isWaitUp;
        
        [Header("Transforms")]
        [SerializeField] private Transform attachPoint;
        [SerializeField] private Transform model;
        [SerializeField] private LineRenderer rope;
        [SerializeField] private BoxCollider box;
        [SerializeField] private HomingTarget homingTarget;
        
        [Header("Sound")]
        [SerializeField] private EventReference sound;

        private Vector3 _localStartPosition;
        private bool _isPlayerAttached;
        private CharacterBase _attachedCharacter;
        private EventInstance _eventInstance;

        private Coroutine _lowerCoroutine;
        
        private float _currentSpeed;
        private bool _isMovingUp;
        private bool _isMovingDown;
        private Vector3 _targetLocalPosition;

        private void Awake()
        {
            _localStartPosition = model.localPosition;
            _eventInstance = RuntimeManager.CreateInstance(sound);
            _eventInstance.set3DAttributes(transform.To3DAttributes());

            PutModel();
            
            homingTarget.gameObject.SetActive(!isWaitUp);
        }

        private void Update()
        {
            rope.SetPosition(1, new Vector3(0, model.localPosition.y + 0.45f, 0));
            box.center = new Vector3(box.center.x, model.localPosition.y + 0.15f, box.center.z);
        }

        private void FixedUpdate()
        {
            if (_isMovingUp || _isMovingDown)
            {
                float acceleration = 30f;
                _currentSpeed = Mathf.MoveTowards(_currentSpeed, upMaxSpeed, acceleration * Time.fixedDeltaTime);

                Vector3 direction = (_targetLocalPosition - model.localPosition).normalized;
                float distanceToTarget = Vector3.Distance(model.localPosition, _targetLocalPosition);

                float stepDistance = _currentSpeed * Time.fixedDeltaTime;
                if (stepDistance >= distanceToTarget)
                {
                    model.localPosition = _targetLocalPosition;
                    _currentSpeed = 0f;
                    _isMovingUp = false;
                    _isMovingDown = false;
                }
                else
                {
                    model.localPosition += direction * stepDistance;
                }
            }

            if (_isPlayerAttached)
            {
                var ctx = _attachedCharacter;
                ctx.Rigidbody.MovePosition(attachPoint.position);
                ctx.Rigidbody.MoveRotation(attachPoint.rotation);

                Vector3 target = _localStartPosition + Vector3.up * length;
                float distance = Vector3.Distance(model.localPosition, target);
                if (distance < 0.1f)
                {
                    ctx.StateMachine.SetState<FStateAir>();
                    ctx.Kinematics.SetDetachTime(0.1f);
                    ctx.Kinematics.Rigidbody.position += Vector3.up;
                    ctx.Kinematics.Rigidbody.AddForce(transform.up * impulseVelocity, ForceMode.Impulse);
                    ctx.Kinematics.Rigidbody.AddForce(model.forward * impulseVelocity / 4, ForceMode.Impulse);

                    Cancel(ctx);
                }
            }
        }

        private void OnValidate()
        {
            if (model && box && rope)
            {
                model.localPosition = new Vector3(model.localPosition.x, -length, model.localPosition.z);
                box.center = new Vector3(box.center.x, model.localPosition.y + 0.15f, box.center.z);
                rope.SetPosition(1, new Vector3(0, model.localPosition.y + 0.45f, 0));
            }
        }

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);

            if (!_isPlayerAttached)
            {
                _attachedCharacter = context;
                _attachedCharacter.StateMachine.SetState<FStateUpreel>();

                if (_attachedCharacter.StateMachine.GetState(out FBoost boost))
                {
                    boost.Active = false;
                }
                
                _targetLocalPosition = _localStartPosition + Vector3.up * length;
                _currentSpeed = 0f;
                _isMovingUp = true;
                _isMovingDown = false;
                _isPlayerAttached = true;
            
                _eventInstance.start();
            }
        }

        private void PutModel()
        {
            if (!isWaitUp)
            {
                model.localPosition = new Vector3(model.localPosition.x, -length, model.localPosition.z);
            }
            else
            {
                model.localPosition = new Vector3(model.localPosition.x, -1, model.localPosition.z);
            }
        }

        private void Cancel(CharacterBase ctx)
        {
            _isPlayerAttached = false;
            _eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
            _currentSpeed = 0f;
            Lower();
            ctx.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, Mathf.Abs(outOfControl)));

            _attachedCharacter = null;
        }

        public void Lower(float delay = 0.5f)
        {
            homingTarget.gameObject.SetActive(true);

            if (_lowerCoroutine != null)
            {
                StopCoroutine(_lowerCoroutine);
            }

            _lowerCoroutine = StartCoroutine(LowerCoroutine(delay));
        }

        private IEnumerator LowerCoroutine(float delay)
        {
            if (delay > 0)
            {
                yield return new WaitForSeconds(delay);
            }

            StartLowering();
            _lowerCoroutine = null;
        }

        private void StartLowering()
        {
            _targetLocalPosition = _localStartPosition;
            _currentSpeed = 0f;
            _isMovingDown = true;
            _isMovingUp = false;
        }

        public void Load()
        {
            PutModel();
            
            homingTarget.gameObject.SetActive(isWaitUp);
            _isPlayerAttached = false;
            _attachedCharacter = null;
            _currentSpeed = 0f;
            _isMovingUp = false;
            _isMovingDown = false;
        }
    }
}