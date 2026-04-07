using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Source.Code.Core.Character.System
{
    public class SplineData
    {
        public float Time { get; set; }
        public float Length;
        public float NormalizedTime => Mathf.Clamp01(Time / Length);
        public SplineTag Tag { get; private set; }
        public SplineContainer Container => _container;
        private DominantSide Dominant { get; set; }

        private SplineContainer _container;

        public SplineData(SplineContainer container, Vector3 position)
        {
            Dominant = DominantSide.Left;
            
            UpdateContainer(container);
            UpdateTime(position);
        }

        public void EvaluateWorld(out Vector3 position, out Vector3 tangent, out Vector3 up, out Vector3 right)
        {
            float t = NormalizedTime;
            var sample = Evaluate(t);
            position = sample.Position;
            tangent = sample.Tangent;
            up = sample.Up;
            right = sample.Right;
        }

        public PointSample Evaluate(float t)
        {
            var transform = _container.transform;
            if (_container.Splines.Count == 2)
            {
                PointSample sample = HasCurve(t)
                    ? GetSplineVectorsCurve(t)
                    : GetSplineVectorsSimple(t);

                DrawDebug(sample.Position, sample.Tangent, sample.Up, sample.Right);
                return sample;
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
                Debug.DrawRay(position, tangent, Color.blue, 0, false);
                Debug.DrawRay(position, up, Color.green, 0, false);
                Debug.DrawRay(position, right, Color.red, 0, false);
            }
        }
        
        private PointSample GetSplineVectorsSimple(float t)
        {
            var transform = _container.transform;
            var splineL = _container.Splines[Dominant == DominantSide.Left ? 0 : 1];
            var splineR = _container.Splines[Dominant == DominantSide.Left ? 1 : 0];

            splineL.Evaluate(t, out var posL, out _, out _);
            splineR.Evaluate(t, out var posR, out var tgR, out _);

            Vector3 worldPosL = transform.TransformPoint(posL);
            Vector3 worldPosR = transform.TransformPoint(posR);
            Vector3 forward = transform.TransformDirection(tgR).normalized;
            Vector3 right = Vector3.Normalize(worldPosR - worldPosL);
            Vector3 up = Vector3.Cross(forward, right);
            Vector3 position = Vector3.Lerp(worldPosL, worldPosR, 0.5f);

            return new PointSample(position, forward, up, right, t);
        }

        private PointSample GetSplineVectorsCurve(float t)
        {
            var transform = _container.transform;
            var splineL = _container.Splines[Dominant == DominantSide.Left ? 0 : 1];
            var splineR = _container.Splines[Dominant == DominantSide.Left ? 1 : 0];

            Spline spline0 = Dominant == DominantSide.Left ? splineL : splineR;
            spline0.Evaluate(t, out float3 position0, out float3 tangent0, out _);
            int curveIndex = spline0.SplineToCurveT(t, out float t1);

            BezierCurve curve0 = spline0.GetCurve(curveIndex);
            if ((Vector3)curve0.P0 == (Vector3)curve0.P1)
            {
                while ((Vector3)curve0.P0 == (Vector3)curve0.P1 && curveIndex < splineL.Count)
                    curve0 = splineL.GetCurve(curveIndex++);

                curve0 = splineL.GetCurve(curveIndex--);
                t1 = 0;
            }

            Spline spline1 = Dominant == DominantSide.Left ? splineR : splineL;
            float3 position1 = CurveUtility.EvaluatePosition(spline1.GetCurve(curveIndex), t1);

            Vector3 positionL = transform.TransformPoint(Dominant == DominantSide.Left ? position0 : position1);
            Vector3 positionR = transform.TransformPoint(Dominant == DominantSide.Left ? position1 : position0);

            Vector3 position = Vector3.Lerp(positionL, positionR, 0.5f);
            Vector3 forward = transform.TransformDirection(tangent0).normalized;
            if (forward.magnitude < 0.5f)
                forward = transform.TransformDirection(SplineUtility.GetCatmullRomTangent(curve0.P0, curve0.P3)).normalized;

            Vector3 right = Vector3.Normalize(positionR - positionL);
            Vector3 up = Vector3.Cross(forward, right).normalized;

            return new PointSample(position, forward, up, right, t);
        }
        
        private bool HasCurve(float t)
        {
            Spline spline0 = _container.Splines[Dominant == DominantSide.Left ? 0 : 1];
            int curveIndex = spline0.SplineToCurveT(t, out _);
            BezierCurve curve = spline0.GetCurve(curveIndex);

            return (Vector3)curve.P0 != (Vector3)curve.P1
                   || (Vector3)curve.P2 != (Vector3)curve.P3;
        }
        
        private DominantSide DetectDominant(SplineContainer container)
        {
            if (container.Splines.Count < 2) return DominantSide.Left;

            var transform = container.transform;
            var spline0 = container.Splines[0];
            var spline1 = container.Splines[1];
            
            const int samples = 8;
            float totalCross = 0f;

            for (int i = 0; i < samples; i++)
            {
                float t = i / (float)(samples - 1);

                spline0.Evaluate(t, out var pos0, out var tg0, out _);
                spline1.Evaluate(t, out var pos1, out _, out _);

                Vector3 worldPos0 = transform.TransformPoint(pos0);
                Vector3 worldPos1 = transform.TransformPoint(pos1);
                Vector3 worldTg0  = transform.TransformDirection(tg0).normalized;

                Vector3 toSpline1 = (worldPos1 - worldPos0).normalized;
                if ((worldPos1 - worldPos0).sqrMagnitude < 0.001f) continue;

                totalCross += Vector3.Dot(Vector3.Cross(worldTg0, toSpline1), Vector3.up);
            }
            
            return totalCross >= 0 ? DominantSide.Left : DominantSide.Right;
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
            if (container != null)
            {
                _container = container;
                Length = container.Spline.GetLength();
                if (container.TryGetComponent(out HESpline dominant))
                {
                    Tag = dominant.SplineTag;
                }
                
                Dominant = DetectDominant(container);
            }
        }

        public void UpdateTime(Vector3 position)
        {
            if (_container)
            {
                SplineUtility.GetNearestPoint(_container.Spline, _container.transform.InverseTransformPoint(position), 
                    out _, out var f, 8, 4);
            
                Time = f * Length;
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