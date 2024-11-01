using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AI;

namespace SurgeEngine.Code.Enemy.States
{
    public class EGStateTurn : EGState
    {
        private float _timer;
        private Quaternion _start;
        
        public EGStateTurn(EggFighter eggFighter, Transform transform, NavMeshAgent agent) : base(eggFighter, transform, agent)
        {
            
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0f;
            _start = transform.rotation;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            _timer += dt / 1.25f;
            
            Quaternion euler = Quaternion.Euler(0, _start.eulerAngles.y + 180f, 0);
            transform.rotation = Quaternion.Slerp(_start, euler, _timer);

            if (_timer >= 1)
            {
                Vector3 point = eggFighter.stateMachine.GetState<EGStatePatrol>().GetRandomPoint();
                eggFighter.animation.ResetAction();

                Debug.Log("123");
                
                eggFighter.stateMachine.SetState<EGStatePatrol>().SetNewPatrolPoint(point);
            }
        }
    }
}