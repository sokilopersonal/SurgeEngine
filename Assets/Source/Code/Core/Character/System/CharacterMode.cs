using System;
using System.Collections.Generic;
using SurgeEngine.Source.Code.Core.Character.States;
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

        public ChangeMode2DData ModeSide { get; private set; }
        public ChangeMode3DData ModeForward { get; private set; }
        public ChangeMode3DData ModeDash { get; private set; }
        public event Action<ChangeMode2DData> OnMode2DChange;
        public event Action<ChangeMode3DData> OnModeForwardChange;
        public event Action<ChangeMode3DData> OnModeDashChange;

        private float _lastRelativeTime;
        private Vector3 _lastTangent;

        private List<HESpline> _allSplines;
        private Dictionary<SplineTag, List<HESpline>> _splinesByTag;
        private Dictionary<HESpline, Bounds> _cachedBounds;

        private readonly float _sideSplineSearchRadius = 16f;

        public PointSample SideSample { get; private set; }
        public PointSample ForwardSample { get; private set; }
        public PointSample DashSample { get; private set; }
        public SplineData SideSplineData { get; private set; }
        public SplineData ForwardSplineData { get; private set; }
        public SplineData DashSplineData { get; private set; }

        private PhysicsConfig Config => _character.Config;

        private void Start()
        {
            _allSplines = new List<HESpline>();
            _splinesByTag = new Dictionary<SplineTag, List<HESpline>>();
            _cachedBounds = new Dictionary<HESpline, Bounds>();

            foreach (SplineTag heTag in Enum.GetValues(typeof(SplineTag)))
                _splinesByTag[heTag] = new List<HESpline>();

            foreach (var heSpline in FindObjectsByType<HESpline>(FindObjectsSortMode.None))
            {
                if (heSpline.Container == null || heSpline.Container.Spline == null)
                {
                    Debug.LogWarning($"HESpline {heSpline.name} has null Spline, skipping", heSpline);
                    continue;
                }

                _allSplines.Add(heSpline);
                if (_splinesByTag.TryGetValue(heSpline.SplineTag, out var list))
                    list.Add(heSpline);
                CacheBounds(heSpline);
            }
        }

        private void CacheBounds(HESpline heSpline)
        {
            var spline = heSpline.Container.Spline;
            if (spline != null)
            {
                var localBounds = heSpline.Container.Spline.GetBounds();
                var worldBounds = new Bounds(
                    heSpline.transform.TransformPoint(localBounds.center),
                    heSpline.transform.TransformVector(localBounds.size).Abs());

                worldBounds.Expand(_sideSplineSearchRadius * 2f);
                _cachedBounds[heSpline] = worldBounds;
            }
        }

        private void FixedUpdate()
        {
            CalculatePath2D();
            CalculatePathForward();
            CalculatePathDash();
        }

        private void CalculatePath2D()
        {
            if (ModeSide == null) return;

            Vector3 rbPos = Rigidbody.position - transform.up;
            var newContainer = SearchSplineContainer(rbPos, SplineTag.SideView | SplineTag.Grind);

            if (SideSplineData == null)
            {
                if (newContainer == null)
                {
                    ModeSide.StartPosition = Rigidbody.position;
                    ModeSide.CurrentEaseTime = 0f;
                    _lastRelativeTime = 0f;
                    _lastTangent = Vector3.zero;
                    return;
                }

                SideSplineData = new SplineData(newContainer, rbPos);
                UpdateRelativeTime(SideSplineData);
                ModeSide.StartPosition = Rigidbody.position;
                ModeSide.CurrentEaseTime = 0f;
            }

            if (newContainer != null && SideSplineData.Container != newContainer)
            {
                SideSplineData.UpdateContainer(newContainer);
                UpdateRelativeTime(SideSplineData);
            }

            SideSample = SideSplineData.EvaluateRelative(rbPos, _lastRelativeTime, 4);
            _lastRelativeTime = SideSample.Time;
            if ((SideSample.Position - rbPos).sqrMagnitude > 32)
            {
                SideSample = SideSplineData.EvaluateNearest(rbPos);
                UpdateRelativeTime(SideSplineData);
            }

            bool isGrind = _character.StateMachine.CurrentState is FStateGrind;

            if (!isGrind)
                Kinematics.Project(SideSample.Right);

            if (_lastTangent == Vector3.zero) _lastTangent = SideSample.Tangent;

            float sign = Mathf.Sign(Vector3.Dot(Rigidbody.transform.forward, SideSample.Tangent));
            float pathEaseTime = ModeSide.PathEaseTime;
            if (_character.Flags.GetFlag<AutorunFlag>(out var autoRunFlag) && autoRunFlag.PathEaseTime > 0)
                pathEaseTime = autoRunFlag.PathEaseTime;

            if (Kinematics.Speed > 0.02f && _character.Flags.HasFlag(FlagType.Autorun))
            {
                var rotTarget = Quaternion.LookRotation(SideSample.Tangent * sign, Kinematics.Normal);
                Rigidbody.MoveRotation(Quaternion.RotateTowards(Rigidbody.rotation, rotTarget, 720 * Time.fixedDeltaTime));
            }

            Vector3 newPos = SideSample.Position;
            Vector3 physicsTarget = newPos + Vector3.ProjectOnPlane(Rigidbody.position - newPos, SideSample.Right);
            if (pathEaseTime > 0f)
            {
                ModeSide.CurrentEaseTime += Time.fixedDeltaTime / pathEaseTime;
                ModeSide.CurrentEaseTime = Mathf.Clamp01(ModeSide.CurrentEaseTime);
                ModeSide.StartPosition += Kinematics.Velocity * Time.fixedDeltaTime;
            }
            
            if (!isGrind)
            {
                Vector3 target = pathEaseTime > 0f
                    ? Vector3.Lerp(ModeSide.StartPosition, physicsTarget, ModeSide.CurrentEaseTime)
                    : physicsTarget;

                Rigidbody.MovePosition(target);
            }
        }

        private void CalculatePathForward()
        {
            if (ModeForward == null) return;

            var newContainer = SearchSplineContainer(Rigidbody.position, SplineTag.DashPath | SplineTag.Quickstep);
            
            if (ForwardSplineData == null)
            {
                if (newContainer == null) return;

                ForwardSplineData = new SplineData(newContainer, Rigidbody.position);
                UpdateRelativeTime(ForwardSplineData);
            }

            if (newContainer != null && ForwardSplineData.Container != newContainer)
            {
                ForwardSplineData.UpdateContainer(newContainer);
                UpdateRelativeTime(ForwardSplineData);
            }

            ForwardSample = ForwardSplineData.EvaluateNearest(Rigidbody.position);
            AdjustVelocityForPath(ModeForward, ForwardSample);
        }

        private void CalculatePathDash()
        {
            if (ModeDash == null) return;

            var newContainer = SearchSplineContainer(Rigidbody.position, SplineTag.DashPath | SplineTag.Quickstep);

            if (DashSplineData == null)
            {
                if (newContainer == null) return;

                DashSplineData = new SplineData(newContainer, Rigidbody.position);
                UpdateRelativeTime(DashSplineData);
            }

            if (newContainer != null && DashSplineData.Container != newContainer)
            {
                DashSplineData.UpdateContainer(newContainer);
                UpdateRelativeTime(DashSplineData);
            }

            DashSample = DashSplineData.EvaluateNearest(Rigidbody.position);
            AdjustVelocityForPath(ModeDash, DashSample);
        }

        private void AdjustVelocityForPath(ChangeMode3DData data, PointSample sample)
        {
            var force = data.PathCorrectionForce;
            if (force > 0)
            {
                if (Kinematics.CheckForGround(out _))
                {
                    var tg = sample.Tangent;
                    if (data.IsLimitEdge)
                    {
                        if (IsPathOutOfRange(sample.Time))
                        {
                            SetForwardMode(null);
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
                        var rotation = Quaternion.RotateTowards(
                            Quaternion.LookRotation(horizontalVelocity),
                            Quaternion.LookRotation(targetDir),
                            adjustedForce * Mathf.Rad2Deg * Time.fixedDeltaTime);
                        Rigidbody.linearVelocity = rotation * Vector3.forward * horizontalVelocity.magnitude + Kinematics.VerticalVelocity;
                    }
                }
            }
        }

        private SplineContainer SearchSplineContainer(Vector3 position, SplineTag filter)
        {
            SplineContainer bestContainer = null;
            float bestDist = _sideSplineSearchRadius * _sideSplineSearchRadius;

            var currentData = GetActiveSplineData(filter);
            if (currentData != null)
            {
                float dist = GetNearestSqrDist(currentData.Container, position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestContainer = currentData.Container;
                }
            }

            foreach (var heSpline in _allSplines)
            {
                if (heSpline == null) continue;
                if (!filter.HasFlag(heSpline.SplineTag)) continue;
                if (currentData != null && heSpline.Container == currentData.Container) continue;
                if (heSpline.Container == null || heSpline.Container.Spline == null) continue; // <--

                if (_cachedBounds.TryGetValue(heSpline, out var worldBounds) && !worldBounds.Contains(position)) continue;

                float dist = GetNearestSqrDist(heSpline.Container, position);
                if (dist < bestDist)
                {
                    bestDist = dist;
                    bestContainer = heSpline.Container;
                }
            }

            return bestContainer;
        }

        private float GetNearestSqrDist(SplineContainer container, Vector3 position)
        {
            if (container == null || container.Spline == null) 
                return float.MaxValue;

            SplineUtility.GetNearestPoint(
                container.Spline,
                container.transform.InverseTransformPoint(position),
                out var nearestLocal, out _,
                resolution: 8, iterations: 4);

            return (container.transform.TransformPoint(nearestLocal) - position).sqrMagnitude;
        }

        private SplineData GetActiveSplineData(SplineTag filter)
        {
            if (filter.HasFlag(SplineTag.SideView) && SideSplineData != null) return SideSplineData;
            if (filter.HasFlag(SplineTag.Grind) && SideSplineData != null) return SideSplineData;
            if (filter.HasFlag(SplineTag.DashPath) && ForwardSplineData != null) return ForwardSplineData;
            if (filter.HasFlag(SplineTag.DashPath) && DashSplineData != null) return DashSplineData;
            return null;
        }

        public void Set2DMode(ChangeMode2DData data)
        {
            if (ModeSide == data) return;

            ModeSide = data;
            SideSplineData = null;
            _lastTangent = Vector3.zero;
            _lastRelativeTime = 0f;
            OnMode2DChange?.Invoke(data);

            if (ModeForward != null) SetForwardMode(null);
            if (ModeDash != null) SetDashPath(null);
        }

        public void SetForwardMode(ChangeMode3DData data)
        {
            if (ModeForward == data) return;

            ModeForward = data;
            ForwardSplineData = null;
            OnModeForwardChange?.Invoke(data);

            if (ModeSide != null) Set2DMode(null);
            if (ModeDash != null) SetDashPath(null);
        }

        public void SetDashPath(ChangeMode3DData data)
        {
            if (ModeDash == data) return;

            ModeDash = data;
            DashSplineData = null;
            OnModeDashChange?.Invoke(data);

            if (ModeSide != null) Set2DMode(null);
            if (ModeForward != null) SetForwardMode(null);
        }

        public void UpdateRelativeTime(SplineData data)
        {
            if (data == null || data.Container == null || data.Container.Spline == null) return;

            SplineUtility.GetNearestPoint(data.Container.Spline,
                data.Container.transform.InverseTransformPoint(Rigidbody.position),
                out _, out _lastRelativeTime);
        }

        public void Load()
        {
            Set2DMode(null);
            SetForwardMode(null);
            SetDashPath(null);
        }

        private static bool IsPathOutOfRange(float t) => t >= 1f || t <= 0;
    }
}