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
        public SplineData Data { get; private set; }

        [Inject] private DiContainer _diContainer;

        private CharacterBase _character;
        private Collider _collider;

        private void Awake()
        {
            if (!container)
                container = GetComponent<SplineContainer>();
            
            Data = new SplineData(container);
            
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

                    var sample = Data.Evaluate(0);
                    target.transform.position = sample.Position;
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
                var sample = Data.Evaluate(t);
                Vector3 position = container.transform.InverseTransformPoint(sample.Position);
                Vector3 right = container.transform.InverseTransformDirection(sample.Right);
                
                vertices.Add(position - right * effectiveRadius);
                vertices.Add(position + right * effectiveRadius);
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