using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.Actor.States;
using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Code.CommonObjects
{
    public class Upreel : ContactBase
    {
        [Header("Transforms")]
        [SerializeField] private Transform attachPoint;
        [SerializeField] private Transform model;
        [SerializeField] private LineRenderer rope;
        [SerializeField] private BoxCollider box;
        
        [Header("Upreel Movement")]
        [SerializeField, Tooltip("How long it takes to move to the target position")] private float moveTime = 2;
        [SerializeField, Min(1)] private float length = 25;
        
        [Header("After Speed")]
        [SerializeField] private float forwardPushForce = 7;
        [SerializeField] private float upPushForce = 10;
        
        [Header("Sound")]
        [SerializeField] private EventReference sound;

        private Vector3 _localStartPosition;
        private bool _isPlayerAttached;
        private EventInstance _eventInstance;

        private Vector3 _contactPoint;
        private float _attachTimer;

        protected override void Awake()
        {
            base.Awake();
            
            _localStartPosition = model.localPosition;
            _eventInstance = RuntimeManager.CreateInstance(sound);
            _eventInstance.set3DAttributes(transform.To3DAttributes());
            
            model.localPosition = new Vector3(model.localPosition.x, -length, model.localPosition.z);
            box.center = new Vector3(box.center.x, model.localPosition.y + 0.15f, box.center.z);
        }

        protected override void Update()
        {
            base.Update();

            rope.SetPosition(1, new Vector3(0, model.localPosition.y + 0.45f, 0));
            
            if (_isPlayerAttached)
            {
                var ctx = ActorContext.Context;
                _attachTimer += Time.deltaTime / 0.1f;
                _attachTimer = Mathf.Clamp01(_attachTimer);
                
                Vector3 target = _localStartPosition + Vector3.up * length;
                float distance = Vector3.Distance(model.localPosition, target);
                if (distance < 0.1f)
                {
                    ctx.stateMachine.SetState<FStateAir>();
                    ctx.kinematics.Rigidbody.position += Vector3.up;
                    ctx.kinematics.Rigidbody.AddForce(transform.up * upPushForce, ForceMode.Impulse);
                    ctx.kinematics.Rigidbody.AddForce(model.forward * forwardPushForce, ForceMode.Impulse);
                    
                    Cancel(ctx);
                }
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            if (_isPlayerAttached)
            {
                var ctx = ActorContext.Context;
                ctx.transform.position = Vector3.Lerp(_contactPoint, attachPoint.position, _attachTimer);
                ctx.transform.rotation = attachPoint.rotation;
            }
        }

        public override void Contact(Collider msg)
        {
            base.Contact(msg);
            
            ActorBase context = ActorContext.Context;
            context.stateMachine.SetState<FStateUpreel>(0.1f)?.SetAttach(attachPoint);
            context.stateMachine.OnStateAssign += OnStateAssign;

            _contactPoint = context.transform.position;
            _attachTimer = 0;
            
            model.DOLocalMove(_localStartPosition + Vector3.up * length, moveTime).SetEase(Ease.InSine).SetUpdate(UpdateType.Fixed).SetLink(gameObject);
            _isPlayerAttached = true;
            
            context.camera.stateMachine.SetLateOffset(context.transform.position - attachPoint.position);
            
            _eventInstance.start();
        }

        private void Cancel(ActorBase ctx)
        {
            _isPlayerAttached = false;
            _attachTimer = 0;
            _eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
            model.DOLocalMove(_localStartPosition, 1f).SetEase(Ease.InSine).SetDelay(0.5f).SetLink(gameObject);
            ctx.stateMachine.OnStateAssign -= OnStateAssign;
        }

        private void OnStateAssign(FState obj)
        {
            if (obj is not FStateUpreel)
            {
                Cancel(ActorContext.Context);
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (!Application.isPlaying)
            {
                if (model)
                {
                    model.localPosition = new Vector3(model.localPosition.x, -length, model.localPosition.z);
                }

                if (box)
                {
                    box.center = new Vector3(box.center.x, model.localPosition.y + 0.15f, box.center.z);
                }

                if (rope)
                {
                    rope.SetPosition(1, new Vector3(0, model.localPosition.y + 0.45f, 0));
                }
            }
        }
    }
}