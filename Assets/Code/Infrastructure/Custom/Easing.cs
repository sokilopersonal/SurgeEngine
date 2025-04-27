using UnityEngine;

namespace SurgeEngine.Code.Infrastructure.Custom
{
    public enum Easing
    {
        Linear,
        InSine,
        OutSine,
        InOutSine,
        InCubic,
        OutCubic,
        InCirc,
        InQuint,
        InQuart,
        InExpo,
        Gens
    }
    
    public static class Easings
    {
        public static float Get(Easing easing, float t)
        {
            switch (easing)
            {
                case Easing.Linear:
                    return t;
                case Easing.InSine:
                    return 1f - Mathf.Cos((t * Mathf.PI) / 2f);
                case Easing.OutSine:
                    return Mathf.Sin((t * Mathf.PI) / 2f);
                case Easing.InOutSine:
                    return -(Mathf.Cos(Mathf.PI * t) - 1) / 2f;
                case Easing.InCubic:
                    return t * t * t;
                case Easing.OutCubic:
                    return 1f - Mathf.Pow(1f - t, 3f);
                case Easing.InCirc:
                    return 1 - Mathf.Sqrt(1 - Mathf.Pow(t, 2));
                case Easing.InQuint:
                    return t * t * t * t * t;
                case Easing.InQuart:
                    return t * t * t * t;
                case Easing.InExpo:
                    return Mathf.Pow(2, 10 * (t - 1));
                case Easing.Gens:
                    return 0.5f - Mathf.Cos(Mathf.PI * t) / 2;
            }
            
            return 0f;
        }
    }
}