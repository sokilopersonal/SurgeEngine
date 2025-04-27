using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.Interfaces;
using SurgeEngine.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Code.Gameplay.Enemy.Base;
using SurgeEngine.Code.Gameplay.Enemy.EggFighter.States;
using SurgeEngine.Code.Gameplay.Enemy.Physics;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.Enemy.EggFighter
{
    public class EggFighter : EnemyBase, IDamageable, IPointMarkerLoader
    {
        public EGView View => view as EGView;
        public EGAnimationReference animationReference;
        public EnemyRagdoll ragdollPrefab;

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
        
        [HideInInspector] public Rigidbody rb;
        private int ragdollLayer = 69;

        private Vector3 _startPosition;
        private Quaternion _startRotation;

        protected override void Awake()
        {
            base.Awake();
            
            _startPosition = transform.position;
            _startRotation = transform.rotation;

            ragdollLayer = LayerMask.NameToLayer("EnemyRagdoll");

            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            
            StateMachine.AddState(new EGStateIdle(this));
            StateMachine.AddState(new EGStateChase(this));
            StateMachine.AddState(new EGStatePatrol(this));
            StateMachine.AddState(new EGStateTurn(this));
            StateMachine.AddState(new EGStatePunch(this));
            StateMachine.AddState(new EGStateDead(this));
            
            StateMachine.SetState<EGStateIdle>();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, patrolDistance);
        }

        public void TakeDamage(Entity sender, float damage)
        {
            ActorBase context = ActorContext.Context;
            Vector3 force = context.kinematics.Rigidbody.linearVelocity * 1.25f;
            force += Vector3.up * (force.magnitude * 0.15f);
            Debug.DrawRay(context.transform.position, force, Color.red, 999f);
            StateMachine.SetState<EGStateDead>(0f, true, true).ApplyKnockback(force, ragdollPrefab);
            
            OnDied?.Invoke();
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == ragdollLayer)
            {
                Rigidbody rb = other.transform.GetComponent<Rigidbody>();
                StateMachine.SetState<EGStateDead>(0f, true, true).ApplyKnockback(rb.linearVelocity, ragdollPrefab);
            }
        }

        public void Load(Vector3 loadPosition, Quaternion loadRotation)
        {
            transform.position = _startPosition;
            transform.rotation = _startRotation;
        }
    }
}