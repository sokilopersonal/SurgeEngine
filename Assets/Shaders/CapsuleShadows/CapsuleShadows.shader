Shader "Hidden/CapsuleShadow"
{
    HLSLINCLUDE
    #pragma target 5.0

    #pragma shader_feature _CAPSULE_SHADOWS_ON
    
    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    
    struct CapsuleData
    {
        float4 positionStart; // xyz: position, w: radius
        float4 positionEnd;   // xyz: position, w: unused
        float4 color;         // rgb: color, a: intensity
    };
    
    StructuredBuffer<CapsuleData> _CapsuleBuffer;
    int _CapsuleCount;
    float _ShadowIntensity;
    float _MaxSoftness;
    float _PenumbraSize;
    float _BlendDistance;
    float _FadeDistance;
    float _OverlapBlend;
    float3 _LightDirection;
    float _ShadowFalloff;
    float _CapsuleShadowsEnabled; // 1.0 for enabled, 0.0 for disabled
    
    struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };
    
    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };
    
    Varyings Vert(Attributes input)
    {
        Varyings output;
        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
        output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
        output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
        return output;
    }

    float DistanceToSegment(float3 p, float3 a, float3 b)
    {
        float3 pa = p - a;
        float3 ba = b - a;
        float t = clamp(dot(pa, ba) / dot(ba, ba), 0.0, 1.0);
        return length(pa - ba * t);
    }
    
    float4 CalculateCapsuleShadow(float3 worldPos, CapsuleData capsule)
    {
        float3 start = capsule.positionStart.xyz;
        float3 end = capsule.positionEnd.xyz;
        float radius = capsule.positionStart.w;
        
        float3 lightDir = normalize(_LightDirection);
        
        float3 capsuleMid = (start + end) * 0.5;
        float distToMid = distance(worldPos, capsuleMid);
        float maxShadowDistance = _FadeDistance;
        float distanceFade = 1.0 - smoothstep(0, maxShadowDistance, distToMid);

        if (distanceFade <= 0.0)
            return float4(0, 0, 0, 0);
        
        float planeDistance = dot(worldPos, lightDir);
        float startDist = dot(start, lightDir) - planeDistance;
        float endDist = dot(end, lightDir) - planeDistance;
        
        float3 startProj = start - startDist * lightDir;
        float3 endProj = end - endDist * lightDir;
        
        float dist = DistanceToSegment(worldPos, startProj, endProj);
        
        float avgDist = (abs(startDist) + abs(endDist)) * 0.5;
        float shadowRadius = radius * (1.0 + avgDist * 0.1);
        
        float shadowDist = dist - shadowRadius;
        
        float penumbraSize = radius * _PenumbraSize; 
        float occluderDist = avgDist;
        float softness = min(penumbraSize * occluderDist, _MaxSoftness);
        
        float shadow = 1.0 - smoothstep(0, softness + 0.001, shadowDist);
        
        float fadeDistance = _BlendDistance + softness;
        shadow *= 1.0 - smoothstep(0, fadeDistance, shadowDist);
        shadow *= distanceFade;
        
        shadow *= pow(distanceFade, _ShadowFalloff);
        
        return float4(capsule.color.rgb, shadow * capsule.color.a * _ShadowIntensity);
    }
    
    float4 CombineShadows(float4 finalColor, float4 newShadow)
    {
        float alpha = newShadow.a;
        finalColor.rgb = lerp(finalColor.rgb, finalColor.rgb * (1.0 - alpha), alpha);
        return finalColor;
    }
    
    float4 Fragment(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
        
        #if !defined(_CAPSULE_SHADOWS_ON)
            return float4(1, 1, 1, 1);
        #endif

        float2 uv = input.texcoord;
        float depth = LoadCameraDepth(input.positionCS.xy);
        if (depth == UNITY_RAW_FAR_CLIP_VALUE)
            return float4(1, 1, 1, 1);
        
        float3 camRelPos = ComputeWorldSpacePosition(uv, depth, UNITY_MATRIX_I_VP);
        float3 worldPos = camRelPos + _WorldSpaceCameraPos;
        float4 finalColor = float4(1, 1, 1, 1);
        
        float combinedA = 0;
        for (int i = 0; i < _CapsuleCount; i++)
        {
            float4 sh = CalculateCapsuleShadow(worldPos, _CapsuleBuffer[i]);
            float mx    = max(combinedA, sh.a);
            float sum   = combinedA + sh.a;
            combinedA   = lerp(mx, sum, _OverlapBlend);
        }
        finalColor.rgb *= 1 - combinedA;
        
        return finalColor;
    }
    ENDHLSL

    SubShader
    {
        Tags { "RenderPipeline" = "HDRenderPipeline" }
        
        Pass
        {
            Name "Capsule Shadows"
            
            ZWrite Off
            ZTest Always
            Blend DstColor Zero
            Cull Off
            
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Fragment

            ENDHLSL
        }
    }
    Fallback Off
}