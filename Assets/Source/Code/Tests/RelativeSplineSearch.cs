using System;
using SurgeEngine.Source.Code.Core.Character.System;
using UnityEngine;
using UnityEngine.Splines;

namespace SurgeEngine.Source.Code.Tests
{
    public class RelativeSplineSearch : MonoBehaviour
    {
        [SerializeField] private SplineContainer container;
        [SerializeField] private Transform searchTarget;

        private SplineData _splineData;
        private float _relative;
        
        private void Awake()
        {
            if (searchTarget == null) searchTarget = transform;
            
            _splineData = new SplineData(container, searchTarget.position);
        }

        private void FixedUpdate()
        {
            var sample = _splineData.EvaluateRelative(searchTarget.position, _relative, 64);
            _relative = sample.T;
            
            Debug.DrawRay(sample.Position, sample.Tangent, Color.purple, 0, false);
            Debug.DrawRay(sample.Position, sample.Up, Color.green, 0, false);
            Debug.DrawRay(sample.Position, sample.Right, Color.red, 0, false);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.2f);
        }
    }
}