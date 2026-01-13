using Alchemy.Inspector;
using FMODUnity;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Source.Code.Infrastructure.Custom;
using SurgeEngine.Source.Code.Rendering;
using UnityEngine;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Collectables
{
    public class Ring : StageObject, IPointMarkerLoader
    {
        [SerializeField] private bool isLightSpeedDashTarget = true;
        [SerializeField] private ParticleSystem particle;
        [SerializeField] private EventReference ringSound;
        [FoldoutGroup("GPU Instancing"), Required, SerializeField] private GameObject model;
        
        public virtual bool IsLightSpeedDashTarget => isLightSpeedDashTarget;
        public virtual bool IsSuperRing => false;
        public bool InMagnet { get; private set; }

        private Stage Stage => Stage.Instance;
        
        private const float RotationSpeed = 240f;

        private CharacterBase _character;
        private float _factor;
        private Vector3 _velocity;

        private Vector3 _startPosition;
        private Quaternion _startRotation;

        private void Awake()
        {
            _startPosition = transform.position;
            _startRotation = transform.rotation;
        }

        private void OnEnable()
        {
            if (!IsSuperRing)
            {
                var instance = RingsGPURenderer.Instance;
                if (instance)
                {
                    instance.Register(transform);
                    model.SetActive(false);
                }
                else
                {
                    model.SetActive(true);
                }
            }
        }

        private void OnDisable()
        {
            if (!IsSuperRing)
            {
                var instance = RingsGPURenderer.Instance;
                if (instance)
                {
                    instance.Unregister(transform);
                }
            }
        }

        private void Update()
        {
            transform.rotation *= Quaternion.AngleAxis(RotationSpeed * Time.deltaTime, Vector3.up);
            
            if (_character == null)
            {
                return;
            }
            
            if (InMagnet)
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

        public override void OnEnter(Collider msg, CharacterBase context)
        {
            base.OnEnter(msg, context);

            Collect(1);
        }

        public virtual void StartMagnet(CharacterBase character)
        {
            if (!InMagnet)
            {
                InMagnet = true;
                
                _character = character;
            }
        }

        public virtual void Collect(int count)
        {
            if (!gameObject.activeInHierarchy)
                return;
            
            RuntimeManager.PlayOneShot(ringSound, transform.position);

            for (int i = 0; i < count; i++)
            {
                Stage.Data.RingCount++;
                Utility.AddScore(10);
            }
            
            var p = Instantiate(particle, transform.position, Quaternion.identity);
            Destroy(p.gameObject, 1f);
            gameObject.SetActive(false);
        }

        public void Load()
        {
            InMagnet = false;
            _factor = 0f;
            _velocity = Vector3.zero;

            transform.position = _startPosition;
            transform.rotation = _startRotation;
            gameObject.SetActive(true);
        }
    }
}
