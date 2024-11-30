using System;
using Cysharp.Threading.Tasks;
using SurgeEngine.Code.ActorStates;
using SurgeEngine.Code.ActorSystem;
using SurgeEngine.Code.CommonObjects;
using SurgeEngine.Code.GameDocuments;
using SurgeEngine.Code.Parameters;
using UnityEngine;
using UnityEngine.Splines;
using static SurgeEngine.Code.GameDocuments.SonicGameDocumentParams;

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
                context.kinematics.PlanarVelocity = Vector3.zero;
                context.kinematics.MovementVector = Vector3.zero;
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

        public static bool CheckForGround(out RaycastHit result, CheckGroundType type = CheckGroundType.Normal, float castDistance = 0f)
        {
            var context = ActorContext.Context;
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
                    direction = context.rigidbody.linearVelocity.normalized;
                    break;
                case CheckGroundType.PredictJump:
                    origin = context.transform.position - context.transform.up * 0.5f;
                    direction = context.rigidbody.linearVelocity.normalized;
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
            
            Debug.DrawLine(origin, origin + direction, Color.red);
            
            Document doc = SonicGameDocument.GetDocument("Sonic");
            ParameterGroup group = doc.GetGroup(SonicGameDocument.CastGroup);
            if (castDistance == 0) castDistance = group.GetParameter<float>(Cast_Distance);
            LayerMask castMask = group.GetParameter<LayerMask>(Cast_Mask);

            bool hit = Physics.Raycast(ray, out result,
                castDistance, castMask, QueryTriggerInteraction.Ignore);
            
            return hit;
        }

        public static bool CheckForRail(out RaycastHit result, out Rail rail)
        {
            var context = ActorContext.Context;
            Vector3 origin = context.transform.position;
            Vector3 direction = -context.transform.up;

            Ray ray = new Ray(origin, direction);
            Document doc = SonicGameDocument.GetDocument("Sonic");
            ParameterGroup group = doc.GetGroup(SonicGameDocument.CastGroup);
            float castDistance = group.GetParameter<float>(Cast_Distance) *
                                 (context.stateMachine.Is<FStateHoming>() ? 1.5f : 1f);
            LayerMask mask = group.GetParameter<LayerMask>(Cast_RailMask);

            if (Physics.SphereCast(ray, 0.4f, out result, castDistance, mask, QueryTriggerInteraction.Collide))
            {
                rail = result.collider.GetComponentInParent<Rail>();
                return true;
            }
            
            rail = null;
            return false;
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
        
        public static HomingTarget FindHomingTarget()
        {
            var context = ActorContext.Context;
            var transform = context.transform;
            var doc = SonicGameDocument.GetDocument("Sonic");
            var param = doc.GetGroup(SonicGameDocument.HomingGroup);
            Vector3 origin = transform.position + Vector3.down;
            Vector3 dir = context.kinematics.GetInputDir() == Vector3.zero ? transform.forward : context.kinematics.GetInputDir();
            var maxDistance = param.GetParameter<float>(Homing_FindDistance);
            var hits = Physics.OverlapSphere(origin + dir, maxDistance, param.GetParameter<LayerMask>(Homing_Mask), QueryTriggerInteraction.Collide);
            HomingTarget closestTarget = null;
            float closestDistance = float.MaxValue;
            foreach (var hit in hits)
            {
                Transform target = hit.transform;
                Vector3 end = target.position + Vector3.up * 0.5f;
                Vector3 direction = target.position - origin;
                bool facing = Vector3.Dot(direction.normalized, transform.forward) > 0.5f;
                if (facing && !Physics.Linecast(origin, end, doc.GetGroup(SonicGameDocument.CastGroup).GetParameter<LayerMask>(Cast_Mask)))
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
        Normal,
        DefaultDown,
        Predict,
        PredictJump,
        PredictOnRail,
        PredictEdge
    }
}