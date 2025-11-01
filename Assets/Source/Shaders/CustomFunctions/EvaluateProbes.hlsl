#include "Packages/com.unity.render-pipelines.core/Runtime/Lighting/ProbeVolume/ProbeVolume.hlsl"
#define EVAL_APV
#ifdef EVAL_APV

void EvaluateProbes_half(in float3 posWS, in float2 positionSS, out float3 bakeDiffuseLighting){
    EvaluateAdaptiveProbeVolume(posWS, positionSS, bakeDiffuseLighting);
}

#endif