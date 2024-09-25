using SurgeEngine.Code.ActorSystem;
using UnityEngine;

namespace SurgeEngine.Code.Custom
{
    public static class Common
    {
        public static string GetGroundTag(this GameObject gameObject)
        {
            string input = gameObject.name;
            int index = input.IndexOf('@');
            string result = input.Substring(index + 1);
            
            return result;
        }

        public static bool InDelayTime(float last, float delay)
        {
            return last + delay < Time.time;
        }
        
        public static Vector3 GetCross(Transform transform, float pitch, bool inverse = false)
        {
            Vector3 cross = Vector3.Cross(Vector3.up, inverse ? transform.right : -transform.right);
            cross = Quaternion.AngleAxis(inverse ? pitch : -pitch, transform.right) * cross;
            return cross;
        }

        public static Vector3 GetImpulseWithPitch(Transform transform, float pitch, float impulse)
        {
            Vector3 dir = -transform.forward;
            dir = Quaternion.AngleAxis(pitch, transform.right) * dir;
            Vector3 impulseV = dir * impulse;
            return impulseV;
        }

        public static void ResetVelocity()
        {
            var context = ActorContext.Context;
            context.rigidbody.linearVelocity = Vector3.zero;
            context.rigidbody.angularVelocity = Vector3.zero;
            context.stats.planarVelocity = Vector3.zero;
            context.stats.movementVector = Vector3.zero;
        }
        
        public static void ApplyImpulse(Vector3 impulse)
        {
            var context = ActorContext.Context;
            ResetVelocity();
            
            context.rigidbody.AddForce(impulse, ForceMode.Impulse);
            context.rigidbody.linearVelocity = Vector3.ClampMagnitude(context.rigidbody.linearVelocity, impulse.magnitude);
        }

        public static void ApplyGravity(float yGravity, float dt)
        {
            var context = ActorContext.Context;
            context.rigidbody.linearVelocity += Vector3.down * (yGravity * dt);
        }
        
        public static bool CheckForGround(out RaycastHit result)
        {
            var context = ActorContext.Context;
            return Physics.Raycast(context.transform.position, -context.transform.up, out result,
                context.stats.moveParameters.castParameters.castDistance, context.stats.moveParameters.castParameters.collisionMask, QueryTriggerInteraction.Ignore);
        }

        public static Transform FindHomingTarget()
        {
            var context = ActorContext.Context;
            float distance = float.MaxValue;
            Transform result = null;
            Collider[] colliders = new Collider[20];
            var size = Physics.OverlapSphereNonAlloc(context.transform.position, context.stats.homingParameters.homingFindRadius, colliders,
                context.stats.homingParameters.homingMask);

            if (size > 0)
            {
                foreach (var collider in colliders)
                {
                    if (collider != null)
                    {
                        float newDistance = Vector3.Distance(context.transform.position, collider.transform.position);
                        if (newDistance < distance)
                        {
                            Vector3 dir = (collider.transform.position - context.transform.position);

                            if (Vector3.Dot(dir.normalized, context.transform.forward) > 0.1f)
                            {
                                if (!Physics.Linecast(context.transform.position, collider.transform.position, out var hit, 
                                        context.stats.moveParameters.castParameters.collisionMask))
                                {
                                    result = collider.transform;
                                }
                            }
                        }
                        
                        return result;
                    }
                }
            }

            return null;
        }
    }
}