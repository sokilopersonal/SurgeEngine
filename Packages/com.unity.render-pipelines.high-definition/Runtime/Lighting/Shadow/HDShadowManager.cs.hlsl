//
// This file was automatically generated. Please don't edit by hand. Execute Editor command [ Edit > Rendering > Generate Shader Includes ] instead
//

#ifndef HDSHADOWMANAGER_CS_HLSL
#define HDSHADOWMANAGER_CS_HLSL
// Generated from UnityEngine.Rendering.HighDefinition.HDDirectionalShadowData
// PackingRules = Exact
// Note: MAX_SHADOW_CASCADES must stay in sync with HDShadowSettings.k_MaxCascades in C#
#define MAX_SHADOW_CASCADES 6
struct HDDirectionalShadowData
{
    float4 sphereCascades[MAX_SHADOW_CASCADES];
    real4 cascadeDirection;
    real cascadeBorders[MAX_SHADOW_CASCADES];
    float fadeScale;
    float fadeBias;
};

// Generated from UnityEngine.Rendering.HighDefinition.HDShadowData
// PackingRules = Exact
struct HDShadowData
{
    float3 rot0;
    float3 rot1;
    float3 rot2;
    float3 pos;
    float4 proj;
    float2 atlasOffset;
    float worldTexelSize;
    float normalBias;
    real4 zBufferParam;
    float4 shadowMapSize;
    float4 shadowFilterParams0;
    float4 dirLightPCSSParams0;
    float4 dirLightPCSSParams1;
    float3 cacheTranslationDelta;
    float isInCachedAtlas;
    float4x4 shadowToWorld;
};


#endif
