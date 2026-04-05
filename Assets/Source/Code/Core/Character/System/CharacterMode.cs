using System;
using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.System;
using SurgeEngine.Source.Code.Infrastructure.Config;
using SurgeEngine.Source.Code.Infrastructure.Custom.Extensions;
using UnityEngine;
using UnityEngine.Splines;
using Zenject;

namespace SurgeEngine.Source.Code.Core.Character.System
{
    public class CharacterMode : MonoBehaviour, IPointMarkerLoader
    {
        [Inject] private CharacterBase _character;
        
        private CharacterKinematics Kinematics => _character.Kinematics;
        private Rigidbody Rigidbody => _character.Rigidbody;
        
        public ChangeMode2DData Path2D { get; private set; }
        public ChangeMode3DData PathForward { get; private set; }
        public ChangeMode3DData PathDash { get; private set; }
        public event Action<ChangeMode2DData> OnPath2DChange;
        public event Action<ChangeMode3DData> OnPathForwardChange;
        public event Action<ChangeMode3DData> OnPathDashChange;
        private float _lastRelativeTime;
        private Vector3 _lastTangent;
        
        private SplineContainer[] _allSideSplines;
        private readonly float _sideSplineSearchRadius = 5f;

        public PointSample SideSample { get; private set; }
        
        private PhysicsConfig Config => _character.Config;

        private void Awake()
        {
            var allContainers = FindObjectsByType<SplineContainer>(FindObjectsSortMode.None);
            _allSideSplines = Array.FindAll(allContainers, c => c.CompareTag("Splines/SideView"));
        }

        private void FixedUpdate()
        {
            CalculatePath2D();
            CalculatePathForward();
            CalculatePathDash();
        }
        
        private void CalculatePath2D()
        {
            if (Path2D != null && Path2D.Tag == SplineTag.SideView)
            {
                var path = Path2D.Spline;
                Vector3 rbPos = Rigidbody.position - transform.up;

                SearchSideSplines(rbPos);
                
                SideSample = path.EvaluateRelative(rbPos, _lastRelativeTime);
                _lastRelativeTime = SideSample.Time;
                if (SideSample.Right != Vector3.zero)
                {
                    Kinematics.Project(SideSample.Right);
                }
                
                if (_lastTangent == Vector3.zero) _lastTangent = SideSample.Tangent;
                
                float sign = Mathf.Sign(Vector3.Dot(Rigidbody.transform.forward, SideSample.Tangent));
                float pathEaseTime = Path2D.PathEaseTime;
                if (_character.Flags.GetFlag<AutorunFlag>(out var autoRunFlag) && autoRunFlag.PathEaseTime > 0)
                {
                    pathEaseTime = autoRunFlag.PathEaseTime;
                }
                
                if (Kinematics.Speed > 0.02f && _character.Flags.HasFlag(FlagType.Autorun))
                {
                    var rotTarget = Quaternion.LookRotation(SideSample.Tangent * sign, Kinematics.Normal);
                    Rigidbody.MoveRotation(Quaternion.RotateTowards(Rigidbody.rotation, rotTarget, 720 * Time.fixedDeltaTime));
                }

                Vector3 target;
                Vector3 newPos = SideSample.Position;
                Vector3 physicsTarget = newPos + Vector3.ProjectOnPlane(Rigidbody.position - newPos, SideSample.Right);
                if (pathEaseTime > 0f)
                {
                    Path2D.CurrentEaseTime += Time.fixedDeltaTime / pathEaseTime;
                    Path2D.CurrentEaseTime = Mathf.Clamp01(Path2D.CurrentEaseTime);
                    Path2D.StartPosition += Kinematics.Velocity * Time.fixedDeltaTime;
                    
                    target = Vector3.Lerp(Path2D.StartPosition, physicsTarget, Path2D.CurrentEaseTime);
                }
                else
                {
                    target = physicsTarget;
                }
                
                Rigidbody.MovePosition(target);
            }
        }

        private void CalculatePathForward()
        {
            if (PathForward != null)
            {
                AdjustVelocityForPath(PathForward);
            }
        }

        private void CalculatePathDash()
        {
            if (PathDash != null)
            {
                AdjustVelocityForPath(PathDash);
            }
        }

        private void AdjustVelocityForPath(ChangeMode3DData data)
        {
            var force = data.PathCorrectionForce;
            if (force > 0)
            {
                if (Kinematics.CheckForGround(out _))
                {
                    var sample = data.Spline.EvaluateNearest(Rigidbody.position);
                    var tg = sample.Tangent;
                    
                    if (data.IsLimitEdge)
                    {
                        if (IsPathOutOfRange(sample.Time))
                        {
                            SetForwardPath(null);
                            SetDashPath(null);
                            return;
                        }
                    }

                    var velocity = Kinematics.Velocity;
                    float dot = Vector3.Dot(velocity.normalized, tg);
                    float sign = Mathf.Sign(dot);
                    tg = sign * tg;
                    
                    var targetDir = tg.normalized * velocity.magnitude;
                    var horizontalVelocity = Kinematics.HorizontalVelocity;
                    float speedFactor = Mathf.Clamp01(1f - horizontalVelocity.magnitude / (Config.maxSpeed * 2f));
                    float adjustedForce = force * speedFactor;

                    if (horizontalVelocity.magnitude > 0.1f)
                    {
                        var rotation = Quaternion.RotateTowards(Quaternion.LookRotation(horizontalVelocity), 
                            Quaternion.LookRotation(targetDir), adjustedForce * Mathf.Rad2Deg * Time.fixedDeltaTime);
                        Rigidbody.linearVelocity = rotation * Vector3.forward * horizontalVelocity.magnitude + Kinematics.VerticalVelocity;
                    }
                }
            }
        }

        private void SearchSideSplines(Vector3 position)
        {
            if (_allSideSplines == null) return;

            SplineContainer bestContainer = null;
            float bestDist = _sideSplineSearchRadius * _sideSplineSearchRadius;

            foreach (var container in _allSideSplines)
            {
                if (container == null) continue;
                
                var bounds = container.Spline.GetBounds();
                var worldBounds = new Bounds(
                    container.transform.TransformPoint(bounds.center),
                    container.transform.TransformVector(bounds.size).Abs());

                if (worldBounds.SqrDistance(position) > bestDist) continue;

                SplineUtility.GetNearestPoint(
                    container.Spline,
                    container.transform.InverseTransformPoint(position),
                    out var nearestLocal, out _,
                    resolution: 4, iterations: 2);

                float dist = (container.transform.TransformPoint(nearestLocal) - position).sqrMagnitude;
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestContainer = container;
                }
            }
            
            if (bestContainer != null)
            {
                Path2D.Spline.UpdateContainer(bestContainer);
                
                SplineUtility.GetNearestPoint(
                    bestContainer.Spline, 
                    bestContainer.transform.InverseTransformPoint(position), 
                    out _, out _lastRelativeTime);
            }
        }

        public void Set2DPath(ChangeMode2DData data)
        {
            if (Path2D != null && data != null && Path2D.Spline.Container == data.Spline.Container)
                return;

            UpdateRelativeTime(data);
            Path2D = data;
            _lastTangent = Vector3.zero;
            OnPath2DChange?.Invoke(data);

            if (PathForward != null)
            {
                SetForwardPath(null);
            }

            if (PathDash != null)
            {
                SetDashPath(null);
            }
        }

        public void SetForwardPath(ChangeMode3DData data)
        {
            if (PathForward != null && data != null && PathForward.Spline.Container == data.Spline.Container)
                return;
            
            UpdateRelativeTime(data);
            PathForward = data;
            OnPathForwardChange?.Invoke(data);

            if (Path2D != null)
            {
                Set2DPath(null);
            }

            if (PathDash != null)
            {
                SetDashPath(null);
            }
        }

        public void SetDashPath(ChangeMode3DData data)
        {
            if (PathDash != null && data != null && PathDash.Spline.Container == data.Spline.Container)
                return;
            
            UpdateRelativeTime(data);
            PathDash = data;
            OnPathDashChange?.Invoke(data);
            
            if (Path2D != null)
            {
                Set2DPath(null);
            }
            
            if (PathForward != null)
            {
                SetForwardPath(null);
            }
        }

        private void UpdateRelativeTime(ChangeModeData data)
        {
            if (data != null)
            {
                SplineUtility.GetNearestPoint(data.Spline.Container.Spline, 
                    data.Spline.Container.transform.InverseTransformPoint(Rigidbody.position), out _, out _lastRelativeTime);
            }
        }

        public void Load()
        {
            Set2DPath(null);
            SetForwardPath(null);
            SetDashPath(null);
        }

        private static bool IsPathOutOfRange(float t) => t >= 1f || t <= 0;
    }
}