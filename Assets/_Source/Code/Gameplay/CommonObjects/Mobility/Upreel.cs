using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.System;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Mobility
{
    public class Upreel : ContactBase, IPointMarkerLoader
    {
        [Header("Transforms")]
        [SerializeField] private Transform attachPoint;
        [SerializeField] private Transform model;
        [SerializeField] private LineRenderer rope;
        [SerializeField] private BoxCollider box;
        [SerializeField] private HomingTarget homingTarget;
        
        [Header("Upreel")]
        [SerializeField, Tooltip("How long it takes to move to the target position")] private float moveTime = 1;
        [SerializeField, Tooltip("How long it takes to lower the upreel")] private float lowerTime = 1.25f;
        [SerializeField, Min(1)] private float length = 10;
        [SerializeField, Tooltip("Out Of Control time when player exits the Upreel.")] private float outOfControl = 0.5f;
        [SerializeField] private bool isWaitUp;
        
        [Header("After Speed")]
        [SerializeField] private float forwardPushForce = 5;
        [SerializeField] private float upPushForce = 14;
        
        [Header("Sound")]
        [SerializeField] private EventReference sound;

        private Vector3 _localStartPosition;
        private bool _isPlayerAttached;
        private CharacterBase _attachedCharacter;
        private EventInstance _eventInstance;

        private Tween _modelTween;
        
        private Vector3 _contactPoint;
        private float _attachTimer;

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
            
            if (_isPlayerAttached)
            {
                var ctx = _attachedCharacter;
                _attachTimer += Time.deltaTime / 0.1f;
                _attachTimer = Mathf.Clamp01(_attachTimer);
                
                Vector3 target = _localStartPosition + Vector3.up * length;
                float distance = Vector3.Distance(model.localPosition, target);
                if (distance < 0.1f)
                {
                    ctx.StateMachine.SetState<FStateAir>();
                    ctx.Kinematics.Rigidbody.position += Vector3.up;
                    ctx.Kinematics.Rigidbody.AddForce(transform.up * upPushForce, ForceMode.Impulse);
                    ctx.Kinematics.Rigidbody.AddForce(model.forward * forwardPushForce, ForceMode.Impulse);
                    
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

        private void FixedUpdate()
        {
            if (Application.isPlaying)
            {
                if (_isPlayerAttached)
                {
                    var ctx = _attachedCharacter;
                    ctx.transform.position = Vector3.Lerp(_contactPoint, attachPoint.position, _attachTimer);
                    ctx.transform.rotation = attachPoint.rotation;
                }
            }
        }

        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);
            
            _attachedCharacter = context;
            _attachedCharacter.StateMachine.SetState<FStateUpreel>();
            _contactPoint = _attachedCharacter.transform.position;
            _attachTimer = 0;

            if (_attachedCharacter.StateMachine.GetState(out FBoost boost))
            {
                boost.Active = false;
            }
            
            _modelTween?.Kill();
            _modelTween = model.DOLocalMove(_localStartPosition + Vector3.up * length, moveTime).SetEase(Ease.InSine).SetUpdate(UpdateType.Fixed).SetLink(gameObject).From(model.transform.localPosition);
            _isPlayerAttached = true;
            
            _eventInstance.start();
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
            _attachTimer = 0;
            _eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
            Lower();
            ctx.Flags.AddFlag(new Flag(FlagType.OutOfControl, true, Mathf.Abs(outOfControl)));

            _attachedCharacter = null;
        }

        public void Lower(float delay = 0.5f)
        {
            homingTarget.gameObject.SetActive(true);
            
            _modelTween?.Kill();
            _modelTween = model.DOLocalMove(_localStartPosition, lowerTime).SetEase(Ease.Linear).SetDelay(delay).SetLink(gameObject);
        }

        public void Load(Vector3 loadPosition, Quaternion loadRotation)
        {
            PutModel();
            
            homingTarget.gameObject.SetActive(isWaitUp);
            _isPlayerAttached = false;
            _attachTimer = 0;
            _attachedCharacter = null;
        }
    }
}