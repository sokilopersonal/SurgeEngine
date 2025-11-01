using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using UnityEngine;
using Zenject;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Collectables
{
    public class Ring : StageObject, IPointMarkerLoader
    {
        [SerializeField] private ParticleSystem particle;
        [SerializeField] private EventReference ringSound;
        
        [Inject] private Stage _stage;
        
        private const float RotationSpeed = 240f;

        private CharacterBase _character;
        private bool _inMagnet;
        private float _factor;
        private Vector3 _velocity;

        private Vector3 _startPosition;
        private Quaternion _startRotation;

        private void Awake()
        {
            _startPosition = transform.position;
            _startRotation = transform.rotation;
        }

        private void Update()
        {
            transform.rotation *= Quaternion.AngleAxis(RotationSpeed * Time.deltaTime, Vector3.up);
            
            if (_character == null)
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

                Vector3 target = _character.transform.position - Vector3.up * 0.9f;

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

        public override void Contact(Collider msg, CharacterBase context)
        {
            base.Contact(msg, context);

            Collect(1);
        }

        public virtual void StartMagnet(CharacterBase character)
        {
            if (!_inMagnet)
            {
                _inMagnet = true;
                
                _character = character;
            }
        }

        protected virtual void Collect(int count)
        {
            RuntimeManager.PlayOneShot(ringSound, transform.position);

            for (int i = 0; i < count; i++)
            {
                _stage.Data.RingCount++;
                Utility.AddScore(10);
            }
            
            var p = Instantiate(particle, transform.position, Quaternion.identity);
            Destroy(p.gameObject, 1f);
            gameObject.SetActive(false);
        }

        public void Load()
        {
            _inMagnet = false;
            _factor = 0f;
            _velocity = Vector3.zero;

            transform.position = _startPosition;
            transform.rotation = _startRotation;
            gameObject.SetActive(true);
        }

        public virtual bool IsSuperRing() => false;
    }
}
