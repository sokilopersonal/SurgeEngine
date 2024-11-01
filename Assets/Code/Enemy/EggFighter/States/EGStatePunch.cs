using UnityEngine;
using UnityEngine.AI;

namespace SurgeEngine.Code.Enemy.States
{
    public class EGStatePunch : EGState
    {
        private float _stayTimer;
        
        public EGStatePunch(EggFighter eggFighter, Transform transform, NavMeshAgent agent) : base(eggFighter, transform, agent)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();
            
            _stayTimer = 0f;
            
            agent.velocity = Vector3.zero;
            agent.SetDestination(transform.position);
        }

        public override void OnExit()
        {
            base.OnExit();
            
            eggFighter.animation.ResetAction();
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            if (_stayTimer < 1f)
            {
                _stayTimer += Time.deltaTime;
            }
            else
            {
                eggFighter.stateMachine.SetState<EGStateIdle>(1.5f);
            }
        }
    }
}