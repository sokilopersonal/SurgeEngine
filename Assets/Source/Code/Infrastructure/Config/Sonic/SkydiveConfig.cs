using UnityEngine;

namespace SurgeEngine.Source.Code.Infrastructure.Config.Sonic
{
    [CreateAssetMenu(fileName = "SkydiveConfig", menuName = "Surge Engine/Configs/Sonic/Skydive", order = 0)]
    public class SkydiveConfig : ScriptableObject
    {
        public float fallSpeed = 10.0f;
        public float diveSpeed = 20.0f;
        public float lerpSpeed = 5.0f;
    }
}