using System;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Core.Actor.System;
using SurgeEngine.Code.Gameplay.CommonObjects.System;
using UnityEngine;

namespace SurgeEngine.Code.Infrastructure.Custom
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
        
        public static GroundTag GetGroundTag(this GameObject gameObject)
        {
            string input = gameObject.name;
            int index = input.IndexOf('@');
            string result = index == -1 ? "Concrete" : input.Substring(index + 1);
            GroundTag tag = (GroundTag)Enum.Parse(typeof(GroundTag), result);
            
            return tag;
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