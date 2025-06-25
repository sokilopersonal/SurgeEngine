using SurgeEngine.Code.Gameplay.Enemy.Base;
using UnityEngine;
using UnityEngine.AI;

namespace SurgeEngine.Code.Gameplay.Enemy.EggFighter.States
{
    public class EGStateChase : EGState
    {
        private Vector3 _normal;

        public EGStateChase(EnemyBase enemy) : base(enemy)
        {
            
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            bool hasTarget = sensor.FindVisibleTargets(out var pos);
            if (!hasTarget)
            {
                Debug.DrawLine(transform.position, pos, Color.blue);
            }

            bool cast = Physics.Raycast(transform.position, Vector3.down, out var ground, 1.5f, 1 << LayerMask.NameToLayer("Default"));
            if (cast)
            {
                _normal = ground.normal;
            }
            else
            {
                _normal = Vector3.up;
            }

            eggFighter.GetComponent<NavMeshAgent>().SetDestination(pos);

            if (Vector3.Distance(pos, transform.position) < eggFighter.punchRadius)
            {
                if (hasTarget)
                {
                    stateMachine.SetState<EGStatePunch>();
                }
                else
                {
                    stateMachine.SetState<EGStateIdle>();
                }
            }
        }
    }
}