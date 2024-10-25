using System;
using Cysharp.Threading.Tasks;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.GameDocuments;
using UnityEngine;

namespace SurgeEngine.Code.Custom
{
    public static class Common
    {
        public static bool AddScore(int score)
        {
            int abs = Math.Abs(score);
            if (abs > 0)
            {
                Stage.Instance.data.AddScore(abs);
                return true;
            }
            
            return false;
        }
        
        public static string GetGroundTag(this GameObject gameObject)
        {
            string input = gameObject.name;
            int index = input.IndexOf('@');
            string result;
            if (index == -1)
            {
                result = "Concrete";
            }
            else
            {
                result = input.Substring(index + 1);
            }
            
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
            var context = ActorContext.Context;

            if (context.rigidbody.isKinematic) return;
            
            if (type is ResetVelocityType.Linear or ResetVelocityType.Both)
            {
                context.stats.planarVelocity = Vector3.zero;
                context.stats.movementVector = Vector3.zero;
                context.rigidbody.linearVelocity = Vector3.zero;
            }
            if (type is ResetVelocityType.Angular or ResetVelocityType.Both) context.rigidbody.angularVelocity = Vector3.zero;
        }
        
        public static void ApplyImpulse(Vector3 impulse, ResetVelocityType type = ResetVelocityType.Both)
        {
            var context = ActorContext.Context;
            ResetVelocity(type);
            
            context.rigidbody.AddForce(impulse, ForceMode.Impulse);
            context.rigidbody.linearVelocity = Vector3.ClampMagnitude(context.rigidbody.linearVelocity, impulse.magnitude);
        }

        public static void ApplyGravity(float yGravity, float dt)
        {
            var context = ActorContext.Context;
            if (!context.rigidbody.isKinematic) context.rigidbody.linearVelocity += Vector3.down * (yGravity * dt);
        }
        
        public static bool CheckForGround(out RaycastHit result, CheckGroundType type = CheckGroundType.Default)
        {
            var context = ActorContext.Context;
            Vector3 origin;
            Vector3 direction;
            switch (type)
            {
                case CheckGroundType.Default:
                    origin = context.transform.position;
                    direction = -context.transform.up;
                    break;
                case CheckGroundType.Predict:
                    origin = context.transform.position;
                    direction = context.rigidbody.linearVelocity.normalized;
                    break;
                case CheckGroundType.PredictJump:
                    origin = context.transform.position - context.transform.up * 0.5f;
                    direction = context.rigidbody.linearVelocity.normalized;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            Ray ray = new Ray(origin, direction);
            Debug.DrawRay(ray.origin, ray.direction);
            
            Document doc = SonicGameDocument.Instance.GetDocument("Sonic");
            ParameterGroup group = doc.GetGroup(SonicGameDocument.CastGroup);
            float castDistance = group.GetParameter<float>("CastDistance");
            LayerMask castMask = group.GetParameter<LayerMask>("CastMask");

            return Physics.Raycast(ray, out result,
                castDistance, castMask, QueryTriggerInteraction.Ignore);
        }

        public static async UniTask TemporarilyDisableCollider(Collider collider, float time = 0.25f)
        {
            collider.enabled = false;
            await UniTask.Delay(TimeSpan.FromSeconds(time), true);
            collider.enabled = true;
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
        public static async UniTask ChangeRigidbodyPositionOverTime(Rigidbody rigidbody, Vector3 targetPosition, float duration)
        {
            Vector3 startPosition = rigidbody.position;
            float elapsed = 0f;
            rigidbody.isKinematic = true;
            
            while (elapsed < duration)
            {
                rigidbody.position = Vector3.Lerp(startPosition, targetPosition, elapsed / duration);
                elapsed += Time.unscaledDeltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update);
            }
            
            rigidbody.isKinematic = false;
            ResetVelocity(ResetVelocityType.Both);
        }
        
        public static Transform FindHomingTarget()
        {
            var context = ActorContext.Context;
            var camera = context.camera.GetCamera();
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
                            Vector3 dir = (collider.transform.position - context.transform.position).normalized;

                            var pointOnScreen = camera.WorldToViewportPoint(collider.transform.position);
                            if (pointOnScreen.x > 0 && pointOnScreen is { x: < 1, y: > 0 } && pointOnScreen.y < 1)
                            {
                                if (Vector3.Dot(dir, context.transform.forward) > 0.1f)
                                {
                                    if (!Physics.Linecast(context.transform.position, collider.transform.position + collider.transform.up * 0.2f, out _, 
                                            context.stats.moveParameters.castParameters.collisionMask))
                                    {
                                        result = collider.transform;
                                    }
                                }
                            }
                        }
                        
                        return result;
                    }
                }
            }

            return null;
        }
        
        public static async UniTask ChangeFOVOverTime(float targetFov, float duration)
        {
            var camera = ActorContext.Context.camera;
            float startFov = camera.GetCamera().fieldOfView;
            float elapsed = 0f;
            
            while (elapsed < duration)
            {
                camera.GetCamera().fieldOfView = Mathf.Lerp(startFov, targetFov, elapsed / duration);
                elapsed += Time.deltaTime;
                await UniTask.Yield();
            }
            
            camera.GetCamera().fieldOfView = targetFov;
        }
    }

    public enum ResetVelocityType
    {
        None,
        Angular,
        Linear,
        Both
    }

    public enum CheckGroundType
    {
        Default,
        Predict,
        PredictJump
    }
}