using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.Enemy.States;
using SurgeEngine.Code.StateMachine;
using SurgeEngine.Code.Tools;
using UnityEngine;

namespace SurgeEngine.Code.Enemy
{
    public class EggFighter : EnemyBase, IPlayerContactable
    {
        public new EnemyAnimation animation;
        public EGView view;
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
        
        private Rigidbody _rigidbody;
        private int ragdollLayer = 69;

        protected override void Awake()
        {
            base.Awake();

            ragdollLayer = LayerMask.NameToLayer("EnemyRagdoll");

            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.freezeRotation = true;
            
            stateMachine.AddState(new EGStateIdle(this, transform, _rigidbody));
            stateMachine.AddState(new EGStateChase(this, transform, _rigidbody));
            stateMachine.AddState(new EGStatePatrol(this, transform, _rigidbody));
            stateMachine.AddState(new EGStateTurn(this, transform, _rigidbody));
            stateMachine.AddState(new EGStatePunch(this, transform, _rigidbody));
            stateMachine.AddState(new EGStateDead(this, transform, _rigidbody));
            
            stateMachine.SetState<EGStateIdle>();
            InitializeComponents();
        }

        private void Update()
        {
            stateMachine.Tick(Time.deltaTime);
        }
        
        private void FixedUpdate()
        {
            stateMachine.FixedTick(Time.fixedDeltaTime);
        }

        private void LateUpdate()
        {
            stateMachine.LateTick(Time.deltaTime);
        }

        public void InitializeComponents()
        {
            foreach (IEnemyComponent component in new IEnemyComponent[] { view, animation })
            {
                component?.SetOwner(this);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, patrolDistance);
        }

        public void OnContact()
        {
            if (!CanBeDamaged())
            {
                return;
            }
            
            Actor context = ActorContext.Context;
            Vector3 force = context.kinematics.Rigidbody.linearVelocity * 1.15f * (SonicTools.IsBoost() ? 2f : 1f);
            force += Vector3.up * force.magnitude * 0.165f;
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