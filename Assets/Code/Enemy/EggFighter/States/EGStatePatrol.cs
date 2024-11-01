using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SurgeEngine.Code.Enemy.States
{
    public class EGStatePatrol : EGState
    {
        private Vector3 _targetPoint;
        private List<Vector3> _waypoints;
        private int _currentWaypointIndex = 0;
        private float _moveInterval = 0.5f;
        private float _timer = 0f;
        
        public EGStatePatrol(EggFighter eggFighter, Transform transform, NavMeshAgent agent) : base(eggFighter, transform, agent)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
        }

        public void SetNewPatrolPoint(Vector3 point)
        {
            _targetPoint = point;
            
            CreateWaypoints();
            MoveToNextWaypoint();
        }

        public Vector3 GetRandomPoint()
        {
            var unit = Random.insideUnitCircle.normalized * eggFighter.patrolDistance;
            Vector3 unitVector = new Vector3(unit.x, 0, unit.y);
            return eggFighter.transform.position + unitVector;
        }

        private void CreateWaypoints()
        {
            _waypoints = new List<Vector3>();
            Vector3 startPosition = transform.position;
            int steps = 12;

            for (int i = 1; i <= steps; i++)
            {
                Vector3 point = Vector3.Lerp(startPosition, _targetPoint, i / (float)steps);
                point.y = transform.position.y;
                
                Debug.DrawRay(point, Vector3.up, Color.red, 12f);
                
                _waypoints.Add(point);
            }
            
            _currentWaypointIndex = 0;
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            _moveInterval = eggFighter.punchRadius;
            if (_currentWaypointIndex < _waypoints.Count)
            {
                Vector3 direction = (_waypoints[_currentWaypointIndex] - transform.position).normalized;
                agent.velocity = direction * eggFighter.patrolSpeedCurve.Evaluate(_timer / _moveInterval);
            
                _timer += dt;
                Debug.DrawLine(transform.position, _waypoints[_currentWaypointIndex], Color.red);
                float distance = Vector3.Distance(transform.position, _waypoints[_currentWaypointIndex] + Vector3.up);
                if (distance < 5f && _timer >= _moveInterval)
                {
                    MoveToNextWaypoint();
                }
            
                if (agent.velocity.magnitude > 0.2f)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(agent.velocity, Vector3.up);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, dt * 12);
                }
            }
            
            if (_currentWaypointIndex >= _waypoints.Count && Vector3.Distance(transform.position, _targetPoint) < 1f)
            {
                eggFighter.stateMachine.SetState<EGStateIdle>();
            }
        }

        private void MoveToNextWaypoint()
        {
            _timer = 0f;
            if (_currentWaypointIndex < _waypoints.Count)
            {
                agent.SetDestination(_waypoints[_currentWaypointIndex]);
                _currentWaypointIndex++;
            }
        }
    }
}