#ifndef GRASS_INSTANCE_DATA_INCLUDED
#define GRASS_INSTANCE_DATA_INCLUDED

struct GPUGrassInstance
{
    float4x4 mat;
    float4 posAndTex;
};

StructuredBuffer<GPUGrassInstance> _VisibleInstances;

void GetGrassInstanceData_float(uint InstanceID, out float3 WorldPosition, out float TextureIndex)
{
    GPUGrassInstance inst = _VisibleInstances[InstanceID];
    WorldPosition = inst.posAndTex.xyz;
    TextureIndex = inst.posAndTex.w;
}

void GetGrassInstanceScale_float(uint InstanceID, out float Width, out float Height)
{
    float4x4 m = _VisibleInstances[InstanceID].mat;
    Width = length(float3(m[0][0], m[1][0], m[2][0]));
    Height = length(float3(m[0][1], m[1][1], m[2][1]));
}

#endif