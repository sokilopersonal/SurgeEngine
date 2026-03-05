float3 _CameraPosition;
half _MaxDistance;
half _FadeRange;

void GrassDistanceFade_half(float3 _WorldPosition, out half Alpha)
{
    half dist = distance(_WorldPosition, _CameraPosition);
    half fadeStart = _MaxDistance * (1.0 - _FadeRange);
    Alpha = 1.0 - smoothstep(fadeStart, _MaxDistance, dist);
    
    if (_MaxDistance <= 0)
    {
        Alpha = 1.0;
    }
}