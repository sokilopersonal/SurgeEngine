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

        public SplineData(SplineContainer container, Vector3 position, DominantSpline dominant = DominantSpline.Left)
        {
            _container = container;
            Dominant = dominant;
            
            UpdateTime(position);
        }

        public void EvaluateWorld(out Vector3 position, out Vector3 tangent, out Vector3 up, out Vector3 right)
        {
            var transform = _container.transform;
            if (_container.Splines.Count == 2)
            {
                var splineL = _container.Splines[Dominant == DominantSpline.Left ? 0 : 1];
                var splineR = _container.Splines[Dominant == DominantSpline.Left ? 1 : 0];

                float t = NormalizedTime;
            
                splineL.Evaluate(t,
                    out var posL,
                    out var tgL,
                    out _);
            
                splineR.Evaluate(t,
                    out var posR,
                    out var tgR,
                    out _);

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
                _container.Spline.Evaluate(NormalizedTime,
                    out var pos,
                    out var tg,
                    out var upVector);

                position = transform.TransformPoint(pos);
                tangent = transform.TransformDirection(tg).normalized;
                right = Vector3.Cross(tangent, upVector);
                up = upVector;
            }
            
            Debug.DrawRay(position, tangent, Color.blue, 0, false);
            Debug.DrawRay(position, up, Color.green, 0, false);
            Debug.DrawRay(position, right, Color.red, 0, false);
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
                    out _, out var f, 12, 8);
            
                Time = f * Length;
            }
            else
            {
                Debug.LogError("When trying to update time on spline we got null container. How?");
            }
        }
    }
}