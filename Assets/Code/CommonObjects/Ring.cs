using FMODUnity;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Parameters.SonicSubStates;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class Ring : ActorTrigger
    {
        [SerializeField] private float rotationSpeed = 360f;
        [SerializeField] private float flyDuration = 1f;
        [SerializeField] private AnimationCurve heightCurve;
        
        [SerializeField] private EventReference ringSound;

        private Actor _actor => ActorContext.Context;
        private bool _magneted;
        private Vector3 _initialPosition;
        private float _factor;
        private Vector3 _targetPosition;
        private float _elapsedTime;

        private void Start()
        {
            if (heightCurve == null)
            {
                heightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            }
        }

        private void Update()
        {
            transform.rotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up);
            
            if ((transform.position - _actor.transform.position).magnitude < _actor.stateMachine.GetSubState<FBoost>().magnetRadius
                && _actor.stateMachine.GetSubState<FBoost>().Active && !_magneted)
            {
                _initialPosition = transform.position;
                _magneted = true;
                _elapsedTime = 0f;
            }
            
            if (_magneted)
            {
                _elapsedTime += Time.deltaTime;
                _factor = Mathf.Clamp01(_elapsedTime / flyDuration);
                _targetPosition = _actor.transform.position - _actor.transform.up * 0.5f;
                
                Vector3 flatPosition = Vector3.Lerp(_initialPosition, _targetPosition, _factor);
                
                float heightOffset = heightCurve.Evaluate(_factor);
                flatPosition.y += heightOffset;

                transform.position = flatPosition;
                
                if (_factor >= 1f)
                {
                    OnTriggerContact(null);
                }
            }
        }

        public override void OnTriggerContact(Collider msg)
        {
            base.OnTriggerContact(msg);
            
            RuntimeManager.PlayOneShot(ringSound);

            ActorEvents.OnRingCollected?.Invoke(this);
            gameObject.SetActive(false);
        }
    }
}
