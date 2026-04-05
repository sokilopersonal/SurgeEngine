using SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Splines;

namespace SurgeEngine.Source.Code.Core.Character.System
{
    public class SplineData
    {
        public float Time { get; set; }
        public float Length;
        public float NormalizedTime => Mathf.Clamp01(Time / Length);
        public SplineContainer Container => _container;
        private DominantSpline Dominant { get; }

        private SplineContainer _container;

        public SplineData(SplineContainer container, Vector3 position, DominantSpline dominant = DominantSpline.Left)
        {
            _container = container;
            Dominant = dominant;
            
            Length = _container.Spline.GetLength();
            
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
                
                Debug.DrawRay(worldPosL, up, Color.white);
                Debug.DrawRay(worldPosR, up, Color.white);
                DrawDebug(position, tangent, up, right);
                return new PointSample(position, tangent, up, right, t);
            }
            else
            {
                _container.Spline.Evaluate(t, out var pos, out var tg, out var upVector);
                var position = transform.TransformPoint(pos);
                var tangent = transform.TransformDirection(tg).normalized;
                var right = Vector3.Cross(upVector, tangent).normalized;
                var up = upVector;
                DrawDebug(position, tangent, up, right);
                return new PointSample(position, tangent, up, right, t);
            }

            void DrawDebug(Vector3 position, Vector3 tangent, Vector3 up, Vector3 right)
            {
                Debug.DrawRay(position, tangent, Color.purple, 0, false);
                Debug.DrawRay(position, up, Color.green, 0, false);
                Debug.DrawRay(position, right, Color.red, 0, false);
            }
        }
        
        public PointSample EvaluateNearest(Vector3 position, int resolution = 4, int iterations = 2)
        {
            SplineUtility.GetNearestPoint(_container.Spline, _container.transform.InverseTransformPoint(position), out _, out var f, resolution, iterations);

            return Evaluate(f);
        }
        
        public PointSample EvaluateRelative(Vector3 position, float relative, float resolution = 2) // optimized version of Josh's code
        {
            if (resolution <= 0)
                return Evaluate(relative);

            float step = 1f / resolution / Length;

            PointSample bestSample = Evaluate(relative);
            float bestDist = (position - bestSample.Position).sqrMagnitude;
            
            for (float t = relative + step; t <= 1f; t += step)
            {
                PointSample candidate = Evaluate(t);
                float d = (position - candidate.Position).sqrMagnitude;
                if (d <= bestDist) { bestSample = candidate; bestDist = d; }
                else break;
            }

            PointSample backSample = Evaluate(relative);
            float backDist = (position - backSample.Position).sqrMagnitude;
            
            for (float t = relative - step; t >= 0f; t -= step)
            {
                PointSample candidate = Evaluate(t);
                float d = (position - candidate.Position).sqrMagnitude;
                if (d <= backDist) { backSample = candidate; backDist = d; }
                else break;
            }

            PointSample result = bestDist <= backDist ? bestSample : backSample;
            
            if (result.Time >= 1f - step) return Evaluate(1f);
            if (result.Time <= step)      return Evaluate(0f);

            return result;
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

        public void UpdateContainer(SplineContainer container)
        {
            if (_container == container)
                return;
            
            _container = container;
            Length = _container.Spline.GetLength();
        }

        public void UpdateTime(Vector3 position)
        {
            if (_container)
            {
                SplineUtility.GetNearestPoint(_container.Spline, _container.transform.InverseTransformPoint(position), 
                    out _, out var f, 8, 4);
            
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
        public readonly float Time;
        
        public PointSample(Vector3 position, Vector3 tangent, Vector3 up, Vector3 right, float t)
        {
            Position = position;
            Tangent = tangent;
            Up = up;
            Right = right;
            Time = t;
        }
        
        public Vector3 ProjectOnUp(Vector3 plane) => Vector3.ProjectOnPlane(plane, Up);
    }
}