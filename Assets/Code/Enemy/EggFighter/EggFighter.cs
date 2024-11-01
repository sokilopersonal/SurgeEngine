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

        public float chaseSpeed;
        public float findDistance;
        
        public float punchRadius;
        
        public AnimationCurve patrolSpeedCurve;
        public float patrolSpeed;
        public float patrolTime;
        public float patrolDistance;

        protected override void Awake()
        {
            base.Awake();
            
            var agent = GetComponent<NavMeshAgent>();
            
            stateMachine.AddState(new EGStateIdle(this, transform, agent));
            stateMachine.AddState(new EGStateChase(this, transform, agent));
            stateMachine.AddState(new EGStatePatrol(this, transform, agent));
            stateMachine.AddState(new EGStateTurn(this, transform, agent));
            stateMachine.AddState(new EGStatePunch(this, transform, agent));
            
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