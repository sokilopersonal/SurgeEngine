using FMODUnity;
using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Code.Infrastructure.Custom;
using SurgeEngine.Code.Infrastructure.Tools;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.CommonObjects.Collectables
{
    public class Ring : ContactBase, IPointMarkerLoader
    {
        [SerializeField] private float rotationSpeed = 360f;
        [SerializeField] private float flyDuration = 1f;
        [SerializeField] private AnimationCurve heightCurve;
        [SerializeField] private ParticleSystem particle;
        
        [SerializeField] private EventReference ringSound;

        private ActorBase _actor;
        private bool _inMagnet;
        private Vector3 _initialPosition;
        private float _factor;
        private Vector3 _targetPosition;
        private float _elapsedTime;

        private Vector3 _startPosition;

        private void Awake()
        {
            _startPosition = transform.position;
            
            if (heightCurve == null)
            {
                heightCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            }
        }

        private void Update()
        {
            transform.rotation *= Quaternion.AngleAxis(rotationSpeed * Time.deltaTime, Vector3.up);
            
            if (_actor == null)
            {
                return;
            }
            
            if (_inMagnet)
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
                    Contact(null, null);
                }
            }
        }

        public override void Contact(Collider msg, ActorBase context)
        {
            base.Contact(msg, context);

            Collect();
        }

        public void StartMagnet(ActorBase actor)
        {
            if (!_inMagnet)
            {
                _inMagnet = true;
                
                _actor = actor;
                _initialPosition = transform.position;
                _elapsedTime = 0f;
            }
        }

        private void Collect()
        {
            RuntimeManager.PlayOneShot(ringSound, transform.position);

            Utility.AddScore(10);
            var p = Instantiate(particle, transform.position, Quaternion.identity);
            Destroy(p.gameObject, 1f);
            gameObject.SetActive(false);
        }

        public void Load(Vector3 loadPosition, Quaternion loadRotation)
        {
            _inMagnet = false;
            _elapsedTime = 0f;

            transform.position = _startPosition;
            gameObject.SetActive(true);
        }
    }
}
