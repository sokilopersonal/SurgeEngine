using SurgeEngine.Code.ActorStates.SonicSubStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.ActorSystem.Actors;
using SurgeEngine.Code.CommonObjects;
using UnityEngine;

namespace SurgeEngine.Code.Tools
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
            var transform = _sonic.transform;
            var config = _sonic.homingConfig;
            Vector3 origin = transform.position + Vector3.down;
            Vector3 dir = _sonic.kinematics.GetInputDir() == Vector3.zero ? transform.forward : _sonic.kinematics.GetInputDir();
            
            var maxDistance = config.findDistance;
            var hits = Physics.OverlapSphere(origin + dir, maxDistance, config.mask, QueryTriggerInteraction.Collide);
            
            HomingTarget closestTarget = null;
            float closestDistance = float.MaxValue;
            foreach (var hit in hits)
            {
                Transform target = hit.transform;
                Vector3 end = target.position + Vector3.up * 0.5f;
                Vector3 direction = target.position - origin;
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