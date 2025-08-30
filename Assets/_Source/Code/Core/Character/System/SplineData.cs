using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Code.Core.Actor.System
{
    public class SplineData
    {
        public float Time { get; set; }
        public float Length => _container.Spline.GetLength();
        public float NormalizedTime => Time / Length;
        public SplineContainer Container => _container;

        private readonly SplineContainer _container;

        public SplineData(SplineContainer container, Vector3 position)
        {
            _container = container;
            
            SplineUtility.GetNearestPoint(container.Spline, _container.transform.InverseTransformPoint(position), 
                out _, out var f, 8, 4);
            
            Time = f * Length;
        }

        public void EvaluateWorld(out Vector3 position, out Vector3 tangent, out Vector3 up, out Vector3 right)
        {
            var transform = _container.transform;

            float normalizedTime = Mathf.Clamp01(Mathf.Max(0.001f, Time / Length));
            var spline = _container.Spline;

            spline.Evaluate(normalizedTime, 
                out var pos,
                out var tg,
                out var upVector);
            
            position = transform.TransformPoint(pos);
            tangent = transform.TransformDirection(tg).normalized;
            up = transform.TransformDirection(upVector);
            right = Vector3.Cross(tangent, -up).normalized;
        }

        public Vector3 EvaluatePosition()
        {
            EvaluateWorld(out var pos, out _, out _, out _);
            return pos;
        }
        
        public Vector3 EvaluateTangent()
        {
            EvaluateWorld(out _, out var tg, out _, out _);
            return tg;
        }
    }
}