using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.CameraObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Environment;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace SurgeEngine.Source.Code.Infrastructure.Custom
{
    public static class Utility
    {
        public static void AddScore(int score)
        {
            int abs = Mathf.Abs(score);
            if (abs > 0)
            {
                Stage.Instance.Data.AddScore(abs);
            }
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
            
            if (Enum.TryParse(typeof(GroundTag), result, out object tag))
            {
                return (GroundTag)tag;
            }
            
            return GroundTag.Concrete;
        }

        public static bool IsWater(this Transform transform, out WaterSurface waterSurface)
        {
            if (transform == null)
            {
                waterSurface = null;
                return false;
            }
            
            bool isWater = false;
            if (transform.gameObject.GetGroundTag() == GroundTag.Water)
                isWater = true;
            
            if (isWater && transform.TryGetComponent(out waterSurface))
            {
                return true;
            }
            
            waterSurface = null;
            return false;
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
        
        public static void MoveToPosition(MonoBehaviour runner, Rigidbody body, Vector3 targetPosition, float duration = 0.2f)
        {
            runner.StartCoroutine(MoveToPositionVelocityRoutine(body, targetPosition, duration));
        }

        public static void MoveToPosition(MonoBehaviour runner, Rigidbody body, Vector3 targetPosition,
            Vector3 velocity, float duration = 0.2f)
        {
            runner.StartCoroutine(MoveToPositionRoutine(body, targetPosition, velocity, duration));
        }

        private static IEnumerator MoveToPositionVelocityRoutine(Rigidbody body, Vector3 targetPosition, float duration)
        {
            Vector3 startPos = body.position;
            Vector3 endPos = targetPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                startPos += body.linearVelocity * Time.deltaTime;
                endPos += body.linearVelocity * Time.deltaTime;

                body.MovePosition(Vector3.Lerp(startPos, endPos, t));
                yield return null;
            }

            body.MovePosition(endPos);
        }

        private static IEnumerator MoveToPositionRoutine(Rigidbody body, Vector3 targetPosition, Vector3 velocity, float duration)
        {
            Vector3 startPos = body.position;
            Vector3 endPos = targetPosition;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                
                startPos += velocity * Time.deltaTime;
                endPos += velocity * Time.deltaTime;

                body.MovePosition(Vector3.Lerp(startPos, endPos, t));
                yield return null;
            }

            body.MovePosition(endPos);
        }

        public static bool IsObjectInView(this Camera camera, Transform obj)
        {
            Vector3 viewport = camera.WorldToViewportPoint(obj.position);
            return viewport.x > 0 && viewport.x < 1 && viewport.y > 0 && viewport.y < 1 && viewport.z > 0;
        }

        public static List<ChangeCameraVolume> GetVolumesInBounds(Vector3 position)
        {
            return Object.FindObjectsByType<ChangeCameraVolume>(FindObjectsInactive.Include, FindObjectsSortMode.None)
                .Where(volume => volume.GetComponent<BoxCollider>().bounds.Contains(position)).ToList();
        }
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