using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Enemy.EggFighter.States;
using SurgeEngine.Code.StateMachine;
using UnityEngine;

namespace SurgeEngine.Code.Enemy.EggFighter
{
    public class EggFighter : EnemyBase, IDamageable
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

        protected override void Awake()
        {
            base.Awake();

            ragdollLayer = LayerMask.NameToLayer("EnemyRagdoll");

            rb = GetComponent<Rigidbody>();
            rb.freezeRotation = true;
            
            stateMachine.AddState(new EGStateIdle(this));
            stateMachine.AddState(new EGStateChase(this));
            stateMachine.AddState(new EGStatePatrol(this));
            stateMachine.AddState(new EGStateTurn(this));
            stateMachine.AddState(new EGStatePunch(this));
            stateMachine.AddState(new EGStateDead(this));
            
            stateMachine.SetState<EGStateIdle>();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, patrolDistance);
        }

        public void TakeDamage(object sender, float damage)
        {
            Actor context = ActorContext.Context;
            Vector3 force = context.kinematics.Rigidbody.linearVelocity * 1.25f;
            force += Vector3.up * (force.magnitude * 0.15f);
            Debug.DrawRay(context.transform.position, force, Color.red, 999f);
            stateMachine.SetState<EGStateDead>(0f, true, true).ApplyKnockback(force, ragdollPrefab);
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.layer == ragdollLayer)
            {
                Rigidbody rb = other.transform.GetComponent<Rigidbody>();
                stateMachine.SetState<EGStateDead>(0f, true, true).ApplyKnockback(rb.linearVelocity, ragdollPrefab);
            }
        }
    }
}