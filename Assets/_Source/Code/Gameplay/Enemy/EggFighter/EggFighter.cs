using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.AnimationCallback;
using SurgeEngine.Code.Gameplay.CommonObjects.Interfaces;
using SurgeEngine.Code.Gameplay.CommonObjects.Sensors;
using SurgeEngine.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Code.Gameplay.Enemy.Base;
using SurgeEngine.Code.Gameplay.Enemy.EggFighter.States;
using SurgeEngine.Code.Gameplay.Enemy.RagdollPhysics;
using UnityEngine;
using UnityEngine.AI;

namespace SurgeEngine.Code.Gameplay.Enemy.EggFighter
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
        
        [Header("Idle")]
        public float findDistance;
        
        [Header("Punch")]
        public float punchRadius;
        
        [Header("Patrol")]
        public AnimationCurve patrolSpeedCurve;
        public float patrolTime;
        public float patrolDistance;

        [Header("Turn")]
        public AnimationCurve turnCurve;
        public AnimationCurve turnHeightCurve;
        public float turnTime;

        public NavMeshAgent Agent { get; private set; }
        private int ragdollLayer = 69;

        private Vector3 _startPosition;
        private Quaternion _startRotation;

        protected override void Awake()
        {
            base.Awake();
            
            animation.Initialize(this);
            effects.Initialize(this);
            
            _startPosition = transform.position;
            _startRotation = transform.rotation;

            ragdollLayer = LayerMask.NameToLayer("EnemyRagdoll");

            Agent = GetComponent<NavMeshAgent>();
            Agent.updatePosition = false;
            Agent.updateRotation = true;
            
            Sensor = GetComponentInChildren<VisionSensor>();
            Sensor.enabled = enableAI;
            
            StateMachine.AddState(new EGStateIdle(this));
            StateMachine.AddState(new EGStateChase(this));
            StateMachine.AddState(new EGStatePatrol(this));
            StateMachine.AddState(new EGStateTurn(this));
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
            if (other.gameObject.layer == ragdollLayer)
            {
                TakeDamage(other.rigidbody);
            }
        }

        private void Kill(Vector3 force)
        {
            StateMachine.SetState<EGStateDead>(true).ApplyKnockback(force, ragdollPrefab);
        }

        public void Load(Vector3 loadPosition, Quaternion loadRotation)
        {
            Agent.Warp(_startPosition);
            transform.rotation = _startRotation;

            IsDead = false;
        }
    }
}