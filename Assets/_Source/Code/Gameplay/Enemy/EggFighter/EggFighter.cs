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
        public EGView View => view as EGView;
        public EggFighterEffects effects;
        public EnemyRagdoll ragdollPrefab;
        public VisionSensor Sensor { get; private set; }
        [field: SerializeField] public AnimationEventCallback PunchAnimationCallback { get; private set; }

        [Header("AI")]
        [SerializeField, Tooltip("Disabling this will disable enemy vision, meaning enemy won't notice you, but still will be functional.")] private bool enableAI = true;
        
        [Header("Idle")]
        public float findDistance;
        
        [Header("Chase")]
        public float chaseSpeed;
        public float chaseMaxDistance = 12;
        
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

        public void TakeDamage(MonoBehaviour sender, float damage)
        {
            Vector3 force = sender.GetComponent<ActorBase>().Kinematics.Rigidbody.linearVelocity * 1.25f;
            force += Vector3.up * (force.magnitude * 0.15f);
            StateMachine.SetState<EGStateDead>(0f, true, true).ApplyKnockback(force, ragdollPrefab);
            
            OnDied?.Invoke();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == ragdollLayer)
            {
                StateMachine.SetState<EGStateDead>(0f, true, true).ApplyKnockback(other.rigidbody.linearVelocity, ragdollPrefab);
                OnDied?.Invoke();
            }
        }

        public void Load(Vector3 loadPosition, Quaternion loadRotation)
        {
            transform.position = _startPosition;
            transform.rotation = _startRotation;
        }
    }
}