using FMODUnity;
using SurgeEngine.Code.Core.Actor.States.SonicSubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Infrastructure.Custom;
using SurgeEngine.Code.Infrastructure.Tools;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Collectables
{
    public class Ring : ContactBase
    {
        [SerializeField] private float rotationSpeed = 360f;
        [SerializeField] private float flyDuration = 1f;
        [SerializeField] private AnimationCurve heightCurve;
        [SerializeField] private ParticleSystem particle;
        
        [SerializeField] private EventReference ringSound;

        private ActorBase _actor => ActorContext.Context;
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

        protected override void Update()
        {
            if (_actor == null)
            {
                return;
            }
            
            transform.rotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up);
            
            if ((transform.position - _actor.transform.position).magnitude < _actor.stateMachine.GetSubState<FBoost>().GetConfig().magnetRadius
                && SonicTools.IsBoost() && !_magneted)
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
                    Collect();
                }
            }
        }

        public override void Contact(Collider msg, ActorBase context)
        {
            base.Contact(msg, context);

            Collect();
        }

        private void Collect()
        {
            RuntimeManager.PlayOneShot(ringSound);

            Common.AddScore(10);
            var p = Instantiate(particle, transform.position, Quaternion.identity);
            Destroy(p.gameObject, 1f);
            gameObject.SetActive(false);
        }
    }
}
