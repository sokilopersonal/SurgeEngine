using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using SurgeEngine.Code.Actor.States.SonicSpecific;
using SurgeEngine.Code.Actor.System;
using SurgeEngine.Code.CommonObjects;
using UnityEngine;

namespace SurgeEngine.Code.Custom
{
    public static class Common
    {
        public static Coroutine colliderCoroutine;
        
        public static bool AddScore(int score)
        {
            int abs = Mathf.Abs(score);
            if (abs > 0)
            {
                Stage.Instance.data.AddScore(abs);
                return true;
            }
            
            return false;
        }

        /// <summary>
        /// Start a timer with the specified values
        /// </summary>
        /// <param name="waitTimer">Current timer value.</param>
        /// <param name="waitTime">Maximum timer value.</param>
        /// <param name="autoReset">True,  if you prefer the timer to reset automatically.</param>
        /// <returns>True, when the timer value is 0</returns>
        public static bool TickTimer(ref float waitTimer, float waitTime, bool autoReset = true, bool unscaled = false) 
        {
            if (waitTimer == 0) 
            {
                if (autoReset)
                    waitTimer = waitTime;
                return true;
            }

            waitTimer -= unscaled ? Time.unscaledDeltaTime : Time.deltaTime;
            waitTimer = Mathf.Clamp(waitTimer, 0, waitTime);
            return false;
        }
        
        public static string GetGroundTag(this GameObject gameObject)
        {
            string input = gameObject.name;
            int index = input.IndexOf('@');
            string result = index == -1 ? "Concrete" : input.Substring(index + 1);
            
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

        public static Vector3 GetImpulseWithPitch(Vector3 forward, Vector3 right, float pitch, float impulse)
        {
            Vector3 dir = forward;
            dir = Quaternion.AngleAxis(pitch, right) * dir;
            Vector3 impulseV = dir * impulse;
            return impulseV;
        }

        public static void ResetVelocity(ResetVelocityType type)
        {
            ActorBase context = ActorContext.Context;

            if (context.kinematics.Rigidbody.isKinematic) return;
            
            if (type is ResetVelocityType.Linear or ResetVelocityType.Both)
            {
                context.kinematics.PlanarVelocity = Vector3.zero;
                context.kinematics.MovementVector = Vector3.zero;
                context.kinematics.Rigidbody.linearVelocity = Vector3.zero;
            }
            if (type is ResetVelocityType.Angular or ResetVelocityType.Both) context.kinematics.Rigidbody.angularVelocity = Vector3.zero;
        }
        
        public static void ApplyImpulse(Vector3 impulse, ResetVelocityType type = ResetVelocityType.Both)
        {
            ActorBase context = ActorContext.Context;
            ResetVelocity(type);
            
            context.kinematics.Rigidbody.linearVelocity = impulse;
            context.kinematics.Rigidbody.linearVelocity = Vector3.ClampMagnitude(context.kinematics.Rigidbody.linearVelocity, impulse.magnitude);
        }

        public static void ApplyGravity(float yGravity, float dt)
        {
            ActorBase context = ActorContext.Context;
            if (!context.kinematics.Rigidbody.isKinematic) context.kinematics.Rigidbody.linearVelocity += Vector3.down * (yGravity * dt);
        }

        public static bool CheckForGround(out RaycastHit result, CheckGroundType type = CheckGroundType.Normal, float castDistance = 0f)
        {
            ActorBase context = ActorContext.Context;
            Vector3 origin;
            Vector3 direction;
            switch (type)
            {
                case CheckGroundType.Normal:
                    origin = context.transform.position;
                    direction = -context.kinematics.Normal;
                    break;
                case CheckGroundType.DefaultDown:
                    origin = context.transform.position;
                    direction = -context.transform.up;
                    break;
                case CheckGroundType.Predict:
                    origin = context.transform.position;
                    direction = context.kinematics.Rigidbody.linearVelocity.normalized;
                    break;
                case CheckGroundType.PredictJump:
                    origin = context.transform.position - context.transform.up * 0.5f;
                    direction = context.kinematics.Rigidbody.linearVelocity.normalized;
                    break;
                case CheckGroundType.PredictOnRail:
                    origin = context.transform.position + context.transform.forward;
                    direction = -context.kinematics.Normal;
                    break;
                case CheckGroundType.PredictEdge:
                    origin = context.transform.position + Vector3.ClampMagnitude(context.kinematics.PlanarVelocity * 0.075f, 1f);
                    direction = -context.kinematics.Normal;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            Ray ray = new Ray(origin, direction);
            
            Debug.DrawLine(origin, origin + direction * castDistance, Color.red);
            
            if (castDistance == 0) castDistance = context.config.castDistance;
            LayerMask castMask = context.config.castLayer;

            bool hit = Physics.Raycast(ray, out result,
                castDistance, castMask, QueryTriggerInteraction.Ignore);
            
            return hit;
        }

        public static bool CheckForGroundWithDirection(out RaycastHit result, Vector3 direction,
            float castDistance = 0f)
        {
            ActorBase context = ActorContext.Context;
            Vector3 origin = context.transform.position;
            
            if (castDistance == 0) castDistance = context.config.castDistance;
            
            Ray ray = new Ray(origin, direction);
            LayerMask castMask = context.config.castLayer;
            bool hit = Physics.Raycast(ray, out result, castDistance, castMask, QueryTriggerInteraction.Ignore);
            return hit;
        }

        public static bool CheckForRail(out RaycastHit result, out Rail rail)
        {
            ActorBase context = ActorContext.Context;
            Vector3 origin = context.transform.position;
            Vector3 direction = -context.transform.up;

            Ray ray = new Ray(origin, direction);
            float castDistance = context.config.castDistance *
                                 (context.stateMachine.IsExact<FStateHoming>() ? 1.5f : 1f);
            LayerMask mask = context.config.railMask;

            if (Physics.SphereCast(ray, 0.4f, out result, castDistance, mask, QueryTriggerInteraction.Collide))
            {
                rail = result.collider.GetComponentInParent<Rail>();
                return true;
            }
            
            rail = null;
            return false;
        }
        
        public static async UniTask ChangeTimeScaleOverTime(float targetScale, float duration)
        {
            float startScale = Time.timeScale;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                Time.timeScale = Mathf.Lerp(startScale, targetScale, elapsed / duration);
                elapsed += Time.unscaledDeltaTime;
                await UniTask.Yield();
            }
            
            Time.timeScale = targetScale;
        }
    }

    public enum ResetVelocityType
    {
        Angular,
        Linear,
        Both
    }

    public enum CheckGroundType
    {
        Normal,
        DefaultDown,
        Predict,
        PredictJump,
        PredictOnRail,
        PredictEdge
    }
}