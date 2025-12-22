using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.AnimationCallback;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Interfaces;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Sensors;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Source.Code.Gameplay.Enemy.Base;
using SurgeEngine.Source.Code.Gameplay.Enemy.EggFighter.States;
using SurgeEngine.Source.Code.Gameplay.Enemy.RagdollPhysics;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace SurgeEngine.Source.Code.Gameplay.Enemy.EggFighter
{
    public class EggFighter : EnemyBase, IDamageable, IPointMarkerLoader
    {
        [SerializeField] private new EGAnimation animation;
        [SerializeField] private EGEffects effects;
        [SerializeField] private EnemyRagdoll ragdollPrefab;
        public EGAnimation Animation => animation;
        
        public VisionSensor Sensor { get; private set; }
        [field: SerializeField] public AnimationEventCallback PunchAnimationCallback { get; private set; }

        [Header("AI")]
        [SerializeField, Tooltip("Disabling this will disable enemy vision, meaning enemy won't notice you, but still will be functional.")] private bool enableAI = true;
        [SerializeField] private float punchRadius = 2f;
        [SerializeField] private bool followPlayer = true;
        public float PunchRadius => punchRadius;
        public bool FollowPlayer => followPlayer;

        public NavMeshAgent Agent { get; private set; }
        public CharacterBase Character => _character;

        [Inject] private CharacterBase _character;
        private int _ragdollLayer = 69;

        private Vector3 _startPosition;
        private Quaternion _startRotation;

        protected override void Awake()
        {
            base.Awake();
            
            animation.Initialize(this);
            effects.Initialize(this);
            
            _startPosition = transform.position;
            _startRotation = transform.rotation;

            _ragdollLayer = LayerMask.NameToLayer("EnemyRagdoll");

            Agent = GetComponent<NavMeshAgent>();
            Agent.updatePosition = false;
            Agent.updateRotation = true;
            
            if (!NavMesh.SamplePosition(Vector3.zero, out var hit, 1000.0f, NavMesh.AllAreas))
            {
                Agent.enabled = false;
            }
            
            Sensor = GetComponentInChildren<VisionSensor>();
            Sensor.enabled = enableAI;
            
            StateMachine.AddState(new EGStateIdle(this));
            StateMachine.AddState(new EGStateChase(this));
            StateMachine.AddState(new EGStatePunch(this));
            StateMachine.AddState(new EGStateDead(this));
            
            StateMachine.SetState<EGStateIdle>();
        }

        public void TakeDamage(Component sender)
        {
            if (!IsDead)
            {
                Vector3 force = sender.GetComponentInChildren<Rigidbody>().linearVelocity;
                
                Vector3 horizontal = Vector3.ProjectOnPlane(force, Vector3.up);
                Vector3 vertical = Vector3.Project(force, Vector3.up);
                vertical = Vector3.ClampMagnitude(vertical, 2f);
                
                Kill(horizontal + vertical);
            
                OnDied?.Invoke();
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == _ragdollLayer)
            {
                TakeDamage(other.rigidbody);
            }
        }

        private void Kill(Vector3 force)
        {
            StateMachine.SetState<EGStateDead>(true).ApplyKnockback(force, ragdollPrefab);
        }

        public void Load()
        {
            Agent.Warp(_startPosition);
            transform.rotation = _startRotation;

            IsDead = false;
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (StateMachine?.CurrentState != null)
            {
                float maxDistance = 40f;
                if (Camera.current != null)
                {
                    var style = new GUIStyle(EditorStyles.boldLabel);
                    style.fontSize = 24;
                    
                    float distance = Vector3.Distance(Camera.current.transform.position, transform.position);
                    if (distance <= maxDistance)
                    {
                        Handles.Label(transform.position + Vector3.up * 2f, StateMachine.CurrentState.GetType().Name, style);
                    }
                }
            }
#endif
        }
    }
}