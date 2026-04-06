using SurgeEngine.Source.Code.Gameplay.CommonObjects;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes;
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
        public SplineContainer Container => _container;
        private DominantSide Dominant { get; set; }

        private SplineContainer _container;

        public SplineData(SplineContainer container, Vector3 position, DominantSide dominant = DominantSide.Left)
        {
            Dominant = dominant;
            UpdateContainer(container);
            
            UpdateTime(position);
        }

        public void EvaluateWorld(out Vector3 position, out Vector3 tangent, out Vector3 up, out Vector3 right)
        {
            var transform = _container.transform;
            float t = NormalizedTime;
            if (_container.Splines.Count == 2)
            {
                var splineL = _container.Splines[Dominant == DominantSide.Left ? 0 : 1];
                var splineR = _container.Splines[Dominant == DominantSide.Left ? 1 : 0];

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

        public PointSample EvaluateNearest(Vector3 position, int resolution = 4, int iterations = 2)
        {
            SplineUtility.GetNearestPoint(_container.Spline, _container.transform.InverseTransformPoint(position), out _, out var f, resolution, iterations);

            return Evaluate(f);
        }
        
        public PointSample EvaluateRelative(Vector3 position, float relative, float resolution = 2) // optimized version of Josh's code
        {
            if (resolution <= 0)
                return Evaluate(relative);

            PointSample nearestP = March(1, out float distanceP);
            PointSample nearestM = March(-1, out float distanceM);

            PointSample nearest = distanceP < distanceM
                ? nearestP
                : nearestM;

            return nearest;
            
            PointSample March(float direction, out float distance)
            {
                PointSample nearestSample = Evaluate(relative);
                distance = Vector3.Distance(nearestSample.Position, position);

                bool Condition(float t) => direction >= 0 ? t <= 1 : t >= 0;
                float step = Mathf.Sign(direction) / resolution / Length;

                for (float t = relative; Condition(t); t += step)
                {
                    PointSample thisPoint = Evaluate(t);
                    float thisDistance = Vector3.Distance(thisPoint.Position, position);

                    if (thisDistance <= distance)
                    {
                        nearestSample = thisPoint;
                        distance = thisDistance;
                    }
                    else
                        return nearestSample;
                }

                return Evaluate(direction >= 0 ? 1 : 0);
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

        public void UpdateContainer(SplineContainer container)
        {
            if (_container == container)
                return;
            
            _container = container;
            Length = _container.Spline.GetLength();
            if (container.TryGetComponent(out DominantSpline dominant))
                Dominant = dominant.OverrideDominantSide;
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