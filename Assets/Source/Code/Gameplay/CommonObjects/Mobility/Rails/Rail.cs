using SurgeEngine.Source.Code.Core.Character.States;
using SurgeEngine.Source.Code.Core.Character.System;
using SurgeEngine.Source.Code.Core.StateMachine.Base;
using SurgeEngine.Source.Code.Gameplay.CommonObjects.ChangeModes;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Splines;
using Zenject;

namespace SurgeEngine.Source.Code.Gameplay.CommonObjects.Mobility.Rails
{
    [RequireComponent(typeof(MeshCollider), typeof(SplineContainer)), DisallowMultipleComponent]
    public class Rail : MonoBehaviour
    {
        [SerializeField] private SplineContainer container;
        [SerializeField] private float radius;
        public SplineContainer Container => container;
        public float Radius => radius;

        [Inject] private DiContainer _diContainer;

        private CharacterBase _character;
        private Collider _collider;

        private void Awake()
        {
            if (!container)
                container = GetComponent<SplineContainer>();
            
            _collider = GetComponentInChildren<Collider>();
            if (_collider != null && ((MeshCollider)_collider).sharedMesh == null)
                GenerateCollider(_collider as MeshCollider);

            var player = _diContainer.Resolve<CharacterBase>();
            if (player.GetComponent<HomingTargetDetector>()) // Check if the player is actually able to do homing attack,
                                                             // otherwise homing target is not needed 
            {
                const string key = "HomingTargetPrefab";
                Addressables.LoadAssetAsync<GameObject>(key).Completed += op =>
                {
                    var homingTargetPrefab = op.Result.GetComponent<HomingTarget>();
                    var target = Instantiate(homingTargetPrefab, transform, false);
                    target.OnTargetReached.AddListener(AttachToRail);
                    target.SetDistanceThreshold(1f);

                    var pos = Container.Spline.EvaluatePosition(0f);
                    target.transform.position = transform.TransformPoint(pos);
                };
            }
            
            gameObject.layer = LayerMask.NameToLayer("Rail");
        }

        private void GenerateCollider(MeshCollider meshCollider)
        {
            var mesh = GenerateCollisionMesh();
            meshCollider.sharedMesh = mesh;
            meshCollider.convex = false;
        }

        private Mesh GenerateCollisionMesh()
        {
            Mesh mesh = new Mesh();
            mesh.name = $"Rail Mesh: {container.transform.name}";

            int splineCount = container.Splines.Count;
            float effectiveRadius = radius;

            if (splineCount == 2)
            {
                Vector3 pos0 = container.Splines[0].EvaluatePosition(0f);
                Vector3 pos1 = container.Splines[1].EvaluatePosition(0f);
                effectiveRadius = Vector3.Distance(pos0, pos1) * 0.5f;
            }

            Spline spline = container.Splines[0];
            int samples = 4;
            int segments = Mathf.Max(samples, (int)(spline.GetLength() * samples));

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            for (int i = 0; i <= segments; i++)
            {
                float t = i / (float)segments;
                if (splineCount == 2)
                {
                    var sample = GetSplineVectorsCurve(t);
                    var position = container.transform.InverseTransformPoint(sample.Position);
                    var right = container.transform.InverseTransformDirection(sample.Right);
                    
                    vertices.Add(position - right * effectiveRadius);
                    vertices.Add(position + right * effectiveRadius);
                }
                else if (splineCount == 1)
                {
                    Vector3 position = spline.EvaluatePosition(t);
                    Vector3 tangent = ((Vector3)spline.EvaluateTangent(t)).normalized;
                    Vector3 up = spline.EvaluateUpVector(t);
                    Vector3 right = Vector3.Cross(tangent, up).normalized;

                    vertices.Add(position - right * effectiveRadius);
                    vertices.Add(position + right * effectiveRadius);
                }
            }

            for (int i = 0; i < segments; i++)
            {
                int baseIndex = i * 2;

                triangles.Add(baseIndex);
                triangles.Add(baseIndex + 2);
                triangles.Add(baseIndex + 1);

                triangles.Add(baseIndex + 1);
                triangles.Add(baseIndex + 2);
                triangles.Add(baseIndex + 3);
            }

            for (int i = 0; i < segments; i++)
            {
                int baseIndex = i * 2;

                triangles.Add(baseIndex);
                triangles.Add(baseIndex + 1);
                triangles.Add(baseIndex + 2);

                triangles.Add(baseIndex + 1);
                triangles.Add(baseIndex + 3);
                triangles.Add(baseIndex + 2);
            }

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }
        
        private PointSample GetSplineVectorsCurve(float t)
        {
            var containerTransform = container.transform;
            var dominant = gameObject.GetComponent<HESpline>().Dominant;
            var splineL = container.Splines[dominant == DominantSide.Left ? 0 : 1];
            var splineR = container.Splines[dominant == DominantSide.Left ? 1 : 0];

            Spline spline0 = dominant == DominantSide.Left ? splineL : splineR;
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

            Spline spline1 = dominant == DominantSide.Left ? splineR : splineL;
            float3 position1 = CurveUtility.EvaluatePosition(spline1.GetCurve(curveIndex), t1);

            Vector3 positionL = containerTransform.TransformPoint(dominant == DominantSide.Left ? position0 : position1);
            Vector3 positionR = containerTransform.TransformPoint(dominant == DominantSide.Left ? position1 : position0);

            Vector3 position = Vector3.Lerp(positionL, positionR, 0.5f);
            Vector3 forward = containerTransform.TransformDirection(tangent0).normalized;
            if (forward.magnitude < 0.5f)
                forward = containerTransform.TransformDirection(SplineUtility.GetCatmullRomTangent(curve0.P0, curve0.P3)).normalized;

            Vector3 right = Vector3.Normalize(positionR - positionL);
            Vector3 up = Vector3.Cross(forward, right).normalized;

            return new PointSample(position, forward, up, right, t);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CharacterBase character) 
                && !IsStateExcluded(character.StateMachine.CurrentState))
            {
                AttachToRail(character);
            }
            
            bool IsStateExcluded(FState current) 
                => current is FStateRailSwitch or FStateAirObject;
        }

        private void AttachToRail(CharacterBase character)
        {
            Physics.IgnoreCollision(_collider, character.Model.Collision, true);
            character.StateMachine.SetState<FStateGrind>()?.SetRail(this);
            _character = character;
            _character.StateMachine.OnStateAssign += DisableCollision;
        }

        private void DisableCollision(FState obj)
        {
            if (obj is not FStateGrind)
            {
                Physics.IgnoreCollision(_collider, _character.Model.Collision, false);
                _character.StateMachine.OnStateAssign -= DisableCollision;
            }
        }
    }
}