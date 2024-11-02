using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.Enemy.States;
using SurgeEngine.Code.StateMachine;
using UnityEngine;
using UnityEngine.AI;

namespace SurgeEngine.Code.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EggFighter : EnemyBase, IActor
    {
        public new EnemyAnimation animation;
        public EGAnimationReference animationReference;

        public float chaseSpeed;
        public float findDistance;
        
        public float punchRadius;
        
        public AnimationCurve patrolSpeedCurve;
        public float patrolTime;
        public float patrolDistance;

        public AnimationCurve turnHeightCurve;
        public float turnTime;
        
        private Rigidbody _rigidbody;

        protected override void Awake()
        {
            base.Awake();
            
            _rigidbody = GetComponent<Rigidbody>();
            
            stateMachine.AddState(new EGStateIdle(this, transform, _rigidbody));
            stateMachine.AddState(new EGStateChase(this, transform, _rigidbody));
            stateMachine.AddState(new EGStatePatrol(this, transform, _rigidbody));
            stateMachine.AddState(new EGStateTurn(this, transform, _rigidbody));
            stateMachine.AddState(new EGStatePunch(this, transform, _rigidbody));
            
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
            foreach (var component in new IEnemyComponent[] { animation })
            {
                component?.SetOwner(this);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, patrolDistance);
        }
    }
}