using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace SurgeEngine.Source.Code.Rendering
{
    [StructLayout(LayoutKind.Sequential)]
    struct GPUGrassInstance
    {
        public Matrix4x4 mat;
        public Vector4 posAndTex;
    }
    
    [ExecuteInEditMode]
    public class GrassRenderer : MonoBehaviour
    {
        private static readonly int PropIndex = Shader.PropertyToID("_Index");
        private static readonly int PropCameraPos = Shader.PropertyToID("_CameraPosition");
        private static readonly int PropMaxDistanceSqr = Shader.PropertyToID("_MaxDistanceSqr");
        private static readonly int PropUseDistance = Shader.PropertyToID("_UseDistance");
        private static readonly int PropFrustumPlanes = Shader.PropertyToID("_FrustumPlanes");
        private static readonly int PropTotalCount = Shader.PropertyToID("_TotalCount");
        private static readonly int PropAllInstances = Shader.PropertyToID("_AllInstances");
        private static readonly int PropVisibleInst = Shader.PropertyToID("_VisibleInstances");
        private static readonly int PropCounter = Shader.PropertyToID("_Counter");
        
        private const int GPUStride = 80;

        [Serializable]
        public struct GrassInstance
        {
            public Vector3 position;
            public float rotation;
            public float height;
            public float width;
            public int textureIndex;
        }

        [Header("Assets")]
        [SerializeField] private Mesh grassMesh;
        [SerializeField] private Material grassMaterial;
        [SerializeField] private int maxGrassCount = 10000;

        [Header("Appearance Settings")]
        [SerializeField] private float minHeight = 0.4f;
        [SerializeField] private float maxHeight = 0.7f;
        [SerializeField] private float minWidth = 0.6f;
        [SerializeField] private float maxWidth = 1f;

        [Tooltip("Maximum distance to render grass")]
        [SerializeField] private float maxRenderDistance = 100f;

        [Tooltip("Enable distance-based culling")]
        [SerializeField] private bool useRenderDistance = true;

        [Header("GPU Culling")]
        [SerializeField] private ComputeShader cullingShader;
        [SerializeField] private Camera debugCamera;

        [HideInInspector] public List<GrassInstance> grassInstances = new();

        private Matrix4x4[] _visibleMatrices;
        private float[] _visibleTextureIndices;
        private int _visibleInstanceCount;

        private ComputeBuffer _allInstancesBuffer;
        private ComputeBuffer _visibleBuffer;
        private ComputeBuffer _counterBuffer;
        private int _totalInstanceCount;

        private MaterialPropertyBlock _propertyBlock;

        private readonly Plane[] _frustumPlanes = new Plane[6];
        private readonly Vector4[] _frustumPlanesVec = new Vector4[6];

        private GPUGrassInstance[] _gpuReadbackBuffer;
        private readonly int[] _counterData = new int[1];

        private int _kernelIndex;

        private void OnEnable()
        {
            _visibleMatrices = new Matrix4x4[maxGrassCount];
            _visibleTextureIndices = new float[maxGrassCount];
            _gpuReadbackBuffer = new GPUGrassInstance[maxGrassCount];
            _propertyBlock = new MaterialPropertyBlock();

            if (cullingShader != null)
                _kernelIndex = cullingShader.FindKernel("CSMain");

            RebuildGPUBuffers();
            RenderPipelineManager.beginCameraRendering += Render;
        }

        private void OnDisable()
        {
            RenderPipelineManager.beginCameraRendering -= Render;
            ReleaseBuffers();
        }

        private void ReleaseBuffers()
        {
            _allInstancesBuffer?.Release();
            _visibleBuffer?.Release();
            _counterBuffer?.Release();
            _allInstancesBuffer = null;
            _visibleBuffer = null;
            _counterBuffer = null;
        }

        private void RebuildGPUBuffers()
        {
            _totalInstanceCount = Mathf.Min(grassInstances.Count, maxGrassCount);

            ReleaseBuffers();

            if (_totalInstanceCount == 0) return;
            if (cullingShader == null)
            {
                Debug.LogWarning("GrassRenderer: Compute Shader is not assigned!");
                return;
            }

            var gpuData = new GPUGrassInstance[_totalInstanceCount];
            for (int i = 0; i < _totalInstanceCount; i++)
            {
                GrassInstance inst = grassInstances[i];
                Quaternion rotation = Quaternion.Euler(0, inst.rotation, 0);
                gpuData[i].mat = Matrix4x4.TRS(inst.position, rotation, new Vector3(inst.width, inst.height, inst.width));
                gpuData[i].posAndTex = new Vector4(inst.position.x, inst.position.y, inst.position.z, inst.textureIndex);
            }

            _allInstancesBuffer = new ComputeBuffer(_totalInstanceCount, GPUStride, ComputeBufferType.Structured);
            _allInstancesBuffer.SetData(gpuData);

            _visibleBuffer = new ComputeBuffer(_totalInstanceCount, GPUStride, ComputeBufferType.Structured);
            _counterBuffer = new ComputeBuffer(1, sizeof(int), ComputeBufferType.Structured);
        }

        private void Render(ScriptableRenderContext ctx, Camera cam)
        {
            if (grassMesh == null || grassMaterial == null || cullingShader == null) return;
            if (_totalInstanceCount == 0 || _allInstancesBuffer == null) return;
            if (cam == null) return;
            if (debugCamera != null) cam = debugCamera;

            if (cam.cameraType == CameraType.Preview) return;
            if (cam.cameraType == CameraType.Reflection) return;

            DispatchCulling(cam);
            ReadbackResults();

            if (_visibleInstanceCount == 0) return;

            _propertyBlock.SetFloatArray(PropIndex, _visibleTextureIndices);
            Graphics.DrawMeshInstanced(
                grassMesh,
                0,
                grassMaterial,
                _visibleMatrices,
                _visibleInstanceCount,
                _propertyBlock,
                ShadowCastingMode.Off,
                true
            );
        }

        private void DispatchCulling(Camera cam)
        {
            _counterBuffer.SetData(new int[] { 0 });

            GeometryUtility.CalculateFrustumPlanes(cam, _frustumPlanes);
            for (int i = 0; i < 6; i++)
                _frustumPlanesVec[i] = new Vector4(
                    _frustumPlanes[i].normal.x,
                    _frustumPlanes[i].normal.y,
                    _frustumPlanes[i].normal.z,
                    _frustumPlanes[i].distance);

            cullingShader.SetInt(PropTotalCount, _totalInstanceCount);
            cullingShader.SetVector(PropCameraPos, cam.transform.position);
            cullingShader.SetFloat(PropMaxDistanceSqr, maxRenderDistance * maxRenderDistance);
            cullingShader.SetInt(PropUseDistance, useRenderDistance ? 1 : 0);
            cullingShader.SetVectorArray(PropFrustumPlanes, _frustumPlanesVec);

            cullingShader.SetBuffer(_kernelIndex, PropAllInstances, _allInstancesBuffer);
            cullingShader.SetBuffer(_kernelIndex, PropVisibleInst, _visibleBuffer);
            cullingShader.SetBuffer(_kernelIndex, PropCounter, _counterBuffer);

            int groups = Mathf.CeilToInt(_totalInstanceCount / 64f);
            cullingShader.Dispatch(_kernelIndex, groups, 1, 1);
        }

        private void ReadbackResults()
        {
            _counterBuffer.GetData(_counterData);
            int count = Mathf.Clamp(_counterData[0], 0, _totalInstanceCount);

            if (count == 0)
            {
                _visibleInstanceCount = 0;
                return;
            }

            _visibleBuffer.GetData(_gpuReadbackBuffer, 0, 0, count);

            for (int i = 0; i < count; i++)
            {
                _visibleMatrices[i] = _gpuReadbackBuffer[i].mat;
                _visibleTextureIndices[i] = _gpuReadbackBuffer[i].posAndTex.w;
            }

            _visibleInstanceCount = count;
        }

        public void UpdateMatrices() => RebuildGPUBuffers();

        public void AddGrassInstance(Vector3 position, float size = 1f)
        {
            if (grassInstances.Count >= maxGrassCount) return;

            grassInstances.Add(new GrassInstance
            {
                position = position,
                rotation = Random.Range(0f, 360f),
                height = Random.Range(minHeight, maxHeight) * size,
                width = Random.Range(minWidth, maxWidth) * size,
                textureIndex = Random.Range(0, 4)
            });

            RebuildGPUBuffers();
        }

        public void ClearGrass()
        {
            grassInstances.Clear();
            RebuildGPUBuffers();
        }

        public void RemoveGrassInRadius(Vector3 center, float radius)
        {
            float sqrRadius = radius * radius;
            grassInstances.RemoveAll(inst =>
                (inst.position - center).sqrMagnitude <= sqrRadius);
            RebuildGPUBuffers();
        }

        public void RandomizeGrassInstance(int index)
        {
            if (index < 0 || index >= grassInstances.Count) return;

            GrassInstance inst = grassInstances[index];
            inst.rotation = Random.Range(0f, 360f);
            inst.height = Random.Range(minHeight, maxHeight);
            inst.width = Random.Range(minWidth, maxWidth);
            inst.textureIndex = Random.Range(0, 4);
            grassInstances[index] = inst;

            RebuildGPUBuffers();
        }

        public void RegenerateGrass()
        {
            for (int i = 0; i < grassInstances.Count; i++)
                RandomizeGrassInstance(i);
        }
    }
}