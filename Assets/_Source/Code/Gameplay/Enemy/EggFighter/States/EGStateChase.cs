using SurgeEngine.Code.Gameplay.Enemy.Base;
using UnityEngine;
using UnityEngine.AI;

namespace SurgeEngine.Code.Gameplay.Enemy.EggFighter.States
{
    public class EGStateChase : EGState
    {
        private readonly NavMeshPath navMeshPath = new();
        private Vector3 _normal;
        private Vector3 _obstacleAvoidanceDirection = Vector3.zero;
        private float _obstacleAvoidanceTimer = 0f;
        private readonly float _obstacleAvoidanceDuration = 0.15f;
        private readonly float _obstacleDetectionDistance = 2.5f;
        private readonly float _obstacleDetectionRadius = 1.0f;
        private readonly int _obstacleLayerMask;

        public EGStateChase(EnemyBase enemy) : base(enemy)
        {
            _obstacleLayerMask = 1 << LayerMask.NameToLayer("Default");
        }

        public override void OnTick(float dt)
        {
            base.OnTick(dt);

            bool hasTarget = sensor.FindVisibleTargets(out var pos);
            if (!hasTarget)
            {
                Debug.DrawLine(transform.position, pos, Color.blue);
            }

            bool hasValidPath = NavMesh.CalculatePath(transform.position, pos, NavMesh.AllAreas, navMeshPath);
            Vector3 nextPoint = pos;
            if (navMeshPath.corners != null && navMeshPath.corners.Length > 1)
            {
                nextPoint = navMeshPath.corners[1];
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
            
            Vector3 direction;

            if (_obstacleAvoidanceTimer > 0)
            {
                direction = _obstacleAvoidanceDirection;
                _obstacleAvoidanceTimer -= dt;
                Debug.DrawRay(transform.position, _obstacleAvoidanceDirection * 2, Color.red);
            }
            else
            {
                direction = (nextPoint - transform.position).normalized;

                if (DetectObstacles(direction, out Vector3 avoidanceDir))
                {
                    direction = avoidanceDir;
                    _obstacleAvoidanceDirection = avoidanceDir;
                    _obstacleAvoidanceTimer = _obstacleAvoidanceDuration;
                    Debug.DrawRay(transform.position, avoidanceDir * 2, Color.yellow);
                }

                if (!hasValidPath && Vector3.Distance(transform.position, pos) > 3.0f)
                {
                    Vector3 directPathDirection = (pos - transform.position).normalized;
                    if (DetectObstacles(directPathDirection, out Vector3 alternativeDir))
                    {
                        direction = alternativeDir;
                        _obstacleAvoidanceDirection = alternativeDir;
                        _obstacleAvoidanceTimer = _obstacleAvoidanceDuration;
                        Debug.DrawRay(transform.position, alternativeDir * 2, Color.green);
                    }
                }
            }

            eggFighter.rb.linearVelocity = direction * eggFighter.chaseSpeed;
            eggFighter.rb.linearVelocity = Vector3.ProjectOnPlane(eggFighter.rb.linearVelocity, _normal);

            Quaternion rotation = Quaternion.LookRotation(nextPoint - transform.position, Vector3.up);
            transform.rotation = Quaternion.Euler(0, rotation.eulerAngles.y, 0);

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
        
        private bool DetectObstacles(Vector3 direction, out Vector3 avoidanceDirection)
        {
            avoidanceDirection = direction;
            
            const float maxWalkableSlope = 20f;
            
            if (Physics.SphereCast(transform.position, _obstacleDetectionRadius, direction, out RaycastHit hit, _obstacleDetectionDistance, _obstacleLayerMask))
            {
                float surfaceAngle = Vector3.Angle(hit.normal, Vector3.up);
                
                if (surfaceAngle <= maxWalkableSlope)
                {
                    Vector3 slopeDirection = Vector3.ProjectOnPlane(direction, hit.normal).normalized;
                    
                    if (slopeDirection != Vector3.zero)
                    {   
                        avoidanceDirection = slopeDirection;
                    }
                    
                    return false;
                }
                
                Vector3 hitNormal = hit.normal;
                hitNormal.y = 0;
                
                if (hitNormal != Vector3.zero)
                {
                    Debug.DrawRay(hit.point, hit.normal, Color.red, 0.1f);
                    
                    Vector3 right = Vector3.Cross(Vector3.up, direction).normalized;
                    float rightDot = Vector3.Dot(right, hitNormal);
                    
                    if (rightDot > 0)
                    {
                        avoidanceDirection = Vector3.Lerp(direction, right, 0.75f).normalized;
                    }
                    else
                    {
                        avoidanceDirection = Vector3.Lerp(direction, -right, 0.75f).normalized;
                    }
                    
                    return true;
                }
            }
            
            Vector3 rightDir = Vector3.Cross(Vector3.up, direction).normalized;
            if (Physics.Raycast(transform.position, rightDir, out RaycastHit sideHitRight, _obstacleDetectionDistance * 0.7f, _obstacleLayerMask))
            {
                float sideAngle = Vector3.Angle(sideHitRight.normal, Vector3.up);
                if (sideAngle > maxWalkableSlope)
                {
                    avoidanceDirection = Vector3.Lerp(direction, -rightDir, 0.5f).normalized;
                    return true;
                }
            }
            
            if (Physics.Raycast(transform.position, -rightDir, out RaycastHit sideHitLeft, _obstacleDetectionDistance * 0.7f, _obstacleLayerMask))
            {
                float sideAngle = Vector3.Angle(sideHitLeft.normal, Vector3.up);
                if (sideAngle > maxWalkableSlope)
                {
                    avoidanceDirection = Vector3.Lerp(direction, rightDir, 0.5f).normalized;
                    return true;
                }
            }
            
            return false;
        }
    }
}