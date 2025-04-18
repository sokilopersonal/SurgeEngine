
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

namespace SurgeEngine.Code.Shaders
{
    public class CapsuleShadowPass : CustomPass
    {
        [Range(0, 1)] public float shadowIntensity = 1.0f;
        [Range(0, 1)] public float shadowBias = 1.0f;
        [Range(0.02f, 20f)] public float maxSoftness = 0.05f;
        [Range(0, 10)] public float penumbraSize = 0.05f;
        [Range(1, 10)] public float blendDistance = 0.2f;
        [Range(0.1f, 75)] public float fadeDistance = 0.2f;
        [Range(0, 1)] public float shadowBlend;
        
        public List<CapsuleCollider> capsuleColliders = new List<CapsuleCollider>();
    
        private Material capsuleShadowMaterial;
        private ComputeBuffer capsuleBuffer;
        private List<CapsuleData> capsuleDataList = new List<CapsuleData>();
    
        struct CapsuleData
        {
            public Vector4 positionStart;
            public Vector4 positionEnd;
            public Vector4 color;
        }
    
        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            capsuleShadowMaterial = CoreUtils.CreateEngineMaterial("Hidden/CapsuleShadow");
            name = "Capsule Shadows";
        }
    
        protected override void Execute(CustomPassContext ctx)
        {
            if (capsuleShadowMaterial == null || RenderSettings.sun == null)
                return;
        
            if (capsuleColliders.Count == 0)
                return;

            UpdateCapsuleData();

            capsuleShadowMaterial.SetBuffer("_CapsuleBuffer", capsuleBuffer);
            capsuleShadowMaterial.SetInt("_CapsuleCount", capsuleColliders.Count);
            capsuleShadowMaterial.SetFloat("_ShadowIntensity", shadowIntensity);
            capsuleShadowMaterial.SetFloat("_ShadowBias", shadowBias);
            capsuleShadowMaterial.SetFloat("_MaxSoftness", maxSoftness);
            capsuleShadowMaterial.SetFloat("_PenumbraSize", penumbraSize);
            capsuleShadowMaterial.SetFloat("_BlendDistance", blendDistance);
            capsuleShadowMaterial.SetFloat("_FadeDistance", fadeDistance);
            capsuleShadowMaterial.SetFloat("_OverlapBlend", shadowBlend);
            capsuleShadowMaterial.SetVector("_LightDirection", -RenderSettings.sun.transform.forward);

            CoreUtils.SetRenderTarget(ctx.cmd, ctx.cameraColorBuffer, ctx.cameraDepthBuffer);
            CoreUtils.DrawFullScreen(ctx.cmd, capsuleShadowMaterial, shaderPassId: 0);
        }
    
        private void UpdateCapsuleData()
        {
            if (capsuleBuffer == null || capsuleBuffer.count != capsuleColliders.Count)
            {
                if (capsuleBuffer != null)
                    capsuleBuffer.Release();
                
                if (capsuleColliders.Count > 0)
                    capsuleBuffer = new ComputeBuffer(capsuleColliders.Count, System.Runtime.InteropServices.Marshal.SizeOf(typeof(CapsuleData)));
            }

            if (capsuleColliders.Count == 0)
                return;
            
            capsuleDataList.Clear();

            foreach (var capsule in capsuleColliders)
            {
                if (!capsule.gameObject.activeSelf)
                    continue;
                
                Vector3 center = capsule.transform.TransformPoint(capsule.center);
                float height = capsule.height;
                float radius = capsule.radius;
            
                Vector3 direction = Vector3.up;
                if (capsule.direction == 0)
                    direction = capsule.transform.right;
                else if (capsule.direction == 1)
                    direction = capsule.transform.up;
                else if (capsule.direction == 2) direction = capsule.transform.forward;
                
                direction = direction.normalized * (height * 0.5f - radius);
                
                Vector3 start = center - direction;
                Vector3 end = center + direction;
            
                CapsuleData data = new CapsuleData();
                data.positionStart = new Vector4(start.x, start.y, start.z, radius);
                data.positionEnd = new Vector4(end.x, end.y, end.z, 0);
                data.color = new Vector4(0, 0, 0, 1);

                capsuleDataList.Add(data);
            }

            capsuleBuffer.SetData(capsuleDataList);
        }
    
        protected override void Cleanup()
        {
            CoreUtils.Destroy(capsuleShadowMaterial);
        
            if (capsuleBuffer != null)
            {
                capsuleBuffer.Release();
                capsuleBuffer = null;
            }
        }
    }
}