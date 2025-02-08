using DG.Tweening;
using FMOD.Studio;
using FMODUnity;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

namespace SurgeEngine.Code.CommonObjects
{
    // TODO: Fix that player lags behind the moving upreel
    public class Pulley : ContactBase
    {
        [Header("Transforms")]
        [SerializeField] private Transform attachPoint;
        [SerializeField] private Transform model;
        
        [Header("Pulley Movement")]
        [SerializeField, Tooltip("How long it takes to move to the target position")] private float moveTime = 2;
        [SerializeField] private float length = 25;
        
        [Header("After Speed")]
        [SerializeField] private float forwardPushForce = 7;
        [SerializeField] private float upPushForce = 10;
        
        [Header("Sound")]
        [SerializeField] private EventReference sound;

        private Vector3 _localStartPosition;
        private bool _isPlayerAttached;
        private EventInstance _eventInstance;

        protected override void Awake()
        {
            base.Awake();
            
            _localStartPosition = model.localPosition;
            _eventInstance = RuntimeManager.CreateInstance(sound);
            _eventInstance.set3DAttributes(transform.To3DAttributes());
        }

        protected override void Update()
        {
            base.Update();

            if (_isPlayerAttached)
            {
                Vector3 target = _localStartPosition + Vector3.up * length;
                float distance = Vector3.Distance(model.localPosition, target);
                if (distance < 0.1f)
                {
                    var ctx = ActorContext.Context;
                    ctx.stateMachine.SetState<FStateAir>();
                    ctx.kinematics.Rigidbody.AddForce(Vector3.up * upPushForce, ForceMode.Impulse);
                    ctx.kinematics.Rigidbody.AddForce(model.forward * forwardPushForce, ForceMode.Impulse);
                    
                    Cancel(ctx);
                }
            }
        }

        public override void Contact(Collider msg)
        {
            base.Contact(msg);
            
            Actor context = ActorContext.Context;
            context.stateMachine.SetState<FStatePulley>(0.1f)?.SetAttach(attachPoint);
            context.stateMachine.OnStateAssign += OnStateAssign;
            
            model.DOLocalMove(_localStartPosition + Vector3.up * length, moveTime).SetEase(Ease.InSine).SetUpdate(UpdateType.Fixed);
            _isPlayerAttached = true;
            _eventInstance.start();
        }

        private void Cancel(Actor ctx)
        {
            _isPlayerAttached = false;
            _eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
            model.DOLocalMove(_localStartPosition, 1f).SetEase(Ease.InSine).SetDelay(0.5f);
            ctx.stateMachine.OnStateAssign -= OnStateAssign;
        }

        private void OnStateAssign(FState obj)
        {
            if (obj is not FStatePulley)
            {
                Cancel(ActorContext.Context);
            }
        }

        protected override void OnDrawGizmos()
        {
            base.OnDrawGizmos();

            if (model)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, transform.position + Vector3.up * length);
            }
        }
    }
}