using UnityEngine;

namespace SurgeEngine.Source.Code.Infrastructure.Config.Sonic
{
    [CreateAssetMenu(fileName = "SlideConfig", menuName = "Surge Engine/Configs/Sonic/Slide", order = 0)]
    public class SlideConfig : ScriptableObject
    {
        public float deceleration = 6f;
        public float minSpeed = 5f;
    }
}