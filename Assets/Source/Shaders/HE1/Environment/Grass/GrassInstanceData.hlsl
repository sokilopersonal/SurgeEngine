#ifndef GRASS_INSTANCE_DATA_INCLUDED
#define GRASS_INSTANCE_DATA_INCLUDED

struct GPUGrassInstance
{
    float4x4 mat;
    float4 posAndTex;
};

int _TotalCount;
StructuredBuffer<GPUGrassInstance> _VisibleInstances;

void GetGrassInstanceData_float(uint InstanceID, out float3 WorldPosition, out float TextureIndex, out half Width, out half Height, out half Angle)
{
    if (_TotalCount == 0) return;
    
    GPUGrassInstance inst = _VisibleInstances[InstanceID];
    float4x4 m = inst.mat;
    WorldPosition = inst.posAndTex.xyz;
    TextureIndex = inst.posAndTex.w;
    Width = length(half3(m[0][0], m[1][0], m[2][0]));
    Height = length(half3(m[0][1], m[1][1], m[2][1]));
    Angle = atan2(m[2][0] / Width, m[0][0] / Width);
}

#endif