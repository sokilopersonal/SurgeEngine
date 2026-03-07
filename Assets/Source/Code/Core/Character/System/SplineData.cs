using SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Source.Code.Core.Character.System
{
    public class SplineData
    {
        public float Time { get; set; }
        public float Length => _container.Spline.GetLength();
        public float NormalizedTime => Mathf.Clamp01(Time / Length);
        public SplineContainer Container => _container;
        private DominantSpline Dominant { get; }

        private readonly SplineContainer _container;
        private float _lastStableTime;
        private const float VerticalThreshold = 0.99f;

        public SplineData(SplineContainer container, Vector3 position, DominantSpline dominant = DominantSpline.Left)
        {
            _container = container;
            Dominant = dominant;
            
            UpdateTime(position);
        }

        public void EvaluateWorld(out Vector3 position, out Vector3 tangent, out Vector3 up, out Vector3 right)
        {
            var transform = _container.transform;
            float t = NormalizedTime;
            
            if (_container.Splines.Count == 2)
            {
                var splineL = _container.Splines[Dominant == DominantSpline.Left ? 0 : 1];
                var splineR = _container.Splines[Dominant == DominantSpline.Left ? 1 : 0];

                splineL.Evaluate(t, out var posL, out var tgL, out _);
                splineR.Evaluate(t, out var posR, out var tgR, out _);

                Vector3 worldPosL = transform.TransformPoint(posL);
                Vector3 worldPosR = transform.TransformPoint(posR);
                Vector3 worldTgR = transform.TransformDirection(tgR);

                position = Vector3.Lerp(worldPosL, worldPosR, 0.5f);
                tangent = worldTgR.normalized;
                right = Vector3.Normalize(worldPosR - worldPosL);
                up = Vector3.Cross(tangent, right);
            }
            else
            {
                _container.Spline.Evaluate(t, out var pos, out var tg, out var upVector);
                position = transform.TransformPoint(pos);
                tangent = transform.TransformDirection(tg).normalized;
                right = Vector3.Cross(upVector, tangent).normalized;
                up = upVector;
            }
            
            Debug.DrawRay(position, tangent, Color.blue);
            Debug.DrawRay(position, up, Color.green);
            Debug.DrawRay(position, right, Color.red);
        }

        public PointSample Evaluate(float t)
        {
            var transform = _container.transform;
            if (_container.Splines.Count == 2)
            {
                var splineL = _container.Splines[Dominant == DominantSpline.Left ? 0 : 1];
                var splineR = _container.Splines[Dominant == DominantSpline.Left ? 1 : 0];

                splineL.Evaluate(t, out var posL, out var tgL, out _);
                splineR.Evaluate(t, out var posR, out var tgR, out _);

                Vector3 worldPosL = transform.TransformPoint(posL);
                Vector3 worldPosR = transform.TransformPoint(posR);
                Vector3 worldTgR = transform.TransformDirection(tgR);

                var position = Vector3.Lerp(worldPosL, worldPosR, 0.5f);
                var tangent = worldTgR.normalized;
                var right = Vector3.Normalize(worldPosR - worldPosL);
                var up = Vector3.Cross(tangent, right);
                return new PointSample(position, tangent, up, right);
            }
            else
            {
                _container.Spline.Evaluate(t, out var pos, out var tg, out var upVector);
                var position = transform.TransformPoint(pos);
                var tangent = transform.TransformDirection(tg).normalized;
                var right = Vector3.Cross(upVector, tangent).normalized;
                var up = upVector;
                return new PointSample(position, tangent, up, right);
            }
        }

        public PointSample EvaluateNearest(Vector3 position, int resolution = 4, int iterations = 2)
        {
            SplineUtility.GetNearestPoint(_container.Spline, _container.transform.InverseTransformPoint(position), out var t, out var f, resolution, iterations);

            return Evaluate(f);
        }

        public PointSample EvaluateStandard(float t)
        {
            _container.Spline.Evaluate(t, out var pos, out var tg, out var upVector);
            return new PointSample(pos, tg, upVector, Vector3.Cross(tg, upVector));
        }
        
        public PointSample EvaluateRelative(Vector3 position, float relative, float resolution = 2)
        {
            if (resolution <= 0)
                return Evaluate(relative);

            PointSample nearestP = March(1, out var p);
            PointSample nearestN = March(-1, out var n);
    
            PointSample nearest = p < n ? nearestP : nearestN;
            return nearest;

            PointSample March(float direction, out float distance)
            {
                PointSample thisNearest = Evaluate(relative);
                distance = (position - thisNearest.Position).sqrMagnitude;
        
                bool Condition(float t)
                {
                    return direction >= 0 ? t <= 1 : t >= 0;
                }
                
                float step = Mathf.Sign(direction) / resolution / Length;
                for (float t = relative; Condition(t); t += step)
                {
                    PointSample candidate = Evaluate(t);
                    float candidateDistance = (position - candidate.Position).sqrMagnitude;
                    if (candidateDistance <= distance)
                    {
                        thisNearest = candidate;
                        distance = candidateDistance;
                    }
                    else
                    {
                        return thisNearest;
                    }
                }
        
                return EvaluateStandard(direction >= 0 ? 1 : 0);
            }
        }

        public Vector3 EvaluatePosition()
        {
            EvaluateWorld(out var pos, out _, out _, out _);
            return pos;
        }

        public Vector3 EvaluateUp()
        {
            EvaluateWorld(out _, out _, out var up, out _);
            return up;
        }

        public Vector3 EvaluateTangent()
        {
            EvaluateWorld(out _, out var tg, out _, out _);
            return tg;
        }

        public void UpdateTime(Vector3 position)
        {
            if (_container)
            {
                SplineUtility.GetNearestPoint(_container.Spline, _container.transform.InverseTransformPoint(position), 
                    out _, out var f, 24, 10);
            
                Time = f * Length;
            }
            else
            {
                Debug.LogError("When trying to update time on spline we got null container. How?");
            }
        }
    }

    public struct PointSample
    {
        public Vector3 Position;
        public Vector3 Tangent;
        public Vector3 Up;
        public Vector3 Right;
        
        public PointSample(Vector3 position, Vector3 tangent, Vector3 up, Vector3 right)
        {
            Position = position;
            Tangent = tangent;
            Up = up;
            Right = right;
        }
        
        public Vector3 ProjectOnUp(Vector3 plane) => Vector3.ProjectOnPlane(plane, Up);
    }
}