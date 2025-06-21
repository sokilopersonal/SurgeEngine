using System;
using SurgeEngine.Code.Core.Actor.States;
using SurgeEngine.Code.Gameplay.CommonObjects.System;
using UnityEngine;

namespace SurgeEngine.Code.Infrastructure.Custom
{
    public static class Utility
    {
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
            
            if(Enum.TryParse(typeof(GroundTag), result, out object tag))
            {
                return (GroundTag)tag;
            }
            
            return GroundTag.Concrete;
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

        public static bool IsObjectInView(this Camera camera, Transform obj)
        {
            Vector3 viewport = camera.WorldToViewportPoint(obj.position);
            return viewport.x > 0 && viewport.x < 1 && viewport.y > 0 && viewport.y < 1 && viewport.z > 0;
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