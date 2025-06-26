using System.Collections.Generic;
using UnityEngine;

namespace SurgeEngine.Code.Rendering
{
    [ExecuteInEditMode]
    public class GrassRenderer : MonoBehaviour
    {
        [System.Serializable]
        public struct GrassInstance
        {
            public Vector3 position;
            public float rotation;
            public float height;
            public float width;
        }

        [Header("Grass Settings")]
        [SerializeField] private Mesh grassMesh;
        [SerializeField] private Material grassMaterial;
        [SerializeField] private int maxGrassCount = 10000;
        
        [Header("Rendering Settings")]
        [SerializeField] private float minHeight = 0.8f;
        [SerializeField] private float maxHeight = 1.2f;
        [SerializeField] private float minWidth = 0.7f;
        [SerializeField] private float maxWidth = 1.3f;
        [SerializeField] private float density = 1f;
        [Tooltip("Maximum distance from camera at which grass will be rendered")]
        [SerializeField] private float maxRenderDistance = 100f;
        
        [Tooltip("Enable or disable distance-based rendering")]
        [SerializeField] private bool useRenderDistance = true;
        
        [HideInInspector]
        public List<GrassInstance> grassInstances = new();
        
        private Matrix4x4[] _matrices;
        private Matrix4x4[] _visibleMatrices;
        private readonly List<int> _visibleIndices = new();
        private MaterialPropertyBlock _propertyBlock;
        private int _instanceCount;
        private int _visibleInstanceCount;
        private Camera _camera;

        private void OnEnable()
        {
            _matrices = new Matrix4x4[maxGrassCount];
            _visibleMatrices = new Matrix4x4[maxGrassCount];
            _propertyBlock = new MaterialPropertyBlock();
            
            UpdateMatrices();
        }

        public void UpdateMatrices()
        {
            _instanceCount = Mathf.Min(grassInstances.Count, maxGrassCount);
            
            for (int i = 0; i < _instanceCount; i++)
            {
                GrassInstance instance = grassInstances[i];
        
                Quaternion rotation = Quaternion.Euler(0, instance.rotation, 0);
        
                _matrices[i] = Matrix4x4.TRS(
                    instance.position,
                    rotation,
                    new Vector3(instance.width, instance.height, instance.width)
                );
            }
        }
        
        private void UpdateVisibleMatrices()
        {
            if (!useRenderDistance)
            {
                _visibleInstanceCount = _instanceCount;
                System.Array.Copy(_matrices, _visibleMatrices, _instanceCount);
                return;
            }
            
            if (_camera == null)
            {
                _camera = Camera.main;
                if (_camera == null)
                {
                    _visibleInstanceCount = _instanceCount;
                    System.Array.Copy(_matrices, _visibleMatrices, _instanceCount);
                    return;
                }
            }
            
            Vector3 cameraPosition = _camera.transform.position;
            float sqrMaxDistance = maxRenderDistance * maxRenderDistance;
            
            _visibleIndices.Clear();

            for (int i = 0; i < _instanceCount; i++)
            {
                float sqrDistance = (grassInstances[i].position - cameraPosition).sqrMagnitude;
                if (sqrDistance <= sqrMaxDistance)
                {
                    _visibleIndices.Add(i);
                }
            }

            _visibleInstanceCount = _visibleIndices.Count;
            for (int i = 0; i < _visibleInstanceCount; i++)
            {
                int originalIndex = _visibleIndices[i];
                _visibleMatrices[i] = _matrices[originalIndex];
            }
        }

        private void LateUpdate()
        {
            if (grassMesh == null || grassMaterial == null || _instanceCount == 0)
                return;

            UpdateVisibleMatrices();
            
            if (_visibleInstanceCount == 0)
                return;

            Graphics.DrawMeshInstanced(
                grassMesh,
                0,
                grassMaterial,
                _visibleMatrices,
                _visibleInstanceCount,
                _propertyBlock,
                UnityEngine.Rendering.ShadowCastingMode.On,
                false
            );
        }

        public void AddGrassInstance(Vector3 position, float size = 1f, Vector3? surfaceNormal = null)
        {
            if (grassInstances.Count >= maxGrassCount)
                return;
        
            float randomRotation = Random.Range(0f, 360f);
            float randomHeight = Random.Range(minHeight, maxHeight) * size;
            float randomWidth = Random.Range(minWidth, maxWidth) * size;
        
            GrassInstance instance = new GrassInstance
            {
                position = position,
                rotation = randomRotation,
                height = randomHeight,
                width = randomWidth
            };

            grassInstances.Add(instance);
            UpdateMatrices();
        }

        public void ClearGrass()
        {
            grassInstances.Clear();
            UpdateMatrices();
        }

        public void RemoveGrassInRadius(Vector3 center, float radius)
        {
            float sqrRadius = radius * radius;
            grassInstances.RemoveAll(instance => 
                (instance.position - center).sqrMagnitude <= sqrRadius
            );
            UpdateMatrices();
        }
        
        public void RandomizeGrassInstance(int index, float size = 1f)
        {
            if (index < 0 || index >= grassInstances.Count)
                return;
                
            GrassInstance instance = grassInstances[index];
            Vector3 position = instance.position;

            instance.rotation = Random.Range(0f, 360f);
            instance.height = Random.Range(minHeight, maxHeight) * size;
            instance.width = Random.Range(minWidth, maxWidth) * size;

            instance.position = position;

            grassInstances[index] = instance;
            UpdateMatrices();
        }
        
        public void RandomizeGrassInRadius(Vector3 center, float radius, float size = 1f)
        {
            float sqrRadius = radius * radius;
            
            for (int i = 0; i < grassInstances.Count; i++)
            {
                GrassInstance instance = grassInstances[i];
                if ((instance.position - center).sqrMagnitude <= sqrRadius)
                {
                    RandomizeGrassInstance(i, size);
                }
            }
        }
    }
}