using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;
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
        private static readonly int PropCameraPos = Shader.PropertyToID("_CameraPosition");
        private static readonly int PropMaxDistanceSqr = Shader.PropertyToID("_MaxDistanceSqr");
        private static readonly int PropUseDistance = Shader.PropertyToID("_UseDistance");
        private static readonly int PropFrustumPlanes = Shader.PropertyToID("_FrustumPlanes");
        private static readonly int PropTotalCount = Shader.PropertyToID("_TotalCount");
        private static readonly int PropAllInstances = Shader.PropertyToID("_AllInstances");
        private static readonly int PropVisibleInst = Shader.PropertyToID("_VisibleInstances");
        private static readonly int PropCounter = Shader.PropertyToID("_Counter");
        private static readonly int PropArgs = Shader.PropertyToID("_Args");

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
        [SerializeField] private int maxGrassCount = 100000;

        [Header("Appearance Settings")]
        [SerializeField] private float minHeight = 0.4f;
        [SerializeField] private float maxHeight = 0.7f;
        [SerializeField] private float minWidth = 0.6f;
        [SerializeField] private float maxWidth = 1f;
        [SerializeField] private float maxRenderDistance = 100f;
        [SerializeField] private bool useRenderDistance = true;

        [Header("GPU Culling")]
        [SerializeField] private ComputeShader cullingShader;
        [SerializeField] private Camera debugCamera;

        [HideInInspector] public List<GrassInstance> grassInstances = new();
        [HideInInspector] public float brushSize = 2f;
        [HideInInspector] public float brushDensity = 5f;

        private RenderParams _rp;

        private ComputeBuffer _allInstancesBuffer;
        private ComputeBuffer _visibleBuffer;
        private ComputeBuffer _counterBuffer;
        private GraphicsBuffer _argsBuffer;
        private int _totalInstanceCount;

        private MaterialPropertyBlock _propertyBlock;

        private readonly Plane[] _frustumPlanes = new Plane[6];
        private readonly Vector4[] _frustumPlanesVec = new Vector4[6];
        private readonly int[] _counterReset = new int[1];

        private int _kernelCSMain;
        private int _kernelCopyArgs;

        private void OnEnable()
        {
            _propertyBlock = new MaterialPropertyBlock();

            if (cullingShader != null)
            {
                _kernelCSMain = cullingShader.FindKernel("CSMain");
                _kernelCopyArgs = cullingShader.FindKernel("CopyArgs");
            }

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
            _argsBuffer?.Dispose();
            _allInstancesBuffer = null;
            _visibleBuffer = null;
            _counterBuffer = null;
            _argsBuffer = null;
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

            uint[] args = new uint[5];
            args[0] = grassMesh.GetIndexCount(0);
            args[1] = 0;
            args[2] = grassMesh.GetIndexStart(0);
            args[3] = grassMesh.GetBaseVertex(0);
            args[4] = 0;

            _argsBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, 5, sizeof(uint));
            _argsBuffer.SetData(args);

            _rp = new RenderParams(grassMaterial)
            {
                worldBounds = new Bounds(Vector3.zero, Vector3.one * 10000f),
                matProps = _propertyBlock,
                shadowCastingMode = ShadowCastingMode.Off,
                receiveShadows = true
            };
        }

        private void Render(ScriptableRenderContext ctx, Camera cam)
        {
            if (grassMesh == null || grassMaterial == null || cullingShader == null) return;
            if (_totalInstanceCount == 0 || _allInstancesBuffer == null) return;
            if (cam == null) return;
            if (cam.cameraType == CameraType.Preview) return;

            if (debugCamera != null) cam = debugCamera;

            DispatchCulling(cam);

            _rp.matProps.SetBuffer(PropVisibleInst, _visibleBuffer);

            Graphics.RenderMeshIndirect(_rp, grassMesh, _argsBuffer);
        }

        private void DispatchCulling(Camera cam)
        {
            _counterBuffer.SetData(_counterReset);

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

            cullingShader.SetBuffer(_kernelCSMain, PropAllInstances, _allInstancesBuffer);
            cullingShader.SetBuffer(_kernelCSMain, PropVisibleInst, _visibleBuffer);
            cullingShader.SetBuffer(_kernelCSMain, PropCounter, _counterBuffer);

            cullingShader.Dispatch(_kernelCSMain, Mathf.CeilToInt(_totalInstanceCount / 64f), 1, 1);

            cullingShader.SetBuffer(_kernelCopyArgs, PropCounter, _counterBuffer);
            cullingShader.SetBuffer(_kernelCopyArgs, PropArgs, _argsBuffer);

            cullingShader.Dispatch(_kernelCopyArgs, 1, 1, 1);
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
            grassInstances.RemoveAll(inst => (inst.position - center).sqrMagnitude <= sqrRadius);
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