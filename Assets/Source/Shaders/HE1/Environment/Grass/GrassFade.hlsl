float3 _CameraPosition;
float _MaxDistance;
float _FadeRange;

void GrassDistanceFade_float(float3 _WorldPosition, out float Alpha)
{
    float dist = distance(_WorldPosition, _CameraPosition);
    float fadeStart = _MaxDistance * (1.0 - _FadeRange);
    Alpha = 1.0 - smoothstep(fadeStart, _MaxDistance, dist);
    
    if (_MaxDistance <= 0)
    {
        Alpha = 1.0;
    }
}