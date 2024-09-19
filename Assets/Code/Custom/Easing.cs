using UnityEngine;

namespace SurgeEngine.Code.Custom
{
    public enum Easing
    {
        Linear,
        InSine,
        OutSine,
        InOutSine,
        InCubic,
        OutCubic,
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
            }
            
            return 0f;
        }
    }
}