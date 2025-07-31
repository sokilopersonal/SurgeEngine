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
        [SerializeField] private ParticleSystem particle;
        [SerializeField] private EventReference ringSound;
        
        private const float RotationSpeed = 240f;

        private ActorBase _actor;
        private bool _inMagnet;
        private float _factor;
        private Vector3 _velocity;

        private Vector3 _startPosition;

        private void Awake()
        {
            _startPosition = transform.position;
        }

        private void Update()
        {
            transform.rotation *= Quaternion.AngleAxis(RotationSpeed * Time.deltaTime, Vector3.up);
            
            if (_actor == null)
            {
                return;
            }
            
            if (_inMagnet)
            {
                float stiffnessRampTime = 0.5f;
                float baseStiffness = 12f;
                float baseDamping = 5f;
                float baseSpeed = 36f;

                _factor = Mathf.Clamp01(_factor + Time.deltaTime / stiffnessRampTime);

                Vector3 target = _actor.transform.position - Vector3.up * 0.9f;

                float stiffness = baseStiffness * (baseSpeed * _factor);
                float damping = baseDamping;

                Vector3 dir = (target - transform.position).normalized;
                Vector3 springForce = dir * stiffness;
                Vector3 dampForce = _velocity * damping;
                Vector3 force = springForce - dampForce;

                _velocity += force * Time.deltaTime;
                transform.position += _velocity * Time.deltaTime;
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

            transform.position = _startPosition;
            gameObject.SetActive(true);
        }
    }
}
