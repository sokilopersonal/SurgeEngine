using SurgeEngine.Code.Core.Actor.States.Characters.Sonic.SubStates;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Core.Actor.System.Characters.Sonic;
using SurgeEngine.Code.Gameplay.CommonObjects;
using SurgeEngine.Code.Gameplay.CommonObjects.Mobility;
using SurgeEngine.Code.Infrastructure.Config.SonicSpecific;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Infrastructure.Tools
{
    /// <summary>
    /// A helper class with methods for Sonic
    /// </summary>
    public static class SonicTools
    {
        private static Sonic _sonic => ActorContext.Context as Sonic;
        
        public static bool IsBoost()
        {
            return _sonic.stateMachine.GetSubState<FBoost>().Active;
        }

        public static HomingTarget FindHomingTarget()
        {
            Transform transform = _sonic.transform;
            HomingConfig config = _sonic.homingConfig;
            Vector3 origin = transform.position + Vector3.down;
            Vector3 dir = _sonic.kinematics.GetInputDir() == Vector3.zero ? transform.forward : _sonic.kinematics.GetInputDir();
            
            float maxDistance = config.findDistance;
            LayerMask mask = config.mask;
            mask |= 1 << LayerMask.NameToLayer("Rail");
            
            Collider[] hits = Physics.OverlapSphere(origin + dir, maxDistance, mask, QueryTriggerInteraction.Collide);
            
            HomingTarget closestTarget = null;
            float closestDistance = float.MaxValue;
            foreach (Collider hit in hits)
            {
                Transform target = hit.transform;
                Vector3 end = target.position + Vector3.up * 0.5f;
                Vector3 direction = target.position - origin;
                
                if (target.TryGetComponent(out Rail rail))
                {
                    var spline = rail.Container.Spline;
                    var railTarget = rail.HomingTarget;
                    Vector3 localPos = rail.transform.InverseTransformPoint(origin + transform.forward * maxDistance / 2);
                    SplineUtility.GetNearestPoint(spline, localPos, out _, out var f);
                        
                    SplineSample sample = new SplineSample
                    {
                        pos = spline.EvaluatePosition(f),
                        tg = ((Vector3)spline.EvaluateTangent(f)).normalized,
                        up = spline.EvaluateUpVector(f)
                    };

                    float dot = Vector3.Dot(dir, sample.tg);
                    float sign = Mathf.Sign(dot);
                        
                    Vector3 plane = Vector3.Cross(sample.tg, Vector3.up);
                    Debug.DrawRay(rail.transform.TransformPoint(sample.pos), sample.tg, Color.red, 2f);
                    Vector3 endTargetPos = sample.pos + sample.up * (rail.Radius + 0.35f);
                    Vector3 endWorldPos = rail.transform.TransformPoint(endTargetPos);
                    railTarget.transform.position = Vector3.Lerp(railTarget.transform.position, endWorldPos, 32 * Time.fixedDeltaTime);
                        
                    closestTarget = railTarget;
                    closestDistance = Vector3.Distance(origin, railTarget.transform.position);
                }
                
                bool facing = Vector3.Dot(direction.normalized, transform.forward) > 0.5f;
                if (facing && !Physics.Linecast(origin, end, _sonic.config.castLayer))
                {
                    if (target.TryGetComponent(out HomingTarget homingTarget))
                    {
                        float distance = Vector3.Distance(origin, target.position);
                        if (distance < closestDistance)
                        {
                            closestTarget = homingTarget;
                            closestDistance = distance;
                        }
                    }
                }
            }
    
            return closestTarget;
        }
    }
}