using System.Collections.Generic;
using SurgeEngine.Code.ActorSystem;
using UnityEngine;
using UnityEngine.AI;

namespace SurgeEngine.Code.Enemy.States
{
    public class EGStatePatrol : EGState
    {
        private Vector3 _targetPoint;
        private float _moveInterval = 0.5f;
        private float _timer;
        private float _patrolTimer;
        
        public EGStatePatrol(EggFighter eggFighter, Transform transform, Rigidbody rb) : base(eggFighter, transform, rb)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _timer = 0;
            _patrolTimer = 0;

            if (eggFighter.stateMachine.PreviousState is EGStateTurn)
            {
                _targetPoint = Rb.position + transform.forward * eggFighter.patrolDistance;
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
            
            var rb = eggFighter.GetComponent<Rigidbody>();

            _timer += dt / _moveInterval;
            _moveInterval = eggFighter.punchRadius;
            
            _patrolTimer += dt;
            if (_patrolTimer > eggFighter.patrolTime + 3.5f)
            {
                eggFighter.stateMachine.SetState<EGStateIdle>(2f);
            }
            
            var context = ActorContext.Context;
            if (Vector3.Distance(context.transform.position, transform.position) < eggFighter.findDistance)
            {
                eggFighter.stateMachine.SetState<EGStateChase>();
            }

            // If it see the wall then turn
            if (Physics.Raycast(transform.position + Vector3.up, transform.forward, 3f, 1 << LayerMask.NameToLayer("Default")))
            {
                eggFighter.stateMachine.SetState<EGStateTurn>(0.5f);
            }

            float mod = eggFighter.patrolSpeedCurve.Evaluate(_timer);
            rb.linearVelocity = transform.forward * mod;
            
            Quaternion rotation = Quaternion.LookRotation(_targetPoint - transform.position, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, rotation.eulerAngles.y, 0), 24f * Time.deltaTime);
        }
        
        private Vector3 GetRandomPoint()
        {
            var unit = Random.insideUnitCircle.normalized * eggFighter.patrolDistance;
            Vector3 point = new Vector3(unit.x, 0f, unit.y);
            return transform.position + point;
        }
    }
}