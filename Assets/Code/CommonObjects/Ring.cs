using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Custom;
using SurgeEngine.Code.Parameters.SonicSubStates;
using UnityEngine;

namespace SurgeEngine.Code.CommonObjects
{
    public class Ring : ActorTrigger
    {
        [SerializeField] private float rotationSpeed = 360f;
        
        private Actor _actor => ActorContext.Context;
        
        private bool _magneted;
        private Vector3 _initialPosition;
        private float _factor;

        private void Update()
        {
            transform.rotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up);

            if ((transform.position - _actor.transform.position).magnitude < _actor.stateMachine.GetSubState<FBoost>().magnetRadius
                && _actor.stateMachine.GetSubState<FBoost>().Active && !_magneted)
            {
                _initialPosition = transform.position + Random.insideUnitSphere * 0.25f;
                _magneted = true;
            }

            if (_magneted)
            {
                transform.position = Vector3.Slerp(_initialPosition, 
                    _actor.transform.position - Vector3.up * 0.5f
                    + _actor.transform.forward * 0.2f, Easings.Get(Easing.InOutSine, _factor));
                _factor += Time.deltaTime / 1.45f;
            }
        }

        protected override void OnTriggerContact(Collider msg)
        {
            base.OnTriggerContact(msg);
            
            ActorEvents.OnRingCollected?.Invoke(this);
            
            gameObject.SetActive(false);
        }
    }
}