using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.Enemy.Base;
using UnityEngine;

namespace SurgeEngine.Code.Gameplay.Enemy.EggFighter.States
{
    public class EGStatePatrol : EGState
    {
        private Vector3 _targetPoint;
        private float _moveInterval = 0.5f;
        private float _timer;
        private float _patrolTimer;
        
        public EGStatePatrol(EnemyBase enemy) : base(enemy)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0;
            _patrolTimer = 0;

            if (eggFighter.stateMachine.PreviousState is EGStateTurn)
            {
                _targetPoint = eggFighter.rb.position + transform.forward * eggFighter.patrolDistance;
            }
            else
            {
                _targetPoint = GetRandomPoint();
            }
            
            eggFighter.animationReference.OnStepWalk += OnStepWalk;
        }

        private void OnStepWalk()
        {
            _timer = 0;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);
            
            Rigidbody rb = eggFighter.GetComponent<Rigidbody>();

            _timer += dt / _moveInterval;
            _moveInterval = eggFighter.punchRadius;

            float mod = eggFighter.patrolSpeedCurve.Evaluate(_timer);
            rb.linearVelocity = (_targetPoint - transform.position).normalized * mod;
            
            Quaternion rotation = Quaternion.LookRotation(_targetPoint - transform.position, Vector3.up);
            transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);
            
            _patrolTimer += dt;
            if (_patrolTimer > eggFighter.patrolTime + 3.5f)
            {
                eggFighter.stateMachine.SetState<EGStateIdle>(2f);
            }
            
            ActorBase context = ActorContext.Context;
            if (Vector3.Distance(context.transform.position, transform.position) < eggFighter.findDistance)
            {
                eggFighter.stateMachine.SetState<EGStateChase>();
            }

            // If it see the wall then turn
            if (UnityEngine.Physics.SphereCast(transform.position + Vector3.up, 1.5f, transform.forward, out _, 1f, 1 << LayerMask.NameToLayer("Default")))
            {
                eggFighter.stateMachine.SetState<EGStateTurn>();
            }
        }
        
        private Vector3 GetRandomPoint()
        {
            Vector2 unit = Random.insideUnitCircle.normalized * eggFighter.patrolDistance;
            Vector3 point = new Vector3(unit.x, 0f, unit.y);
            return transform.position + point;
        }
    }
}