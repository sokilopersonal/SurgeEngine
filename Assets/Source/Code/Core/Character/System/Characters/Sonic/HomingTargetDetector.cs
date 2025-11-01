using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility.Rails;
using SurgeEngine.Source.Code.Infrastructure.Config.SonicSpecific;
using SurgeEngine.Source.Code.Infrastructure.Custom.Extensions;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Source.Code.Core.Character.System.Characters.Sonic
{
    public class HomingTargetDetector : MonoBehaviour
    {
        [SerializeField] private HomingConfig config;

        private CharacterBase _character;

        public HomingTarget Target { get; private set; }

        private void Awake()
        {
            _character = GetComponent<CharacterBase>();
        }

        private void FixedUpdate()
        {
            if (_character.Kinematics.InAir)
            {
                Target = FindHomingTarget(config.findDistance, config.findAngle, config.mask, _character.Config.castLayer);
            }
            else
            {
                Target = null;
            }
        }

        private HomingTarget FindHomingTarget(float radius, float angle, LayerMask mask, LayerMask blockMask)
        {
            mask |= _character.Config.railMask;
            Collider[] colliders = Physics.OverlapSphere(transform.position, radius, mask, QueryTriggerInteraction.Collide);

            HomingTarget bestTarget = null;
            float bestDistance = Mathf.Infinity;

            Vector3 forwardFlat = transform.forward;
            forwardFlat.y = 0f;
            forwardFlat.Normalize();

            foreach (var col in colliders)
            {
                HomingTarget target = col.GetComponent<HomingTarget>();
                if (target == null) continue;

                Vector3 targetPos = col.transform.position;

                if (target.TryGetComponentInParent(out Rail rail))
                {
                    var spline = rail.Container.Spline;
                    var railTarget = rail.HomingTarget;
                    Vector3 localPos = rail.transform.InverseTransformPoint(transform.position + transform.forward * radius / 2);
                    SplineUtility.GetNearestPoint(spline, localPos, out _, out var f);

                    SplineSample sample = new SplineSample
                    {
                        pos = spline.EvaluatePosition(f),
                        tg = ((Vector3)spline.EvaluateTangent(f)).normalized,
                        up = spline.EvaluateUpVector(f)
                    };

                    Vector3 endTargetPos = sample.pos + sample.up * (rail.Radius + 0.5f);
                    targetPos = rail.transform.TransformPoint(endTargetPos);

                    railTarget.transform.position = Vector3.Lerp(railTarget.transform.position, targetPos, 32 * Time.fixedDeltaTime);
                    target = railTarget;
                }

                Vector3 dir = targetPos - transform.position;
                Vector3 dirFlat = new Vector3(dir.x, 0f, dir.z).normalized;

                float dot = Vector3.Dot(forwardFlat, dirFlat);
                if (dot < Mathf.Cos(angle * 0.5f * Mathf.Deg2Rad)) continue;

                float dist = dir.magnitude;
                if (dist < bestDistance && !Physics.Linecast(transform.position, targetPos + Vector3.up * 0.5f, blockMask))
                {
                    bestDistance = dist;
                    bestTarget = target;
                }
            }

            return bestTarget;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(Vector3.zero, config.findDistance);
            
            Vector3 forward = Vector3.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 rightBoundary = Quaternion.Euler(0f, config.findAngle * 0.5f, 0f) * forward;
            Vector3 leftBoundary = Quaternion.Euler(0f, -config.findAngle * 0.5f, 0f) * forward;

            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(Vector3.zero, Vector3.zero + rightBoundary * config.findDistance);
            Gizmos.DrawLine(Vector3.zero, Vector3.zero + leftBoundary * config.findDistance);
        }
    }
}