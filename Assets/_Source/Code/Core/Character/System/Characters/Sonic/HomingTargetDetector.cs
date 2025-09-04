using SurgeEngine._Source.Code.Core.Character.States;
using SurgeEngine._Source.Code.Gameplay.CommonObjects;
using SurgeEngine._Source.Code.Gameplay.CommonObjects.Mobility.Rails;
using SurgeEngine._Source.Code.Infrastructure.Config.SonicSpecific;
using UGizmo;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine._Source.Code.Core.Character.System.Characters.Sonic
{
    public class HomingTargetDetector : MonoBehaviour
    {
        [SerializeField] private HomingConfig config;
        [SerializeField] private Vector3 size = Vector3.one;
        
        private CharacterBase _character;

        public HomingTarget Target { get; private set; }

        private void Awake()
        {
            _character = GetComponent<CharacterBase>();
        }

        private void FixedUpdate()
        {
            if (_character.StateMachine.CurrentState is FStateAir && !_character.Flags.HasFlag(FlagType.OutOfControl))
            {
                Target = FindHomingTarget();
            }
            else
            {
                Target = null;
            }
        }

        private HomingTarget FindHomingTarget()
        {
            Transform characterTransform = _character.transform;
            Vector3 origin = characterTransform.position + Vector3.down / 2;
            Vector3 dir = _character.Kinematics.GetInputDir() == Vector3.zero ? characterTransform.forward : _character.Kinematics.GetInputDir();
            
            float maxDistance = config.findDistance;
            LayerMask mask = config.mask;
            mask |= _character.Config.railMask;

            Vector3 finalOrigin = origin + dir * maxDistance / 2;
            Quaternion orientation = Quaternion.LookRotation(dir, Vector3.up);
            Collider[] hits = 
                Physics.OverlapBox(finalOrigin, 
                    size, orientation, mask, QueryTriggerInteraction.Collide);

            if (Debug.isDebugBuild)
            {
                UGizmos.DrawWireCube(finalOrigin, orientation, size, Color.green);
            }
            
            HomingTarget closestTarget = null;
            float closestDistance = float.MaxValue;
            foreach (Collider hit in hits)
            {
                Transform target = hit.transform;
                Vector3 end = target.position + Vector3.up * 0.5f;
                Vector3 direction = target.position - origin;
                float distance = direction.magnitude;
                
                if (target.TryGetComponent(out Rail rail))
                {
                    var spline = rail.Container.Spline;
                    var railTarget = rail.HomingTarget;
                    Vector3 localPos = rail.transform.InverseTransformPoint(origin + characterTransform.forward * maxDistance / 2);
                    SplineUtility.GetNearestPoint(spline, localPos, out _, out var f);
                        
                    SplineSample sample = new SplineSample
                    {
                        pos = spline.EvaluatePosition(f),
                        tg = ((Vector3)spline.EvaluateTangent(f)).normalized,
                        up = spline.EvaluateUpVector(f)
                    };
                    
                    Vector3 endTargetPos = sample.pos + sample.up * (rail.Radius + 0.5f);
                    Vector3 endWorldPos = rail.transform.TransformPoint(endTargetPos);
                    railTarget.transform.position = Vector3.Lerp(railTarget.transform.position, endWorldPos, 32 * Time.fixedDeltaTime);
                    
                    if (distance < closestDistance)
                    {
                        closestTarget = railTarget;
                        closestDistance = distance;
                    }
                }
                
                bool facing = Vector3.Dot(direction.normalized, characterTransform.forward) > 0.5f;
                if (Debug.isDebugBuild)
                {
                    UGizmos.Linecast(origin, end, _character.Config.castLayer);
                }
                if (facing && !Physics.Linecast(origin, end, _character.Config.castLayer))
                {
                    if (target.TryGetComponent(out HomingTarget homingTarget))
                    {
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